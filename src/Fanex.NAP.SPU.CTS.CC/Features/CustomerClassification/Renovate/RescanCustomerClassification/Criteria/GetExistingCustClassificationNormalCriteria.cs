namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetExistingCustClassificationNormalCriteria : CriteriaBase
    {    
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Normal_GetExisting";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
