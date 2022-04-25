namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Collections.Generic;

    public class InsertCategoryResultEntity
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }

        public IEnumerable<long> CustIds { get; set; }

        public IEnumerable<CheckPACustomerAssociationForRollbackEntity> NewPAAssociationCustomerForRollback { get; set; }
    }
}