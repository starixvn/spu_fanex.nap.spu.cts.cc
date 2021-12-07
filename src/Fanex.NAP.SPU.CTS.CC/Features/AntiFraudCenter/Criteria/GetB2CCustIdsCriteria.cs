namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetB2CCustIdsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIds")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "AFC_Get_B2CCustId";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}