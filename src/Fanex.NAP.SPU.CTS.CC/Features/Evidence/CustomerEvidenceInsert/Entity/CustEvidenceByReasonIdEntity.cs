namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    public class CustEvidenceByReasonIdEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int EvidenceID { get; set; }

        public string EvidenceCode { get; set; }

        public string Remark { get; set; }
    }
}