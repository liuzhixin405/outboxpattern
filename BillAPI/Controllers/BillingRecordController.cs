using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BillAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingRecordController : ControllerBase
    {
        private readonly BillingDbContext _billingDbContext;
        public BillingRecordController(BillingDbContext billingDbContext)
        {
            _billingDbContext = billingDbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<BillingRecord>> GetListAsync()
        {
            return await _billingDbContext.BillingRecords.ToListAsync();
        }
    }
}
