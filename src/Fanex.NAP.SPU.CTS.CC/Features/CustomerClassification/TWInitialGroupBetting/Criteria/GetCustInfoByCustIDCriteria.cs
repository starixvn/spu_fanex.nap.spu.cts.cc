namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWInitialGroupBetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCustInfoByCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustInfo_GetByCustID";

        public override bool IsValid() => true;
    }
}
