namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    public class InsertProblemAccountEntity : InsertCustomerClassificationEntity
    {
        public InsertProblemAccountEntity()
        {
            IsFromTVS = false;
            IsFromCTS = false;
            IsFromTW = false;
        }

        public InsertProblemAccountEntity(ClassifyGBMMCustInfosEntity gbMMCust)
        {
            this.CTSCustID = gbMMCust.CTSCustID;
            this.CustID = gbMMCust.CustID;
            this.RoleID = gbMMCust.RoleID;
            this.SubscriberID = gbMMCust.SubscriberID;
            this.CategoryID = gbMMCust.CategoryID;
            this.CategoryGroupID = gbMMCust.CategoryGroup;
            this.IsLicensee = gbMMCust.IsLicensee;
            this.MatchID = gbMMCust.MatchID;
            this.CreatedBy = Constants.StarixITId;
            this.Remark = gbMMCust.MatchID.ToString();
            this.IsMarkedDirectly = true;
            this.IsFromCTS = true;
            this.IsFromTVS = false;
            this.IsFromTW = false;
        }

        public InsertProblemAccountEntity(TVSProblemAccountEntity tvsPACust)
        {
            this.CustID = tvsPACust.CustID;
            this.ReasonID = tvsPACust.ReasonID;
            this.CreatedBy = tvsPACust.CreatedBy;
            this.TVSRequestID = tvsPACust.RequestID;
            this.IsTVSParlay = tvsPACust.IsParlay;
            this.SportType = tvsPACust.Sporttype;
            this.TVSIssueTypeID = tvsPACust.IssueTypeId;
            this.IsFromTVS = true;
            this.IsMarkedDirectly = true;
        }

        public InsertProblemAccountEntity(TWInitialGBGetChangesEntity twGBCust, PotentialPACustInfoEntity ctsCust)
        {
            this.CustID = twGBCust.CustID;
            this.CTSCustID = ctsCust.CTSCustID;
            this.RoleID = ctsCust.RoleID;
            this.SubscriberID = ctsCust.SubscriberID;
            this.CategoryID = ctsCust.CategoryID;
            //this.CategoryGroup = ctsCust.CategoryGroup;
            this.IsLicensee = ctsCust.IsLicensee;
            this.IsFromAI = false;
            this.IsMarkedDirectly = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Constants.StarixITId;
            this.SportType = twGBCust.SportType;
            //this.Remark = string.Format(Constants.TWTWInitialGBRemark, twGBCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy), twGBCust.GBTicketCount, twGBCust.SportName);
        }

        public InsertProblemAccountEntity(TWInitialSmartEntity tWInitialSmartCust)
        {
            this.CustID = tWInitialSmartCust.CustID;
            this.CTSCustID = tWInitialSmartCust.CTSCustID;
            this.RoleID = tWInitialSmartCust.RoleID;
            this.SubscriberID = tWInitialSmartCust.SubscriberID;
            this.CategoryID = tWInitialSmartCust.CategoryID;
            this.CategoryGroupID = tWInitialSmartCust.CategoryGroup;
            this.IsLicensee = tWInitialSmartCust.IsLicensee;
            this.IsFromAI = false;
            this.IsMarkedDirectly = null;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Constants.StarixITId;
            this.Remark = string.Format(Constants.TWInitialSmartRemark, tWInitialSmartCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy), tWInitialSmartCust.Probability);
        }

        public InsertProblemAccountEntity(GetRescanCustomerClassificationEntity data)
        {
            CTSCustID = data.CTSCustID;
            CustID = data.CustID;
            CategoryID = data.CategoryID;
            ReasonID = data.TVSReasonID;
            RoleID = data.RoleID;
            SubscriberID = data.SubscriberID;
            IsLicensee = data.IsLicensee;
            Remark = data.Remark;
            CreatedBy = data.CreatedBy;
            IsMarkedDirectly = data.IsMarkedDirectly;
            IsFromTVS = data.IsFromTVS;
            IsFromTW = data.IsFromTW;
            IsFromCTS = data.IsFromCTS;
            IsFromAI = data.IsFromAI;
            IsFromImperva = data.IsFromImperva;
            LastModifiedDate = data.LastModifiedDate;
        }

        public Int64 CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int? ReasonID { get; set; }

        public bool IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Remark { get; set; }

        public bool? IsMarkedDirectly { get; set; }

        public int? WinlossStatus { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromTVS { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromImperva { get; set; }

        public int? TVSRequestID { get; set; }

        public bool? IsTVSParlay { get; set; }

        public int? SportType { get; set; }

        public int TVSIssueTypeID { get; set; }

        public int? MatchID { get; set; }
    }
}