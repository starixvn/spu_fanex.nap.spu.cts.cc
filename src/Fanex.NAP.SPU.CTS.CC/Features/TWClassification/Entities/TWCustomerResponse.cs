namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using System;

    public class TWCustomerResponse
    {
        public long ID { get; set; }

        public string Username { get; set; }

        public int CustomerClass { get; set; }

        public long CustID { get; set; }

        public int ProcessStatus { get; set; }

        public DateTime InsertedTime { get; set; }

        public DateTime? ResponseTime { get; set; }

        public int? ResponseHttpCode { get; set; }
    }
}
