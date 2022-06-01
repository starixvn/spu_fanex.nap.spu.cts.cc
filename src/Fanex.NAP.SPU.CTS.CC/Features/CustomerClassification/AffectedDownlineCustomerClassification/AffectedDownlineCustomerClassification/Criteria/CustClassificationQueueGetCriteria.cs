namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CustClassificationQueueGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "op_QueueID")]
        public UInt64 QueueID { get; set; }

        [SpParam(SqlParamName = "op_LastDownlineCTSCustID")]
        public UInt64 LastDownlineCTSCustID { get; set; }

        [SpParam(SqlParamName = "op_ActionType")]
        public int ActionType { get; set; }

        [SpParam(SqlParamName = "op_UplineRoleID")]
        public int UplineRoleID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Queue_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}