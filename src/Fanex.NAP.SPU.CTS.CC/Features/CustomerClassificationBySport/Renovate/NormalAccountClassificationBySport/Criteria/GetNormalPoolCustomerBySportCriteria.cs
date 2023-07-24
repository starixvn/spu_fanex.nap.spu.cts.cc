namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNormalPoolCustomerBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_NormalPool_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
