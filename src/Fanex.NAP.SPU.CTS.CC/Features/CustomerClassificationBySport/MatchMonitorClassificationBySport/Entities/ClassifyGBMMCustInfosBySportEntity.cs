namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    public class ClassifyGBMMCustInfosBySportEntity
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int MatchID { get; set; }

        public int SportGroup { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }
    }
}
