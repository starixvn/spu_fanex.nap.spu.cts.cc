namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using System;

    public class RobotUserGetCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public string CreditSite { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_RobotListSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}