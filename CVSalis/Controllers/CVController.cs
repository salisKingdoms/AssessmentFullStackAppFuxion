using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using CVSalis.Data.Dto;
using CVSalis.Data.Repo;

namespace CVSalis.Controllers
{
    public class CVController : Controller
    {
        private readonly ICVRepo _cvRepo;

        public CVController(ICVRepo cVRepo)
        {
            _cvRepo = cVRepo;
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

                //save CV
                param.isCreated = true;
                var save = _cvRepo.CreateOrUpdateCV(param);
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
    }
}
//tes