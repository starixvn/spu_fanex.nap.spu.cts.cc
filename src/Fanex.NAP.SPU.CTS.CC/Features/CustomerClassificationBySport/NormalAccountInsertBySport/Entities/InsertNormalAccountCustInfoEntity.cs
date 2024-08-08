namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;

    public class InsertNormalAccountCustInfoEntity
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

        public long CustID { get; set; }

        public int SportGroup { get; set; }
    }
}