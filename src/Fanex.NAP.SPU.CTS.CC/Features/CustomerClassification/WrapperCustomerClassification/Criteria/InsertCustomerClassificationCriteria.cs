namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.WrapperCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID")]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_FromAction")]
        public int FromAction { get; set; }

        [SpParam(SqlParamName = "ip_UplineRoleID")]
        public int UplineRoleID { get; set; }

        [SpParam(SqlParamName = "ip_QueueID")]
        public long QueueID { get; set; }

        [SpParam(SqlParamName = "ip_AffectDownline")]
        public bool AffectDownline { get; set; }

        [SpParam(SqlParamName = "ip_IsToQueue")]
        public bool IsToQueue { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustClassificationJson { get; set; }

        //[SpParam(SqlParamName = "ip_CustomerClass")]
        //public int CustomerClass { get; set; }

        //[SpParam(SqlParamName = "ip_Remark")]
        //public string Remark { get; set; }

        //[SpParam(SqlParamName = "ip_CreatedBy")]
        //public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
