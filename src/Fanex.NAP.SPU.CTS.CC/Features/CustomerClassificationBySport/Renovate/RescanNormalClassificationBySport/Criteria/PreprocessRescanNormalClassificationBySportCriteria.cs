namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PreprocessRescanNormalClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string ListCustId { get; set; }

        [SpParam(SqlParamName = "FunctionId")]
        public int FunctionId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_RescanNormal_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}