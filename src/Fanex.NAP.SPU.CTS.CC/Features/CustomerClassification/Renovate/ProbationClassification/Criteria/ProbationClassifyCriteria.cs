namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.ProbationClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProbationClassifCriteria : NonQueryCommand
    {
        public string ListCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_Probation_Preprocess";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ListCustId);
        }
    }
}