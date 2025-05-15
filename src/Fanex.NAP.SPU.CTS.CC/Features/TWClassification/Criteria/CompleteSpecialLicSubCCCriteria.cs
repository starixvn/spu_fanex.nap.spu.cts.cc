namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Criteria
{
    using Data.Repository;

    public class CompleteSpecialLicSubCCCriteria : NonQueryCommand
    {
        public long ID { get; set; }

        public int ProcessStatus { get; set; }

        public int ResponseHttpCode { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Response_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
