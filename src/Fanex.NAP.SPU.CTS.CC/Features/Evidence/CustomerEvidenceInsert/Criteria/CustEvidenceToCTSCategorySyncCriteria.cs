namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustEvidenceToCTSCategorySyncCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustEvidences")]
        public string CustEvidences { get; set; }

        [SpParam(SqlParamName = "ip_IsRescan")]
        public bool IsRescan { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_SyncToCategory";

        public override bool IsValid()
            => true;
    }
}