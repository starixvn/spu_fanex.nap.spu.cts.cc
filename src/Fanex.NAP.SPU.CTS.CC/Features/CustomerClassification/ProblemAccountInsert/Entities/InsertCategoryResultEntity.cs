namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using System.Collections.Generic;

    public class InsertCategoryResultEntity
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }


        public IEnumerable<long> CustIds { get; set; }
    }
}