namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;

    public class InsertPABySportResultModel : BaseCustomerInfoBySportModel
    {
        public InsertPABySportResultModel() { }

        public InsertPABySportResultModel(InsertPAProbationBySportResultEntity entity)
        {
            CustID = entity.CustID;
            SportGroup = entity.SportGroup;
        }
    }
}
