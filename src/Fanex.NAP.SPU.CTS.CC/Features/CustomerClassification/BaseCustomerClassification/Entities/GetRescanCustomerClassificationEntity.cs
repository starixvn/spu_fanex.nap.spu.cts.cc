namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System;
    
    public class GetRescanCustomerClassificationEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public int TVSReasonID { get; set; }

        public int? RobotCounter { get; set; }

        public int? TVSRequestID { get; set; }

        public bool? IsParlay { get; set; }

        public int? SportType { get; set; }

        public int DWSportType { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public string Remark { get; set; }

        public long CreatedBy { get; set; }

        public bool IsMarkedDirectly { get; set; }

        public bool IsFromTVS { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromImperva { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int InputFlowID { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}
