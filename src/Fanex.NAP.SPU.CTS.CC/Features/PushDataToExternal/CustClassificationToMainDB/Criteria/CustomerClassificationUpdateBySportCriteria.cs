namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustomerClassificationUpdateBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "custList")]
        public string custList { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdateCustomerClassBySport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
