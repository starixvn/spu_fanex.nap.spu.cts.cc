namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveSystemFormulaBetCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_TWProblemDetectionMonitor_Remove";

        public override bool IsValid()
            => true;
    }
}
