namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class GetCustSubInfoFromMainDBForMonitorCriteria : CriteriaBase
    {
        public int RowSize { get; set; }

        public long LastCustSubId { get; set; }

        public DateTime MaxScannedDate { get; set; }


        public override string GetSettingKey()
        {
            return "CTS_Customer_Monitoring_GetCustSubInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}