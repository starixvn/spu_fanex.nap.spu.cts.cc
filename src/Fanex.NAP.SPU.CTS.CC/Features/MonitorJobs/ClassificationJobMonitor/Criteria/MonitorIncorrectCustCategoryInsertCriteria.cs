namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class MonitorIncorrectCustCategoryInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerListJson { get; set; }

        [SpParam(SqlParamName = "ip_InvalidType")]
        public int InvalidType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_InsertIncorrectCategory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerListJson);
        }
    }
}