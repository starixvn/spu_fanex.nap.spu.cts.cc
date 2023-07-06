namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{  
    public class ProbationScanClassificationBySportEntity
    {
        public long CustID { get; set; }

        public int SportID { get; set; }

        // 0-Up to Good & Normal OR 1-Up to Smart, High Risk, VIP & Normal        
        public int IsProbationLastDay { get; set; }
    }
}
