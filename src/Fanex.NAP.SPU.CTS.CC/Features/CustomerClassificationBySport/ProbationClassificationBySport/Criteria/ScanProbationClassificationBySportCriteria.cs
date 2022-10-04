namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.Data.Repository;

    public class ScanProbationClassificationBySportCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_Probation";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomersXML);
        }
    }
}
