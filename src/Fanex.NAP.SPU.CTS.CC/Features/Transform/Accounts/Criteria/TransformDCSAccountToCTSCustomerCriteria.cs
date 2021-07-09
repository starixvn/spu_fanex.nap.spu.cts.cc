using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.CTS.Features.Transform.Accounts.Criteria
{
    public class TransformDCSAccountToCTSCustomerCriteria: NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AccountList")]
        public string AccountJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_TransformDCSAccountToCTSCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
