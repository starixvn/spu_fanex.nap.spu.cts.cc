namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using System;

    public class GetCustWithFirstBetDateParam
    {      
        public DateTime FromDate { get; set; }
      
        public DateTime ToDate { get; set; }
      
        public bool IsLicensee { get; set; }
      
        public int LastCustID { get; set; }
    
        public int BatchSize { get; set; }
    }
}
