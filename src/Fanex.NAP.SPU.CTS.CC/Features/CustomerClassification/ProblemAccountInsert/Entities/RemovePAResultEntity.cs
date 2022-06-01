namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System.Collections.Generic;

    public class RemovePAResultEntity
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }

        public IEnumerable<CustInfoEntity> CustomerInfo { get; set; }
    }
}