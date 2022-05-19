namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System.Collections.Generic;

    public class InsertPAResultModel
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }

        public IEnumerable<AssCustomerForCheckingRollbackEntity> customerInfo { get; set; }
    }
}