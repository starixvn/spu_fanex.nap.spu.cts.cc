namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Collections.Generic;

    public class SynctEvidenceCategoryResultEntity
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public IEnumerable<CheckPACustomerAssociationForRollbackEntity> CustInfo { get; set; }
    }
}