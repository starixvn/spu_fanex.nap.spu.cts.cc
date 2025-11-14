namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Criteria
{
    using Fanex.Data.Repository;

    public class PADailyScanFromQueueBySportGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_DailyScanPA_GetFromQueue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
