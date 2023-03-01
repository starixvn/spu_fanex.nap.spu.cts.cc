namespace Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using System;
    using System.Collections.Generic;

    public class InsertMissingCustomerModel
    {
        public IEnumerable<MissingCustomerCompareEntity> MissingCustomers { get; set; }

        public IEnumerable<CustomerMainDBModel> CustomerInfo { get; set; }

        public bool IsAutoInsertMissing { get; set; } 

        public DateTime ScannedTimeFrom { get; set; }

        public DateTime ScannedTimeTo { get; set; }
    }
}