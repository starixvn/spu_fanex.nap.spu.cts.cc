namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    public class InsertProblemAccountEntity : CustomerClassificationEntity
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
            this.TVSReasonID = tvsPACust.ReasonID;
            this.CreatedBy = tvsPACust.CreatedBy;
            this.TVSRequestID = tvsPACust.RequestID;
            this.IsTVSParlay = tvsPACust.IsParlay;
            this.SportType = tvsPACust.Sporttype;
            this.TVSIssueTypeID = tvsPACust.IssueTypeId;
            this.IsFromTVS = true;
            this.IsMarkedDirectly = true;
        }

        public InsertProblemAccountEntity(TWRobotUserEntity robotUser, int? robotUserCategoryID)
        {
            this.CustID = robotUser.CustId;
            this.CategoryID = robotUserCategoryID;
            this.TWRobotCounter = robotUser.Counter;
            this.CreatedBy = Constants.StarixITId;
            this.IsFromTW = true;
            this.IsMarkedDirectly = true;
        }    
        
        public InsertProblemAccountEntity(RobotAIEntity robotAI)
        {
            this.CustID = robotAI.CustID;
            this.CategoryID = robotAI.CategoryID;
            this.LastModifiedDate = robotAI.LastModifiedDate;
            this.CreatedBy = Constants.StarixITId;
            this.IsFromAI = true;
            this.IsMarkedDirectly = true;
        }

        public InsertProblemAccountEntity(TWInitialGBGetChangesEntity twGBCust, PotentialPACustInfoEntity ctsCust)
        {
            this.CustID = twGBCust.CustID;
            this.CTSCustID = ctsCust.CTSCustID;
            this.RoleID = ctsCust.RoleID;
            this.SubscriberID = ctsCust.SubscriberID;
            this.CategoryID = ctsCust.CategoryID;
            this.IsLicensee = ctsCust.IsLicensee;
            this.IsFromAI = false;
            this.IsMarkedDirectly = false;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Constants.StarixITId;
            this.SportType = twGBCust.SportType;
            this.SportName = twGBCust.SportName;
            this.GBTicketCount = twGBCust.GBTicketCount;
            this.DetectedDate = twGBCust.DetectedDate;
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
            this.IsMarkedDirectly = false;
            this.IsFromCTS = false;
            this.IsFromTVS = false;
            this.IsFromTW = true;
            this.CreatedBy = Constants.StarixITId;           
        }

        public InsertProblemAccountEntity(GetRescanCustomerClassificationEntity data)
        {
            CTSCustID = data.CTSCustID;
            CustID = data.CustID;
            TVSReasonID = data.TVSReasonID;
            CategoryID = data.CategoryID;
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

        public int? TVSReasonID { get; set; }

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

        public string? SportName { get; set; }

        public int TVSIssueTypeID { get; set; }

        public int? MatchID { get; set; }

        public int? TWRobotCounter { get; set; }

        public string GroupName { get; set; }

        public int? GBTicketCount { get; set; }

        public DateTime? DetectedDate { get; set; }

    }
}