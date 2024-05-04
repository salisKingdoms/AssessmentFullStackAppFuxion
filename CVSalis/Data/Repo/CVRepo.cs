using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using CVSalis.Config;
using CVSalis.Models;
using CVSalis.Data.Dto;
using AutoMapper;
using System.Data;

namespace CVSalis.Data.Repo
{
    public class CVRepo : ICVRepo
    {
        private DataContext _context;
        private readonly IMapper _mapper;

        public CVRepo(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateOrUpdateCV(DetailCV request)
        {
            using var conn = _context.CreateConnection();
            try
            {
                string query = String.Empty;

                query = "INSERT INTO ms_employee" +
                            "(employee_name,phone,email,birth_date,address,ktp,image,soft_skill,hard_skill,gender,marital_status,expectation_sallary,education_type,education_name," +
                            "ipk,year_education,total_exp,npwp,position,focus_education,is_negotiable,is_deleted,created_by,created_at,updated_by,updated_at)" +
                            "VALUES" +
                            "(@employee_name,@phone,@email,@birth_date,@address,@ktp,@image,@soft_skill,@hard_skill,@gender,@marital_status,@expectation_sallary,@education_type,@education_name," +
                            "@ipk,@year_education,@total_exp,@npwp,@position,@focus_education,@is_negotiable,@is_deleted,@created_by,@created_at,@updated_by,@updated_at)";

                if (!request.isCreated)
                {
                    query = @"UPDATE ms_employee
                            SET employee_name=@employee_name,
                            phone=@phone,
                            email=@email,
                            birth_date=@birth_date,
                            address=@address,
                            ktp=@ktp,
                            image=@image,
                            soft_skill=@soft_skill,
                            hard_skill=@hard_skill,
                            gender=@gender,
                            marital_status=@marital_status,
                            expectation_sallary=@expectation_sallary,
                            education_type=@education_type,
                            education_name=@education_name,
                            ipk=@ipk,
                            year_education=@year_education,
                            total_exp=@total_exp,
                            npwp=@npwp,
                            position=@position,
                            focus_education=@focus_education,
                            is_negotiable=@is_negotiable,
                            updated_by=@updated_by,
                            updated_at=@updated_at
                            WHERE employee_no=@employee_no;";
                }

                var param = new Dictionary<string, object>
            {
                {"employee_name", request.employee_name ?? "" },
                {"phone", request.phone },
                {"email", request.email },
                {"birth_date", request.birth_date },
                {"address", request.address },
                {"ktp",request.ktp },
                {"image",request.image },
                {"soft_skill", request.soft_skill },
                {"hard_skill",request.hard_skill },
                {"gender",request.gender },
                {"marital_status",request.marital_status },
                {"expectation_sallary", request.expectation_sallary },
                {"education_type", request.education_type },
                {"education_name", request.education_name },
                {"ipk", request.ipk },
                {"year_education", request.year_education },
                {"total_exp",request.total_exp },
                {"npwp",request.npwp },
                {"position", request.position },
                {"focus_education",request.focus_education },
                {"is_negotiable",request.is_negotiable },
                {"is_deleted",request.is_deleted },
                {"created_by","systemUI" },
                {"created_at",DateTime.UtcNow },
                {"updated_by",!request.isCreated ? "systemUI" : "" },
                {"updated_at",!request.isCreated ? DateTime.UtcNow : null }
            };

                if (!request.isCreated)
                {
                    param.Add("employee_no", request.employee_no);
                }
                //save
                var saveCV = await conn.ExecuteAsync(query, param);
                var getLasID = GetLastIDCV();
                if (getLasID.Result.employee_no > 0)
                {
                    //submit experience
                    foreach (var req in request.Experience_List)
                    {
                        var reqExp = new CreateExperience
                        {
                            isCreated = true,
                            employee_id = req.employee_id,
                            company = req.company,
                            role = req.role,
                            periode_start = req.periode_start,
                            periode_end = req.periode_end,
                            resposibility_desc = req.resposibility_desc,
                            company_address = req.company_address,
                            tech_tools = req.tech_tools,
                            created_by = "systemUI",
                            created_at = DateTime.UtcNow,

                        };
                        var submitExp = CreateOrUpdateExperience(reqExp);
                    }

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        public async Task<GetIDCV> GetLastIDCV()
        {
            GetIDCV result = new GetIDCV();
            using var conn = _context.CreateConnection();
            try
            {
                string query = "SELECT employee_no FROM ms_employee " +
                               "ORDER BY created_at DESC limit 1 offset 0";


                var dataDB = await conn.QueryAsync<GetIDCV>(query, null);
                if (dataDB != null)
                {
                    result = dataDB.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                result = null;
            }
            return result;

        }
        public async Task<DetailCV> GetDetailCVById(long id)
        {
            DetailCV dataCV = new DetailCV();
            using var conn = _context.CreateConnection();
            try
            {
                List<GetDataExperience> detailExp = new List<GetDataExperience>();
                string query = "SELECT * FROM ms_employee a" +
                               "LEFT JOIN experience_employee b on a.employee_no = b.employee_id" +
                               "WHERE a.employee_no =@id";

                var param = new Dictionary<string, object>
            {
                { "id", id  },

            };
                var dataDB = await conn.QueryAsync<DetailCVFromQuery>(query, param);
                if (dataDB != null && dataDB.Count() > 0)
                {
                    foreach (var exp in dataDB)
                    {
                        detailExp.Add(new GetDataExperience
                        {
                            id = exp.id,
                            employee_id = exp.employee_id,
                            company = exp.company,
                            role = exp.role,
                            periode_start = exp.periode_start,
                            periode_end = exp.periode_end,
                            resposibility_desc = exp.resposibility_desc,
                            company_address = exp.company_address,
                            tech_tools = exp.tech_tools
                        });

                        dataCV.employee_no = exp.employee_no;
                        dataCV.employee_name = exp.employee_name;
                        dataCV.address = exp.address;
                        dataCV.email = exp.email;
                        dataCV.position = exp.position;
                        dataCV.phone = exp.phone;
                        dataCV.ktp = exp.ktp;
                        dataCV.npwp = exp.npwp;
                        dataCV.birth_date = exp.birth_date;
                        dataCV.gender = exp.gender;
                        dataCV.marital_status = exp.marital_status;
                        dataCV.total_exp = exp.total_exp;
                        dataCV.education_type = exp.education_type;
                        dataCV.education_name = exp.education_name;
                        dataCV.focus_education = exp.focus_education;
                        dataCV.year_education = exp.year_education;
                        dataCV.ipk = exp.ipk;
                        dataCV.expectation_sallary = exp.expectation_sallary;
                        dataCV.is_negotiable = exp.is_negotiable;
                        dataCV.hard_skill = exp.hard_skill;
                        dataCV.soft_skill = exp.soft_skill;
                        dataCV.image = exp.image;
                        dataCV.Experience_List = detailExp;
                        dataCV.is_deleted = exp.is_deleted;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }

            return dataCV;
        }

        private async Task CreateOrUpdateExperience(CreateExperience request)
        {
            using var conn = _context.CreateConnection();
            try
            {
                string query = String.Empty;

                query = "INSERT INTO experience_employee" +
                            "(employee_id,company,role,periode_start,periode_end,resposibility_desc,company_address,tech_tools,created_by,created_at,updated_by,updated_at)" +
                            "VALUES" +
                            "(@employee_id,@company,@role,@periode_start,@periode_end,@resposibility_desc,@company_address,@tech_tools,@created_by,@created_at,@updated_by,@updated_at)";

                if (!request.isCreated)
                {
                    query = @"UPDATE ms_employee
                            SET employee_id=@employee_id,
                            company=@company,
                            role=@role,
                            periode_start=@periode_start,
                            periode_end=@periode_end,
                            resposibility_desc=@resposibility_desc,
                            company_address=@company_address,
                            tech_tools=@tech_tools,
                            updated_by=@updated_by,
                            updated_at=@updated_at
                            WHERE id=@id;";
                }

                var param = new Dictionary<string, object>
            {
                {"employee_id", request.employee_id },
                {"company", request.company },
                {"role", request.role },
                {"periode_start", request.periode_start },
                {"periode_end",request.periode_end },
                {"resposibility_desc",request.resposibility_desc },
                {"company_address", request.company_address },
                {"tech_tools",request.tech_tools },
                {"created_by",request.created_by },
                {"created_at",request.created_at },
                {"updated_by",request.updated_by },
                {"updated_at",request.updated_at }
            };

                if (!request.isCreated)
                {
                    param.Add("id", request.id);
                }

                await conn.ExecuteAsync(query, param);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        public async Task DeleteCV(long id)
        {
            using var connection = _context.CreateConnection();
            var sql = @" UPDATE ms_employee 
            SET is_deleted = true,
                updated_by = @updated_by,
                updated_at = @updated_at
            WHERE employee_no = @id";
            var param = new Dictionary<string, object>
            {
                { "id", id  },
                { "updated_by", "systemUI"  },
                { "updated_at", DateTime.UtcNow }

            };
            await connection.ExecuteAsync(sql, param);
        }

        private async Task<IEnumerable<ms_employee>> RepoGetAllCV()
        {
            using var connection = _context.CreateConnection();
            var sql = QueryListCV();
            string sqlALL = sql + "limit @limit offset @offset";
            var param = new Dictionary<string, object>
            {
                { "limit",20},
                { "offset",0}
            };
            return await connection.QueryAsync<ms_employee>(sql, param);
        }

        private string QueryListCV()
        {
            var query = "SELECT * FROM ms_employee";
            query += " where is_deleted=false or  is_deleted IS NULL";
            return query;
        }

        public async Task<List<ms_employee>> GetListCV()
        {
            var data = RepoGetAllCV().Result.ToList();
            return data;
        }
    }
}
