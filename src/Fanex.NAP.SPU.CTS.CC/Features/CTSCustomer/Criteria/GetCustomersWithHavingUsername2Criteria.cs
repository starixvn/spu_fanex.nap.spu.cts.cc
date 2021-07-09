namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomersWithHavingUsername2Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custXML")]
        public string CustomerXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetNotNULLUserName2ByXML";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerXML);
        }
    }
}