namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ErrorLogInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        [SpParam(SqlParamName = "ip_Message")]
        public string ErrorMessage { get; set; }

        [SpParam(SqlParamName = "ip_Info")]
        public string DataJson { get; set; }

        public override string GetSettingKey()
        {
            return "AFC_DC_ErrorLog_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}