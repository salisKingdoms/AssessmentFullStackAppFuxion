namespace CVSalis.Models
{
    public class experience_employee
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
        public string created_by { get; set; }
        public DateTime created_at { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_at { get; set; }
    }
}
