using Common.DataModel;

namespace Feed_Service.Logic
{
    public interface IFeedLogic
    {
        Task<List<PostModel>> GetPostFeed(int pageSize, DateTime? latestCreatedDate = null);
    }
}