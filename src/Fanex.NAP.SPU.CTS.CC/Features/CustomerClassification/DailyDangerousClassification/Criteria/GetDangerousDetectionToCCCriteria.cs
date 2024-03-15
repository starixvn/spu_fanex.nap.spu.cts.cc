namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousScoreClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetDangerousDetectionToCCCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ScannedDate")]
        public DateTime ScannedDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DangerousClassifyGet";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
