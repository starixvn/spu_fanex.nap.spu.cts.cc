namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustEvidenceGetByReasonIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_InputCustomers")]
        public string CustInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidence_GetByReasonId";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}