using Common.DataModel;
using Feed_Service.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Feed_Service.Presenters
{
    [ApiController]
    [Route("api/feed")]
    public class FeedPresenter : ControllerBase
    {
        private readonly IFeedLogic _feedLogic;

        public FeedPresenter(IFeedLogic postLogic)
        {
            _feedLogic = postLogic;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetPosts(int pageSize, DateTime? latestCreatedDate = null)
        {
            List<PostModel> posts = await _feedLogic.GetPostFeed(pageSize, latestCreatedDate);

            return Ok(posts);
        }
    }
}
