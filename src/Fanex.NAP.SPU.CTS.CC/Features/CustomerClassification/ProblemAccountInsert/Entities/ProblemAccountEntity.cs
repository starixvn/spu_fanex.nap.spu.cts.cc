namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;

    public class ProblemAccountEntity
    {
        public Int64 CustID { get; set; }

        public int CategoryID { get; set; }

        public Int64? CreatedBy { get; set; }
    }
}