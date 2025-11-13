namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;

    public class InsertPAProbationBySportResultModel : BaseCustomerInfoBySportModel
    {
        public InsertPAProbationBySportResultModel() { }
        public InsertPAProbationBySportResultModel(InsertPAProbationBySportResultEntity entity)
        {
            CustID = entity.CustID;
            SportGroup = entity.SportGroup;
        }
    }
}
