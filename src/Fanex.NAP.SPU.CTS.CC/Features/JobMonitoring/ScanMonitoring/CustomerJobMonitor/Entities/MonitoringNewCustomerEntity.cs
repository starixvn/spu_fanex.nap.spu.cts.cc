namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using System;

    public class MonitoringNewCustomerEntity
    {
        public long CustID { get; set; }

        public int CustSubID { get; set; }

        public int RoleID { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}