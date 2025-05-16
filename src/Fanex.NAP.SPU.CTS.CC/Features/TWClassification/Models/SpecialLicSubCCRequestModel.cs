namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Models
{
    public class SpecialLicSubCCRequestModel
    {
        public string ResponseDateTime { get; set; } 

        public long CustID { get; set; }

        public int SpecialCustomerClass { get; set; }

        public string ErrorMessage { get; set; } 

        public int ErrorCode { get; set; }

        public int HttpStatusCode { get; set; }
    }
}
