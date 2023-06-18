using System.Reflection;

namespace Content_Managment_Service.DTO_s
{
    public class PostDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public string Author { get; set; }
    }
}
