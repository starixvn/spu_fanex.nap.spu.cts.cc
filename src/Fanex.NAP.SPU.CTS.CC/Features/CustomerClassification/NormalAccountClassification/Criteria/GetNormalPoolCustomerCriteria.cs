namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNormalPoolCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_NormalPool_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
