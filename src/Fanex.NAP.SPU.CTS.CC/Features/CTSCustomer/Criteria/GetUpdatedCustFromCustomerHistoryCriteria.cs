namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetUpdatedCustFromCustomerHistoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "lastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }

        [SpParam(SqlParamName = "lastCustId")]
        public Int64 LastCustId { get; set; }

        [SpParam(SqlParamName = "rowSize")]
        public int RowSize { get; set; }

        [SpParam(SqlParamName = "waitTime")]
        public int WaitTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerHistory_GetUpdatedCustID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}