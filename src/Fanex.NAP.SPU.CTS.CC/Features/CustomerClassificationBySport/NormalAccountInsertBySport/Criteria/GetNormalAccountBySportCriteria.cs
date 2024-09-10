namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNormalAccountBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_NormalAccount_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}