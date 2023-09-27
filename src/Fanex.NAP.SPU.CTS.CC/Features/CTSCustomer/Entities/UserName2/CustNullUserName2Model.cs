namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Xml.Serialization;

    [XName("Customer")]
    public class CustNullUserName2Model
    {
        [XName("CTSCustID")]
        public UInt64 CTSCustID { get; set; }

        [XName("CustID")]
        public Int64 CustID { get; set; }

        [XName("CustSubID")]
        public Int64 CustSubID { get; set; }

        [XmlIgnore]
        public string UserName2 { get; set; }
    }
}