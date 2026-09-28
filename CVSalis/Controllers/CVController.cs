using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using CVSalis.Data.Dto;
using CVSalis.Data.Repo;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Globalization;
using System.Text;
using DinkToPdf.Contracts;
using SelectPdf;
using CVSalis.Config;
using RazorEngine;
using RazorEngine.Templating;

namespace CVSalis.Controllers
{
    public class CVController : Controller
    {
        private readonly ICVRepo _cvRepo;
        private IWebHostEnvironment webHostEnvironment;
        string localPath = "\\Image\\";
        string localPathPDF = "\\PDF\\";
        public CVController(ICVRepo cVRepo, IWebHostEnvironment environment)
        {
            _cvRepo = cVRepo;
            webHostEnvironment = environment;
        }
        public IActionResult CurriculumVitae()
        {
            return View();
        }

        public IActionResult SamplePrint()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SubmitNewCV(DetailCV param)
        {
            RespSaveData result = new RespSaveData();
            try
            {
                if (param.birth_date != null && DateTime.Now.Year - param.birth_date.Year < 21)
                {
                    result.is_ok = false;
                    result.messageUI = "age must be more than 21 years old";
                    return Json(result);
                }

                if (param.Experience_List == null || param.Experience_List.Count < 0)
                {
                    result.is_ok = false;
                    result.messageUI = "Experience must be filled";
                    return Json(result);
                }

                //save CV and calculate total exp
                if (param.Experience_List != null && param.Experience_List.Count > 0)
                {
                    var lasttExp = param.Experience_List.Last().periode_end;
                    var firstExp = param.Experience_List.First().periode_start;
                    param.total_exp = lasttExp - firstExp;
                }
                param.isCreated = true;
                var save = _cvRepo.CreateOrUpdateCV(param);
                result.is_ok = true;
                result.messageUI = "success submit";
            }
            catch (Exception ex)
            {
                result.is_ok = false;
                result.messageUI = "Submit Failed";
                result.messageConsole = ex.Message;
            }

            return Json(result);
        }

        [HttpPut]
        public JsonResult EditCV(DetailCV param)
        {
            RespSaveData result = new RespSaveData();
            try
            {
                if (param.birth_date != null && DateTime.Now.Year - param.birth_date.Year < 21)
                {
                    result.is_ok = false;
                    result.messageUI = "age must be more than 21 years old";
                    return Json(result);
                }

                if (param.Experience_List == null || param.Experience_List.Count < 0)
                {
                    result.is_ok = false;
                    result.messageUI = "Experience must be filled";
                    return Json(result);
                }

                if (param.Experience_List != null && param.Experience_List.Count > 0)
                {
                    var lasttExp = param.Experience_List.Last().periode_end;
                    var firstExp = param.Experience_List.First().periode_start;
                    param.total_exp = lasttExp - firstExp;
                }
                //save CV
                param.isCreated = false;
                var save = _cvRepo.CreateOrUpdateCV(param);
                result.is_ok = true;
                result.messageUI = "success submit";
            }
            catch (Exception ex)
            {
                result.is_ok = false;
                result.messageUI = "Edit Failed";
                result.messageConsole = ex.Message;
            }

            return Json(result);
        }

