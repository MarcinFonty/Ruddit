using MediatR;

namespace Content_Managment_Service.Commands
{
    public class CreatePostCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
