namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCustWithFirstBetDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "IsLicensee")]
        public bool IsLicensee { get; set; }

        [SpParam(SqlParamName = "LastCustID")]
        public int LastCustID { get; set; }

        [SpParam(SqlParamName = "BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_GetCustWithFirstbetdate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
