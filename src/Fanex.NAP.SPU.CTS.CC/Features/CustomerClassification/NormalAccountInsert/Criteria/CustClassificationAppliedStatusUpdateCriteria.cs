namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;

    public class CustClassificationAppliedStatusUpdateCriteria : NonQueryCommand
    {
        public string ListCustIds { get; set; }

        public bool IsGeneralCategory { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_UpdateAppliedStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}