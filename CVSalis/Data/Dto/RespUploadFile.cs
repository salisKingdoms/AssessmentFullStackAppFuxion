using CVSalis.Config;

namespace CVSalis.Data.Dto
{
    public class RespUploadFile
    {
        public string message { get; set; }
        public string filePath { get; set; }
    }

    public class RespFileModel
    {
        public bool is_ok { get; set; }
        public string message { get; set; }
        public FileModel data { get; set; }
        public byte[] databyte { get; set; }

    }
}
