namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckNewPAAssociationCustomerForRollbackCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Arc_CustomerAssociation_RollBack_CheckByNewPA";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}