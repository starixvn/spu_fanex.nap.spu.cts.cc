namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    public class CustInfoEntity
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int CategoryType { get; set; } // 1: PA, 2: PA Dangerous
    }
}