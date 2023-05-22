using Content_Managment_Service.DTO_s;
using Content_Managment_Service.Entity;
using Content_Managment_Service.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Content_Managment_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostLogic _postLogic;

        public PostController(IPostLogic postLogic)
        {
            _postLogic = postLogic;
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] PostDTO postDto)
        {
            // Map the PostDTO to a PostEntity
            var postEntity = new PostEntity
            {
                Title = postDto.Title,
                Description = postDto.Description,
                ImagePath = postDto.ImagePath,
                Author = postDto.Author
            };

            // Pass the postEntity to the CreatePostLogic method in IPostLogic
            _postLogic.CreatePostLogic(postEntity);

            // Return a 200 OK response
            return Ok();
        }
    }
}