        [HttpGet]
        public string GetDetailCVById(long id)
        {
            RespCVDetail result = new RespCVDetail();
            try
            {
                var data = _cvRepo.GetDetailCVById(id);
                if (data.Result.employee_no == 0)
                {
                    result.message = "cv not found";
                    result.is_ok = false;
                    return JsonConvert.SerializeObject(result);
                }

                result.dataDetail = data.Result;
                result.message = "success";
                result.is_ok = true;

            }
            catch (Exception ex)
            {
                result.is_ok = false;
                result.message = "Edit Failed";

            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpDelete]
        public string DeleteCV(long id)
        {
            RespDeleteCV result = new RespDeleteCV();
            try
            {
                var deleted = _cvRepo.DeleteCV(id);
                result.is_ok = true;
                result.message = "success";
            }
            catch (Exception ex)
            {
                result.is_ok = true;
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        public string GetListCV()
        {
            RespCVDetail result = new RespCVDetail();
            try
            {
                var data = _cvRepo.GetListCV();
                if (data.Result.Count == 0)
                {
                    result.message = "list cv not found";
                    result.is_ok = false;
                    return JsonConvert.SerializeObject(result);
                }

                result.listCV = data.Result;
                result.message = "success";
                result.is_ok = true;

            }
            catch (Exception ex)
            {
                result.is_ok = false;
                result.message = "Edit Failed";

            }
            return JsonConvert.SerializeObject(result);
        }

        public string GetActualPath(string FileName)
        {
            return Path.Combine(webHostEnvironment.WebRootPath + localPath, FileName);
        }

        public async Task<ActionResult> DataUpload()
        {
            //string response = string.Empty;
            RespUploadFile resp = new RespUploadFile();
            var Files = Request.Form.Files;
            try
            {
                foreach (IFormFile source in Files)
                {
                    string FileName = source.FileName;

                    string filePath = GetActualPath(FileName);

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);

                    using (FileStream stream = System.IO.File.Create(filePath))
                    {
                        await source.CopyToAsync(stream);
                        resp.message = "OK";
                        resp.filePath = FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                resp.message = "Error: " + ex.Message.ToString();
            }

            return Ok(resp);
        }

        [HttpPost]
        public JsonResult GeneratePDF(long cvID)
        {
            RespFileModel RespFile = new RespFileModel();
            try
            {
                var data = _cvRepo.GetDetailCVById(cvID);
                if (data.Result.employee_no == 0)
                {
                    RespFile.is_ok = false;
                    RespFile.message = "Error while get data CV";
                    return Json(RespFile);
                }
                var dataCV = data.Result;
                string dirPath = Path.Combine(webHostEnvironment.WebRootPath + localPathPDF);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                string filePath = Path.Combine(dirPath, "Export_CV_" + dataCV.employee_name + ".pdf");
                var htmlPDF = GetHTMLStringNew(dataCV, webHostEnvironment.WebRootPath);

                HtmlToPdf converter = new HtmlToPdf();
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                converter.Options.MarginLeft = 0;
                converter.Options.MarginRight = 0;
                converter.Options.MarginTop = 0;
                converter.Options.MarginBottom = 0;

                PdfDocument doc = converter.ConvertHtmlString(htmlPDF);

                byte[] pdfFile = doc.Save();
                doc.Close();

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                System.IO.File.WriteAllBytes(filePath, pdfFile);

                FileModel filesdata = new FileModel()
                {
                    filePath = "/PDF/" + "Export_CV_" + dataCV.employee_name + ".pdf"
                };
                RespFile.is_ok = true;
                RespFile.message = "File downloaded";
                RespFile.data = filesdata;

            }
            catch (Exception ex)
            {
                RespFile.is_ok = false;
                RespFile.message = "Error while processing data CV";
            }

            return Json(RespFile);
        }

        public static string GetHTMLStringNew(DetailCV dataCV, string hostingPath)
        {
            var sb = new StringBuilder();

            bool hasRealPhoto = false;
            string photoDiskPath = "";
            if (!string.IsNullOrWhiteSpace(dataCV.image))
            {
                string img = dataCV.image.Trim().Replace('\\', '/');
                string fileName = Path.GetFileName(img);
                if (!string.IsNullOrWhiteSpace(fileName) && !string.Equals(fileName, "cats.png", StringComparison.OrdinalIgnoreCase))
                {
                    string fullPath = Path.Combine(hostingPath, "Image", fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        if (img.StartsWith("/Image/", StringComparison.OrdinalIgnoreCase) || img.StartsWith("/image/", StringComparison.OrdinalIgnoreCase))
                        {
                            hasRealPhoto = true;
                            photoDiskPath = fullPath;
                        }
                    }
                }
            }

            int yearDate = dataCV.birth_date != default ? dataCV.birth_date.Year : 0;
            int now = DateTime.UtcNow.Year;
            dataCV.ages = yearDate > 1900 ? now - yearDate : 0;

            var hardskills = new List<string>();
            if (!string.IsNullOrWhiteSpace(dataCV.hard_skill))
            {
                hardskills = dataCV.hard_skill.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }

            var softskills = new List<string>();
            if (!string.IsNullOrWhiteSpace(dataCV.soft_skill))
            {
                softskills = dataCV.soft_skill.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
            }

            string focusEdu = dataCV.focus_education ?? "";
            if (!string.IsNullOrWhiteSpace(focusEdu) && !focusEdu.Contains(" "))
            {
                var sbFocus = new StringBuilder();
                for (int i = 0; i < focusEdu.Length; i++)
                {
                    if (i > 0 && char.IsUpper(focusEdu[i]))
                        sbFocus.Append(' ');
                    sbFocus.Append(focusEdu[i]);
                }
                focusEdu = sbFocus.ToString();
            }

            sb.AppendFormat(@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'/>
    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\fonts\\boxicons.css'/>
    <link rel='stylesheet' href='{0}\\css\\sb-admin-2.css'/>
    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\css\\core.css' class='template-customizer-core-css'/>
    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\css\\theme-default.css' class='template-customizer-theme-css'/>
    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\css\\demo.css'/>
    <style>
        body {{
            background-color: #ffffff;
            font-family: 'Public Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            color: #566a7f;
            margin: 0;
            padding: 25px 30px;
        }}
        .section-header {{
            font-size: 13px;
            font-weight: 700;
            letter-spacing: 0.8px;
            text-transform: uppercase;
            color: #566a7f;
            border-bottom: 1px solid #d9dee3;
            padding-bottom: 6px;
            margin-bottom: 14px;
        }}
        .meta-label {{
            font-size: 11px;
            color: #a1acb8;
            font-weight: 600;
            text-transform: uppercase;
            display: block;
            margin-bottom: 2px;
        }}
        .meta-val {{
            font-size: 13px;
            color: #566a7f;
        }}
    </style>
</head>
<body>
    <div>
        <!-- Header -->
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='width: 135px; vertical-align: top; text-align: left; padding-right: 20px;'>", hostingPath);

            if (hasRealPhoto)
            {
                sb.AppendFormat(@"<img src='{0}' class='d-block rounded' style='width: 120px; height: 120px; object-fit: cover; border-radius: 0.375rem; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);' alt='Profile Photo' />", photoDiskPath);
            }
            else
            {
                sb.Append(@"<div style='width: 120px; height: 120px; border-radius: 50%; background-color: #ffffff; border: 1px solid #d9dee3; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.08); display: inline-block;'></div>");
            }

            sb.AppendFormat(@"</td>
                <td style='vertical-align: top;'>
                    <h4 style='color: #696cff; font-weight: 700; margin: 0 0 3px 0; font-size: 20px;'>{0}</h4>
                    <div style='font-size: 14px; font-weight: 600; color: #697a8d; margin-bottom: 14px;'>{1}</div>
                    
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='width: 20%; vertical-align: top; padding-right: 10px; padding-bottom: 8px;'>
                                <span class='meta-label'>Age</span>
                                <span class='meta-val'>{2} years</span>
                            </td>
                            <td style='width: 25%; vertical-align: top; padding-right: 10px; padding-bottom: 8px;'>
                                <span class='meta-label'>Phone</span>
                                <span class='meta-val'>{3}</span>
                            </td>
                            <td style='width: 35%; vertical-align: top; padding-right: 10px; padding-bottom: 8px;'>
                                <span class='meta-label'>Email</span>
                                <span class='meta-val' style='word-break: break-all;'>{4}</span>
                            </td>
                            <td style='width: 20%; vertical-align: top; padding-bottom: 8px;'>
                                <span class='meta-label'>Experience</span>
                                <span class='meta-val'>{5} years</span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan='4' style='vertical-align: top; padding-top: 4px;'>
                                <span class='meta-label'>Location</span>
                                <span class='meta-val' style='word-wrap: break-word; overflow-wrap: break-word; white-space: pre-line; line-height: 1.4;'>{6}</span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <!-- Section Divider -->
        <hr style='border: none; border-top: 1px solid #d9dee3; margin: 20px 0;' />

        <!-- 2-Column Content: Experience & Skills/Education -->
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <!-- Left Column: EXPERIENCE -->
                <td style='width: 65%; vertical-align: top; padding-right: 25px; border-right: 1px solid #e7e7e8;'>
                    <div class='section-header'>EXPERIENCE</div>",
                dataCV.employee_name,
                dataCV.position,
                dataCV.ages,
                dataCV.phone,
                dataCV.email,
                dataCV.total_exp,
                dataCV.address);

            if (dataCV.Experience_List != null && dataCV.Experience_List.Count > 0)
            {
                for (int i = 0; i < dataCV.Experience_List.Count; i++)
                {
                    var exp = dataCV.Experience_List[i];
                    bool isLast = (i == dataCV.Experience_List.Count - 1);
                    sb.AppendFormat(@"
                    <div style='margin-bottom: {6}; {7}'>
                        <div style='font-size: 13px; font-weight: 700; color: #566a7f;'>{0} - {1}</div>
                        <div style='font-size: 13px; font-weight: 600; color: #566a7f; margin-top: 2px;'>{2}</div>
                        <div style='font-size: 12px; color: #696cff; font-weight: 600; margin-bottom: 6px;'>{3}</div>
                        <div style='font-size: 12px; line-height: 1.5; color: #697a8d; margin-bottom: 6px; word-wrap: break-word;'>{4}</div>
                        <div style='font-size: 12px; color: #566a7f;'><strong>Tools:</strong> {5}</div>
                    </div>",
                        exp.periode_start,
                        exp.periode_end,
                        exp.company,
                        exp.role,
                        exp.resposibility_desc,
                        exp.tech_tools,
                        isLast ? "0" : "16px",
                        isLast ? "" : "padding-bottom: 16px; border-bottom: 1px dashed #e7e7e8;");
                }
            }

            sb.AppendFormat(@"
                </td>
                <!-- Right Column: SKILLS & EDUCATION -->
                <td style='width: 35%; vertical-align: top; padding-left: 25px;'>
                    <div class='section-header'>SKILLS</div>
                    
                    <div style='font-size: 12px; font-weight: 700; color: #566a7f; margin-bottom: 5px;'>Hard Skill :</div>
                    <ul style='padding-left: 18px; margin: 0 0 14px 0; font-size: 12px; color: #697a8d;'>");

            foreach (var hs in hardskills)
            {
                sb.AppendFormat(@"<li style='margin-bottom: 3px;'>{0}</li>", hs);
            }

            sb.AppendFormat(@"</ul>
                    <div style='font-size: 12px; font-weight: 700; color: #566a7f; margin-bottom: 5px;'>Soft Skill :</div>
                    <ul style='padding-left: 18px; margin: 0 0 18px 0; font-size: 12px; color: #697a8d;'>");

            foreach (var ss in softskills)
            {
                sb.AppendFormat(@"<li style='margin-bottom: 3px;'>{0}</li>", ss);
            }

            sb.AppendFormat(@"</ul>

                    <!-- EDUCATION -->
                    <div class='section-header' style='margin-top: 18px;'>EDUCATION</div>
                    <div style='padding-left: 2px;'>
                        <div style='font-size: 13px; font-weight: 700; color: #566a7f;'>{0}</div>
                        <div style='font-size: 12px; color: #697a8d; margin-top: 3px;'>Graduate on {1}</div>
                        <div style='font-size: 12px; color: #697a8d; margin-top: 2px;'>{2} of {3}</div>
                        <div style='font-size: 12px; font-weight: 600; color: #566a7f; margin-top: 4px;'>IPK {4}</div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>",
                dataCV.education_name,
                dataCV.year_education,
                dataCV.education_type,
                focusEdu,
                dataCV.ipk);

            return sb.ToString();
        }
    }
}
