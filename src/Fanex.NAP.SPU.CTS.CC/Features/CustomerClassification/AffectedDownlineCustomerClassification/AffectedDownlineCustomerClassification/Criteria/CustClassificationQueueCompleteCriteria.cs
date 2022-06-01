namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CustClassificationQueueCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_QueueID")]
        public UInt64 QueueID { get; set; }

        [SpParam(SqlParamName = "ip_MaxCustID")]
        public UInt64 MaxCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Queue_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}