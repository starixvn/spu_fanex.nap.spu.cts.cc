namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NormalClassificationPoolMonitorGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "GeneralRecord")]
        public long GeneralRecord { get; set; }

        [SpParam(SqlParamName = "BySportRecord")]
        public long BySportRecord { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Monitoring_CustomerClassificationPool_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}