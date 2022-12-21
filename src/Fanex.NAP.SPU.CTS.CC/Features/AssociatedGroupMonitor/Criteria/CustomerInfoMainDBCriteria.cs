namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustomerInfoMainDBCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custIdList")]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "sourceType")]
        public int SourceType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetInfoByCustIdList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
