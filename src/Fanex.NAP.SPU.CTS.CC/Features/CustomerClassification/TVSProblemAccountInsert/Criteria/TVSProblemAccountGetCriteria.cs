namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TVSProblemAccountGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "Datetime")]
        public DateTime? Datetime { get; set; }

        [SpParam(SqlParamName = "IsArchive")]
        public bool IsArchive { get; set; }

        [SpParam(SqlParamName = "MaxTimeRequestActive")]
        public DateTime? MaxTimeRequestActive { get; set; }

        public override string GetSettingKey()
        {
            return "TicketVoiding_CTS_SendProblemCustomerSelect";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}