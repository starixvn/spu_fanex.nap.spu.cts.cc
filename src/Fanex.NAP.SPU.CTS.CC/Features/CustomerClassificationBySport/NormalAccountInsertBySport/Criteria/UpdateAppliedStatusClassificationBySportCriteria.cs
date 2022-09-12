namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data.Repository;

    public class UpdateAppliedStatusClassificationBySportCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }
       
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_UpdateAppliedStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
