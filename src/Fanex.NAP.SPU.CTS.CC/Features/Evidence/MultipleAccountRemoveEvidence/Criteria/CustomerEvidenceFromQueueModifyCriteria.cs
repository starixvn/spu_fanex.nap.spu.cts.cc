namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{

    using Fanex.Data;
    using Fanex.Data.Repository;

    class CustomerEvidenceFromQueueModifyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustEvidence")]
        public string CustomerEvidence { get; set; }

        [SpParam(SqlParamName = "ip_ActionID")]
        public int ActionId { get; set; }

        [SpParam(SqlParamName = "op_Result")]
        public string CustIdResultList { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }


        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidenceQueue_TakeAction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
