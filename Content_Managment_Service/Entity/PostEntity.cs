namespace Content_Managment_Service.Entity
{
    public class PostEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set;}
    }
}
