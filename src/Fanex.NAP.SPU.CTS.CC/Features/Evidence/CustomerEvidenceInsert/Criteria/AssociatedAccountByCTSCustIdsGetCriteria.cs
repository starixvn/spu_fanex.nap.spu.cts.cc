namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AssociatedAccountByCTSCustIdsGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDList")]
        public string CTSCustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetByCustList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}