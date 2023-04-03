namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    public class TVSProblemAccountEntity
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        public long? CreatedBy { get; set; }

        public int? RequestID { get; set; }

        public bool? IsParlay { get; set; }

        public int? Sporttype { get; set; }

        public int IssueTypeId { get; set; }
    }
}