using Content_Managment_Service.DTO_s;
using Content_Managment_Service.Entity;
using Content_Managment_Service.Logic;
using Microsoft.AspNetCore.Mvc;
using System.Web;

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
            // Validate inputs
            if (string.IsNullOrWhiteSpace(postDto.Description) && string.IsNullOrWhiteSpace(postDto.ImagePath))
            {
                return BadRequest("At least one of 'Description' or 'ImagePath' must be provided.");
            }

            // Validate ImagePath format
            if (!string.IsNullOrWhiteSpace(postDto.ImagePath) && !Uri.IsWellFormedUriString(postDto.ImagePath, UriKind.Absolute))
            {
                return BadRequest("Invalid 'ImagePath' format.");
            }

            // Map the PostDTO to a PostEntity, Escape special Characters and trim
            var postEntity = new PostEntity
            {
                Title = HttpUtility.HtmlEncode(postDto.Title.Trim()),
                Description = HttpUtility.HtmlEncode(postDto.Description.Trim()),
                ImagePath = HttpUtility.HtmlEncode(postDto.ImagePath.Trim()),
                Author = HttpUtility.HtmlEncode(postDto.Author.Trim())
            };

            // Pass the postEntity to the CreatePostLogic method in IPostLogic
            _postLogic.CreatePostLogic(postEntity);

            // Return a 200 OK response
            return Ok();
        }
    }
}
