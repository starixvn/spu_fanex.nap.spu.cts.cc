namespace Fanex.NAP.SPU.CTS.CC.JobService.Controllers
{
    using Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/TransformCustomer")]
    public class CustomerJobController : ControllerBase
    {
        private readonly ICTSCustomerJobService _ctsCustomerJobService;
        private readonly ICTSNewCustomerJobService _insertNewCustomerJobService;
        private readonly ICTSLatencyCustomerJobService _latencyCustomerJobService;
        private readonly ICTSReactivatedCustomerJobService _reactivatedCustomerJobService;

        public CustomerJobController(
            ICTSCustomerJobService ctsCustomerJobService,
            ICTSNewCustomerJobService insertNewCustomerJobService,
            ICTSLatencyCustomerJobService latencyCustomerJobService,
            ICTSReactivatedCustomerJobService reactivatedCustomerJobService)
        {
            _ctsCustomerJobService = ctsCustomerJobService;
            _insertNewCustomerJobService = insertNewCustomerJobService;
            _latencyCustomerJobService = latencyCustomerJobService;
            _reactivatedCustomerJobService = reactivatedCustomerJobService;
        }

        [HttpPost("InsertNewCustomer")]
        public async Task<bool> InsertNewCustomer([FromForm] int batchSize)
        {
            return await _insertNewCustomerJobService.InsertNewCustomer(batchSize);
        }

        [HttpPost("ScanLatencyCustomer")]
        public async Task<bool> ScanLatencyCustomer([FromForm] int externalBatchSize, [FromForm] int internalBatchSize, [FromForm] int scannedSecondTime)
        {
            return await _latencyCustomerJobService.ScanLatencyCustomer(externalBatchSize, internalBatchSize, scannedSecondTime);
        }

        [HttpPost("InsertLatencyCustomer")]
        public async Task<bool> InsertLatencyCustomer([FromForm] int batchSize)
        {
            return await _latencyCustomerJobService.InsertLatencyCustomer(batchSize);
        }

        [HttpPost("InsertMissingCustomer")]
        public async Task<bool> InsertMissingCustomer([FromForm] int batchSize)
        {
            return await _ctsCustomerJobService.InsertMissingCustomer(batchSize);
        }

        [HttpPost("UpdateCTSCustomerInfo")]
        public async Task<bool> UpdateCTSCustomerInfo([FromForm] int batchSize, [FromForm] int waitTime)
        {
            return await _ctsCustomerJobService.UpdateCTSCustomerInfo(batchSize, waitTime);
        }

        [HttpPost("UpdateUsername2ForCustMain")]
        public async Task<bool> UpdateUsername2ForCustMain([FromForm] int batchSize)
        {
            return await _ctsCustomerJobService.UpdateUsername2ForCustMain(batchSize);
        }

        [HttpPost("UpdateUsername2ForCustSub")]
        public async Task<bool> UpdateUsername2ForCustSub([FromForm] int batchSize)
        {
            return await _ctsCustomerJobService.UpdateUsername2ForCustSub(batchSize);
        }

        [HttpPost("ScanReactivatedLatencyCustomer")]
        public async Task<bool> ScanReactivatedLatencyCustomer([FromForm] int batchSize, [FromForm] int scannedSecondTime)
        {
            return await _reactivatedCustomerJobService.ScanReactivatedLatencyCustomers(batchSize, scannedSecondTime);
        }

        [HttpPost("InsertReactivatedCustomer")]
        public async Task<bool> InsertReactivatedCustomer()
        {
            return await _ctsCustomerJobService.InsertReactivatedCustomers();
        }

        [HttpPost("InactivateNewCustomers")]
        public async Task<bool> InactivateNewCustomers([FromForm] int batchSize)
        {
            return await _ctsCustomerJobService.InactivateNewCustomers(batchSize);
        }
    }
}