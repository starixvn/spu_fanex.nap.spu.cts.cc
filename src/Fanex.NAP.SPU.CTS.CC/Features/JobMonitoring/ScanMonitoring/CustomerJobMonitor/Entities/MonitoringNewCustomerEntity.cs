namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using System;

    public class MonitoringNewCustomerEntity
    {
        public MonitoringNewCustomerEntity(MonitoringNewCustomerEntity input)
        {
            CustID = input.CustID;
            CustSubID = input.CustSubID;
            RoleID = input.RoleID;  
            UserName = input.UserName;  
            CreatedDate = input.CreatedDate;    
        }

        public MonitoringNewCustomerEntity(ReactivatedCustModel input)
        {
            CustID = input.CustID;
            CustSubID = 0;
        }

        public long CustID { get; set; }

        public int CustSubID { get; set; }

        public int RoleID { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}