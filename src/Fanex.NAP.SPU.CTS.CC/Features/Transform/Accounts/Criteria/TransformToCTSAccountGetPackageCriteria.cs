using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.SPU.CTS.Features.Transform.Accounts.Criteria
{
    public class TransformToCTSAccountGetPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfAccount")]
        public int NumberOfAccount { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_TransformToCTS_Account_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
