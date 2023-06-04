using Common.DataModel;

namespace Feed_Service.Repositories
{
    public interface IFeedRepository
    {
        Task<List<PostModel>> GetPage(int pageSize, DateTime? latestCreatedDate = null);
    }
}