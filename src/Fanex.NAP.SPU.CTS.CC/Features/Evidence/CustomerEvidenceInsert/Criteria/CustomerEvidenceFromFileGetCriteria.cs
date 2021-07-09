namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data.Repository;

    public class CustomerEvidenceFromFileGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceFromFile_Get";

        public override bool IsValid()
            => true;
    }
}