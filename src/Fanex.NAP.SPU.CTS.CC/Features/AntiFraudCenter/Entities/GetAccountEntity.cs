namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using System.Collections.Generic;

    public class GetAccountEntity
    {
        //B2CustId
        public string CustId { get; set; }

        public string CountryCode { get; set; }

        public string PhoneNo { get; set; }

        public string Email { get; set; }

        public string AccountNum { get; set; }

        public string AccountName { get; set; }

        public IEnumerable<int> FraudID { get; set; }

        public string FullName { get; set; }
    }
}