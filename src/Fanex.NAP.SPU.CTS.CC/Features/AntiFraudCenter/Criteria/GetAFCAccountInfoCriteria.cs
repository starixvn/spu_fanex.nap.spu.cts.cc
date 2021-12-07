namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAFCAccountInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIdList")]
        public string B2CCustIdJson { get; set; }

        public override string GetSettingKey()
        {
            return "API_CustomerExtraInfo_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}