using HCP.Repository.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CommissionController : ControllerBase
{
    private readonly ICommissionService _commissionService;

    public CommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }


    [HttpGet()]
    public async Task<IActionResult> GetById()
    {
        var commission = await _commissionService.GetCommissionByIdAsync();
        if (commission == null) return NotFound();
        return Ok(commission);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Commissions commission)
    {
        var newCommission = await _commissionService.CreateCommissionAsync(commission);
        return CreatedAtAction(nameof(GetById), new { id = newCommission.Id }, newCommission);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Commissions commission)
    {
        var updatedCommission = await _commissionService.UpdateCommissionAsync(id, commission);
        return Ok(updatedCommission);
    }
}
