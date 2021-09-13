namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustEvidencesByCTSCustIdsRemoveCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_SessionId")]
        public long SessionId { get; set; }

        [SpParam(SqlParamName = "ip_IsRollback")]
        public bool IsRollBack { get; set; }

        [SpParam(SqlParamName = "ip_IsClearLog")]
        public bool IsClearLog { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_RemoveAllEvidenceByCTSCustIDs";

        public override bool IsValid()
            => true;
    }
}