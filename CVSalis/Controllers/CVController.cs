using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using CVSalis.Data.Dto;
using CVSalis.Data.Repo;
using Microsoft.AspNetCore.Hosting;

namespace CVSalis.Controllers
{
    public class CVController : Controller
    {
        private readonly ICVRepo _cvRepo;
        private IWebHostEnvironment webHostEnvironment;
        string localPath = "\\Image\\";
        public CVController(ICVRepo cVRepo, IWebHostEnvironment environment)
        {
            _cvRepo = cVRepo;
            webHostEnvironment = environment;
        }
        public IActionResult CurriculumVitae()
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
                if(param.Experience_List != null && param.Experience_List.Count > 0)
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

                //save CV
                param.isCreated = false;
                var save = _cvRepo.CreateOrUpdateCV(param);
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
    }
}
//tes