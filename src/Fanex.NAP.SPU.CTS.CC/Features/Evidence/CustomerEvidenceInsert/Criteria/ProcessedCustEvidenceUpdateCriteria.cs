namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProcessedCustEvidenceUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IDList")]
        public string IdList { get; set; }

        [SpParam(SqlParamName = "ip_IsProcessed")]
        public bool IsProcessed { get; set; }

        [SpParam(SqlParamName = "ip_UpdatedBy")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidenceFromFile_UpdateProcessed";

        public override bool IsValid()
            => true;
    }
}