using HCP.Repository.Entities;

namespace HCP.Repository.Repository
{
    public interface IRequestRepository
    {
        Task<List<CleaningService>?> GetCleaningServices(string? status);
    }
}