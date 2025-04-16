namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// FromCustID, ToCustID
    /// Expect Result = Min of ToCustID - ScannedTime
    /// </summary>
    public class GetLastScannedCustIDCriteria : CriteriaBase
    {
        public int FromCustID { get; set; }
        
        public int ToCustID { get; set; }

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
