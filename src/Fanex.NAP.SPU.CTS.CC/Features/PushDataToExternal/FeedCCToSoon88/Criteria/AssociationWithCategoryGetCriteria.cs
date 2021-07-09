namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.FeedCCToSoon88
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AssociationWithCategoryGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "ip_IsGetAssociation")]
        public bool IsGetAssociation { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetForSoon88";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds);
        }
    }
}