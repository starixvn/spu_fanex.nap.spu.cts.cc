namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;

    public class InsertNormalAccountCustInfoEntity : BaseCustomerInfoBySportModel
    {
        public InsertNormalAccountCustInfoEntity(CustInfoClassificationBySportEntity custInfo)
        {
            CustID = custInfo.CustID;
            SportGroup = custInfo.SportGroup;
        }

        public InsertNormalAccountCustInfoEntity(NormalAccountBySportEntity custInfo)
        {
            CustID = custInfo.CustId;
            SportGroup = custInfo.SportGroup;
        }
    }
}