namespace CVSalis.Data.Dto
{
    public class CreateCV
    {
        public bool isCreated { get; set; }
        public long employee_no { get; set; }
        public string? employee_name { get; set; }
        public int phone { get; set; }
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
        public string created_by { get; set; }
        public DateTime created_at { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class RespSaveData
    {
        public string messageUI { get; set; }
        public string messageConsole { get; set; }
        public bool is_ok { get; set; }
    }
}
