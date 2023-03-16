namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data.Repository;

    public class PADailyScanFromQueueGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScanPA_GetFromQueue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}