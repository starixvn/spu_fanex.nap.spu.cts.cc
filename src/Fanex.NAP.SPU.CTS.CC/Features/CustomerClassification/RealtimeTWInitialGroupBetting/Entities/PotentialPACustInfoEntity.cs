namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting
{
    public class PotentialPACustInfoEntity
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int? CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsLicensee { get; set; }
    }
}
