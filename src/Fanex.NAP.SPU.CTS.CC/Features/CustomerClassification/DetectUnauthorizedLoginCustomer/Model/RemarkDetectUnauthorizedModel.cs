namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using System;
   
    public class RemarkDetectUnauthorizedModel
    {
        public RemarkDetectUnauthorizedModel() { }

        public RemarkDetectUnauthorizedModel(DetectUnauthorizedLoginCustEntity cust) 
        {
            this.FirstBetDate = cust.FirstBetDate;
            this.DetectionDate = cust.DetectionDate;
            this.Bot = cust.Bot;
            this.InvalidBrowser = cust.InvalidBrowser;
            this.BotPCT = cust.BotPCT;
            this.InvalidBrowserPCT = cust.InvalidBrowserPCT;
        }

        public DateTime FirstBetDate { get; set; }

        public DateTime DetectionDate { get; set; }

        public int Bot { get; set; }

        public int InvalidBrowser { get; set; }

        public double BotPCT { get; set; }

        public double InvalidBrowserPCT { get; set; }
    }
}
