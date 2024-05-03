using CVSalis.Data.Dto;
using CVSalis.Models;
using CVSalis.Config;
using System.Linq;
using Dapper;
using System;
using System.Collections.Generic;
using System.IO;


namespace CVSalis.Data.Repo
{
    public interface ICVRepo
    {
        public Task CreateOrUpdateCV(DetailCV request);
        public Task<DetailCV> GetDetailCVById(long id);
        public Task DeleteCV(long id);
        public Task<List<ms_employee>> GetListCV();
    }
}
