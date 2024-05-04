namespace CVSalis.Data.Dto
{
    public class DetailCV
    {
        public bool isCreated { get; set; }
        public long employee_no { get; set; }
        public string? employee_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime birth_date { get; set; }
        public string address { get; set; }
        public string ktp { get; set; }
        public string image { get; set; }
        public string soft_skill { get; set; }
        public string hard_skill { get; set; }
        public int gender { get; set; }
        public int marital_status { get; set; }
        public decimal expectation_sallary { get; set; }
        public string education_type { get; set; }
        public string education_name { get; set; }
        public decimal ipk { get; set; }
        public int year_education { get; set; }
        public int total_exp { get; set; }
        public string npwp { get; set; }
        public string position { get; set; }
        public string focus_education { get; set; }
        public bool is_negotiable { get; set; }
        public bool is_deleted { get; set; }
        public List<GetDataExperience> Experience_List { get; set; }
    }

    public class DetailCVFromQuery
    {
        public bool isCreated { get; set; }
        public long employee_no { get; set; }
        public string? employee_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime birth_date { get; set; }
        public string address { get; set; }
        public string ktp { get; set; }
        public string image { get; set; }
        public string soft_skill { get; set; }
        public string hard_skill { get; set; }
        public int gender { get; set; }
        public int marital_status { get; set; }
        public decimal expectation_sallary { get; set; }
        public string education_type { get; set; }
        public string education_name { get; set; }
        public decimal ipk { get; set; }
        public int year_education { get; set; }
        public int total_exp { get; set; }
        public string npwp { get; set; }
        public string position { get; set; }
        public string focus_education { get; set; }
        public bool is_negotiable { get; set; }
        public long? id { get; set; }
        public long? employee_id { get; set; }
        public string? company { get; set; }
        public string? role { get; set; }
        public int? periode_start { get; set; }
        public int? periode_end { get; set; }
        public string? resposibility_desc { get; set; }
        public string? company_address { get; set; }
        public string? tech_tools { get; set; }
        public bool is_deleted { get; set; }
    }

    public class GetDataExperience
    {
        public long id { get; set; }
        public long employee_id { get; set; }
        public string company { get; set; }
        public string role { get; set; }
        public int periode_start { get; set; }
        public int periode_end { get; set; }
        public string resposibility_desc { get; set; }
        public string company_address { get; set; }
        public string tech_tools { get; set; }
    }

    public class GetIDCV
    {
        public long employee_no { get; set; }
    }
}
