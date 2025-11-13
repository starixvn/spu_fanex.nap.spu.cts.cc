namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;

    public class ProblemAccountInfoFromDailyQueueModel : BaseCustomerInfoBySportModel
    {
        public ProblemAccountInfoFromDailyQueueModel() { }
        public ProblemAccountInfoFromDailyQueueModel(ProblemAccountInfoFromDailyQueueEntity entity)
        {
            CustID = entity.CustID;
            SportGroup = entity.SportGroup;
        }
    }
}
