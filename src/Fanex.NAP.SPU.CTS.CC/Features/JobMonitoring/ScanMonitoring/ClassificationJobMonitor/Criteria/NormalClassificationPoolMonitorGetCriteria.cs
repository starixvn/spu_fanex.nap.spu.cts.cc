namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data.Repository;

    public class NormalClassificationPoolMonitorGetCriteria : NonQueryCommand
    {
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