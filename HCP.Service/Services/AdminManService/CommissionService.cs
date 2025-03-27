using HCP.Repository.Entities;
using HCP.Repository.Interfaces;

public class CommissionService : ICommissionService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommissionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<Commissions> GetCommissionByIdAsync()
    {
        return _unitOfWork.Repository<Commissions>().GetAll().FirstOrDefault();
    }

    public async Task<Commissions> CreateCommissionAsync(Commissions commission)
    {
        await _unitOfWork.Repository<Commissions>().AddAsync(commission);
        await _unitOfWork.SaveChangesAsync();
        return commission;
    }

    public async Task<Commissions> UpdateCommissionAsync(Guid id, Commissions commission)
    {
        var existingCommission = await _unitOfWork.Repository<Commissions>().FindAsync(c => c.Id == id);
        if (existingCommission == null)
        {
            throw new Exception("Commission not found");
        }

        existingCommission.CommisionRate = commission.CommisionRate;
        _unitOfWork.Repository<Commissions>().Update(existingCommission);
        await _unitOfWork.SaveChangesAsync();
        return existingCommission;
    }

}
