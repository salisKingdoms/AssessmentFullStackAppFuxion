using CVSalis.Models;

namespace CVSalis.Data.Dto
{
    public class RespCVDetail
    {
        public DetailCV dataDetail { get; set; }
        public List<ms_employee> listCV { get; set; }
        public bool is_ok { get; set; }
        public string message { get; set; }
    }

    public class RespDeleteCV
    {
        public bool is_ok { get; set; }
        public string message { get; set; }
    }
}
