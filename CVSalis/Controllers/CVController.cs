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
            string[] parts = dataCV.image.Split('/');
            string imageName = parts[parts.Length - 1];
            int yearDate = dataCV.birth_date.Year;
            int now = DateTime.UtcNow.Year;
            dataCV.ages = now - yearDate;
            sb.AppendFormat(@"
            <html>
                <head>
                    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\fonts\\boxicons.css'/>
                    <link rel='stylesheet' href='{0}\\css\\sb-admin-2.css' />
                    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\css\\core.css' class='template-customizer-core-css'/>
                    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\css\\theme-default.css' class='template-customizer-theme-css'/>
                    <link rel='stylesheet' href='{0}\\Theme\\assets\\vendor\\css\\demo.css'/>
                    

                    <script src='{0}\\Theme\\assets\\vendor\\js\\helpers.js'/></script>
                    <script  src='{0}\\Theme\\assets\\js\\config.js'/></script>
                    
                </head>
                <body>
                 
                    <div class='card'>
                       <div class='d-flex align-items-end row'>
                             <div class='col-sm-3'>
                                    <div class='card-body pb-3 px-3 px-md-4' style='display:flex;justify-content:center;align-items:center;margin:auto;'>
                                        <img src='{0}\Image\{1}' style='border-radius: 50%;box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);' height='140' width='140' alt='View Badge User' />
                                    </div>
                             </div>
                            <div class='col-sm-9 text-right text-sm-left'>
                                <h5 class='card-title control-label text-primary'><b>{2}</b><br /> {3}</h5>
                            
                            <div class='row'>
                                <div class='col-sm-2 text-sm-left'>
                                        <p>Age<br />{4} years</p>
                                </div>
                                <div class='col-sm-2'>
                                        <p>Phone<br />{5}</p>
                                </div>
                                <div class='col-sm-3'>
                                        <p> Email<br />{6}</p>
                                </div>
                                <div class='col-sm-2'>
                                        <p>Experience<br />{7} years</p>
                                </div>
                                <div class='col-sm-3'>
                                        <p>Location<br />{8}</p>
                                </div>
                            </div>
                       </div>
                    </div>", hostingPath, imageName, dataCV.employee_name, dataCV.position, dataCV.ages, dataCV.phone, dataCV.email, dataCV.total_exp,dataCV.address);

            sb.AppendFormat(@"<div class='col-md-12 row' style='margin-top:20px;padding-left:40px;padding-right:40px;'>
                              <hr class='col-md-12'>
                                <div class='card col-sm-8 '>
                                    <label class='control-label' style='margin-top:8px;margin-bottom:5px;'>
                                    <strong>EXPERIENCE</strong></label>
                                    <hr> ");
            foreach(var detail in dataCV.Experience_List)
            {
                sb.AppendFormat(@"<label class='control-label'>
                    <strong>{0}-{1} <br />{2}<br />{3}</strong>
                </label>
                 <label class='control-label'>
                    {4}
                </label>
                <label class ='control-label'><strong>Tools :</strong><br />
                   {5}
                </label>
                <br /><hr>", detail.periode_start,detail.periode_end,detail.company,detail.role,detail.resposibility_desc,detail.tech_tools);
            }
            sb.AppendFormat(@"</div>");//tutup div utk experience
            //hardskill split with,
            List<string> hardskill = new List<string>();
            hardskill = dataCV.hard_skill.Split(',').ToList();

            //header hardskill
            sb.AppendFormat(@"<div class='card col-sm-4 h-800'>
                                <label class='control-label' style='margin-top:8px;margin-bottom:5px;'>
                                    <strong>SKILLS</strong>
                                </label>
                                <hr>
                                <label class='control-label' style='margin-top:5px;margin-bottom:5px;'>
                                    <strong>Hard Skill :</strong>
                                </label>
                                <label class='control-label'>
                                <strong><ul>");
            for(int i = 0; i<=hardskill.Count-1;i++)
            {
                sb.AppendFormat(@"<li>{0}</li>", hardskill[i]);
            }
            sb.AppendFormat(@"</ul></strong>
                          </label>
                          
                          <label class='control-label' style='margin-top:5px;margin-bottom:5px;'>
                            <strong>Soft Skill :</strong>
                          </label>
                          <label class='control-label'><ul>");

            //softskill split with,
            List<string> softskill = new List<string>();
            softskill = dataCV.soft_skill.Split(',').ToList();

            for(int i=0; i <= softskill.Count-1; i++)
            {
                sb.AppendFormat(@" <li>{0}</li>", softskill[i]);
            }
            //sb.AppendFormat(@"</label>
            //                <br /><br />
            //                <hr>
            //                    <label class='control-label' style='margin-top:8px;margin-bottom:5px;'>
            //                        <strong>EDUCATION</strong>
            //                    </label>
            //                    <label class='control-label'>
            //                        <strong>
            //                           <i class='bx bxs-graduation'></i>&nbsp;{0} <br />
            //                            &nbsp;&nbsp;&nbsp; Graduate on {1} <br />&nbsp;&nbsp; &nbsp;{2} Of {3}
            //                            <br />&nbsp;&nbsp;&nbsp; {4}
            //                        </strong>
            //                    </label>
            //                  </div>
            //                </div>
            //              </div>
            //          </body>
            //       </html>",dataCV.education_name,dataCV.year_education,dataCV.education_type,dataCV.focus_education,dataCV.ipk);
            sb.AppendFormat(@"</ul></label>
                            <br /><br />
                           
                                <label class='control-label' style='margin-top:8px;margin-bottom:5px;'>
                                    <strong>EDUCATION</strong>
                                </label>
                                <label class='control-label'>
                                    <strong>
                                        &nbsp;&nbsp;&nbsp;{0} <br />
                                        &nbsp;&nbsp;&nbsp; Graduate on {1} <br />&nbsp;&nbsp; &nbsp;{2} Of {3}
                                        <br />&nbsp;&nbsp;&nbsp;IPK {4}
                                    </strong>
                                </label>
                              </div>
                            </div>
                          </div>", dataCV.education_name, dataCV.year_education, dataCV.education_type, dataCV.focus_education, dataCV.ipk);


            sb.AppendFormat(@"<script src='{0}\\Theme\\assets\\vendor\\libs\\jquery\\jquery.js'/></script>
<script src='{0}\\Theme\\assets\\vendor\\libs\\popper\\popper.js'/></script>
<script src='{0}\\Theme\\assets\\vendor\\js\\bootstrap.js'/></script>
<script src='{0}\\Theme\\assets\\vendor\\libs\\perfect-scrollbar\\perfect-scrollbar.js'/></script>
<script src='{0}\\Theme\\assets\\vendor\\js\\menu.js'/></script>
<script src='{0}\\Theme\\assets\\js\\main.js'/></script>
</body></html>", hostingPath);
                      
                  



            return sb.ToString();
        }
    }
}
