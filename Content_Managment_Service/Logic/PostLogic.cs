using Content_Managment_Service.Commands;
using Content_Managment_Service.Entity;
using MediatR;

namespace Content_Managment_Service.Logic
{
    public class PostLogic : IPostLogic
    {
        private readonly IMediator _mediator;
        public PostLogic(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void CreatePostLogic(PostEntity post)
        {
            post.Id = Guid.NewGuid();
            post.CreatedDate = DateTime.Now;

            var command = new CreatePostCommand 
            { 
                Id = post.Id,
                CreatedDate = DateTime.Now,
                Author = post.Author,
                ImagePath = post.ImagePath,
                Description = post.Description,
                Title = post.Title,
            };

            _mediator.Send(command);
        }
    }
}
