namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// FromCustID, ToCustID
    /// Expect Result = Min of ToCustID - ScannedTime
    /// </summary>
    public class GetLastScannedCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromCustID")]
        public int FromCustID { get; set; }

        [SpParam(SqlParamName = "ip_ToCustID")]
        public int ToCustID { get; set; }

        [SpParam(SqlParamName = "ip_TotalSecond")]
        public int ScannedTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetMinCustIDByScannedTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
