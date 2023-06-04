using Common.DataModel;
using Feed_Service.Repositories;

namespace Feed_Service.Logic
{
    public class FeedLogic : IFeedLogic
    {
        private readonly IFeedRepository _feedRepository;
        public FeedLogic(IFeedRepository feedRepository)
        {
            _feedRepository = feedRepository;
        }

        public async Task<List<PostModel>> GetPostFeed(int pageSize, DateTime? latestCreatedDate = null)
        {
            return await _feedRepository.GetPage(pageSize, latestCreatedDate);
        }
    }
}
