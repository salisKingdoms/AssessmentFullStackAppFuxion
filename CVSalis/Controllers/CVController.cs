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
            //RespLOIPrintModel resultLOI = new RespLOIPrintModel();
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
                string filePath = Path.Combine(webHostEnvironment.WebRootPath + localPathPDF, "Export_CV_" + dataCV.employee_name + ".pdf");
                var htmlPDF = GetHTMLStringNew(dataCV, webHostEnvironment.WebRootPath);
                //string layoutPath = webHostEnvironment.ContentRootPath + "/Views/Shared/_Layout.cshtml";
                //string layoutContent = System.IO.File.ReadAllText(layoutPath);
                //string mergedHtml = Engine.Razor.RunCompile(layoutContent, "layoutKey", null, new { BodyContent = htmlPDF });
                HtmlToPdf converter = new HtmlToPdf();
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.MarginTop = 24;
                converter.Options.MarginBottom = 24;
                converter.Options.MarginLeft = 24;
                converter.Options.MarginRight = 24;
                converter.Options.WebPageWidth = 800;
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
            // Print styles must not inherit responsive grids or fixed-height screen cards.
            var sb = new StringBuilder(@"<!DOCTYPE html>
<html><head><meta charset='utf-8'><style>
* { box-sizing: border-box; }
body { margin: 0; color: #303846; font: 14px/1.5 Arial, sans-serif; }
h1 { margin: 0; color: #5146b8; font-size: 25px; }
h2 { margin: 20px 0 10px; padding-bottom: 5px; border-bottom: 1px solid #cbd0d8;
     font-size: 16px; page-break-after: avoid; }
h3 { margin: 0 0 5px; font-size: 14px; page-break-after: avoid; }
p { margin: 0 0 8px; }
.header { width: 100%; table-layout: fixed; border-collapse: collapse; }
.header td { vertical-align: top; }
.photo-cell { width: 160px; padding-right: 20px; }
.photo { width: 140px; height: 140px; border-radius: 4px; }
.photo-empty { width: 140px; height: 140px; border: 1px solid #cbd0d8;
               border-radius: 50%; background: white; }
.field { margin: 4px 0; }
.label { font-weight: bold; }
.text { white-space: pre-wrap; word-wrap: break-word; overflow-wrap: break-word; }
.entry { margin-bottom: 14px; }
.entry + .entry { padding-top: 12px; border-top: 1px solid #e0e3e8; }
.education { page-break-inside: avoid; }
ul { margin: 4px 0 12px; padding-left: 22px; }
li { word-wrap: break-word; }
</style></head><body><table class='header'><tr><td class='photo-cell'>");
            var photo = GetPdfPhotoUri(dataCV.image, hostingPath);
            if (photo == null)
                sb.Append("<div class='photo-empty'></div>");
            else
                // Like Edit, show the entire square image without circular clipping.
                sb.AppendFormat("<div><img class='photo' src='{0}' alt='' onerror=\"this.style.display='none';this.parentNode.className='photo-empty';\"></div>", PdfText(photo));

            int age = DateTime.UtcNow.Year - dataCV.birth_date.Year;
            sb.AppendFormat("</td><td><h1 class='text'>{0}</h1><p class='text'>{1}</p>",
                PdfText(dataCV.employee_name), PdfText(dataCV.position));
            AppendPdfField(sb, "Age", age + " years");
            AppendPdfField(sb, "Phone", dataCV.phone);
            AppendPdfField(sb, "Email", dataCV.email);
            AppendPdfField(sb, "Experience", dataCV.total_exp + " years");
            AppendPdfField(sb, "Location", dataCV.address);
            sb.Append("</td></tr></table><h2>EXPERIENCE</h2>");
            if (dataCV.Experience_List != null)
            {
                foreach (var detail in dataCV.Experience_List)
                {
                    sb.AppendFormat(@"<div class='entry'><h3 class='text'>{0}-{1}<br>{2}<br>{3}</h3>
<p class='text'>{4}</p>", detail.periode_start, detail.periode_end,
                        PdfText(detail.company), PdfText(detail.role), PdfText(detail.resposibility_desc));
                    AppendPdfField(sb, "Tools", detail.tech_tools);
                    sb.Append("</div>");
                }
            }
            sb.Append("<h2>SKILLS</h2>");
            AppendPdfSkills(sb, "Hard Skill", dataCV.hard_skill);
            AppendPdfSkills(sb, "Soft Skill", dataCV.soft_skill);
            sb.Append("<div class='education'><h2>EDUCATION</h2>");
            AppendPdfField(sb, "University", dataCV.education_name);
            AppendPdfField(sb, "Graduation year", dataCV.year_education);
            AppendPdfField(sb, "Degree", dataCV.education_type);
            AppendPdfField(sb, "Major", dataCV.focus_education);
            AppendPdfField(sb, "GPA (IPK)", dataCV.ipk);
            sb.Append("</div></body></html>");
            return sb.ToString();
        }

        private static string PdfText(object? value) =>
            System.Net.WebUtility.HtmlEncode(Convert.ToString(value, CultureInfo.InvariantCulture) ?? "");

        private static void AppendPdfField(StringBuilder sb, string label, object? value)
        {
            sb.AppendFormat("<div class='field'><span class='label'>{0}: </span><span class='text'>{1}</span></div>",
                PdfText(label), PdfText(value));
        }

        private static void AppendPdfSkills(StringBuilder sb, string label, string? skills)
        {
            sb.AppendFormat("<h3>{0}</h3><ul>", PdfText(label));
            foreach (var skill in (skills ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(skill))
                    sb.AppendFormat("<li class='text'>{0}</li>", PdfText(skill.Trim()));
            }
            sb.Append("</ul>");
        }

        private static string? GetPdfPhotoUri(string? imagePath, string hostingPath)
        {
            // Edit uses the saved URL verbatim. Do not map a broken URL's basename
            // to a different file in /Image, or substitute a theme/default avatar.
            if (string.IsNullOrWhiteSpace(imagePath) ||
                !imagePath.StartsWith("/Image/", StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                var imageRoot = Path.GetFullPath(Path.Combine(hostingPath, "Image")) + Path.DirectorySeparatorChar;
                var relativePath = Uri.UnescapeDataString(imagePath.Substring("/Image/".Length));
                var fullPath = Path.GetFullPath(Path.Combine(imageRoot, relativePath));
                if (!fullPath.StartsWith(imageRoot, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
                    return null;
                return new Uri(fullPath).AbsoluteUri;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is NotSupportedException)
            {
                return null;
            }
        }
    }
}