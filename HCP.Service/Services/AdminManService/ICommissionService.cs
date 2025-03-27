using HCP.Repository.Entities;

public interface ICommissionService
{
    Task<Commissions> CreateCommissionAsync(Commissions commission);
    Task<Commissions> GetCommissionByIdAsync();
    Task<Commissions> UpdateCommissionAsync(Guid id, Commissions commission);
}