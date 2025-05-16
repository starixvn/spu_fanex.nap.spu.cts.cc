namespace Fanex.NAP.SPU.CTS.CC.Shares
{
    using System;
    using System.Linq;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public static class BusinessExtension
    {
        public static bool IsTWTaggedCustValid(this TWTaggedCustEntity tWTaggedCust)
        {
            return tWTaggedCust != null && tWTaggedCust.TaggingID != 0;
        }

        public static bool IsCTSTaggedCustValid(this CTSTaggedCustEntity ctsTaggedCust, int taggingID)
        {
            return taggingID == 0 && ctsTaggedCust != null && ctsTaggedCust.TaggingID != 0;
        }

        public static bool IsTWTaggedSpecialLicSubCustValid(this CTSTaggedCustEntity twTaggedCust, int taggingID, int scanSpecialLicSubType)
        {
            return taggingID == 0 && scanSpecialLicSubType == 1 && twTaggedCust != null && twTaggedCust.TaggingID != 0;
        }

        public static bool IsAgency(this InsertNewCustomerResultModel customer)
        {
            return customer.RoleID > (int)CustomerRole.Member;
        }

        public static bool IsMember(this InsertNewCustomerResultModel customer)
        {
            return customer.RoleID == (int)CustomerRole.Member;
        }

        public static Func<InsertNewCustomerResultModel, bool> MatchRoleGroup(RoleGroupType roleGroup)
        {
            return customer => roleGroup switch
            {
                RoleGroupType.Agency => customer.IsAgency(),
                RoleGroupType.Member => customer.IsMember(),
                _ => false
            };
        }

        public static Func<GetLatencyCustomerFromQueueEntity, bool> MatchLatencyNewCustomer()
        {
            return customer => NewCustomerLatencyTypes.Contains(customer.LatencyType);
        }

        private static readonly int[] NewCustomerLatencyTypes = new[] {
            (int)JobMonitor_IssueType.NewCustomer_Missing,
            (int)JobMonitor_IssueType.NewCustomer_IncorrectCustInfo
        };

        public static Func<GetLatencyCustomerFromQueueEntity, bool> MatchLatencyReactivatedCustomer()
        {
            return customer => ReactivatedCustomerLatencyTypes.Contains(customer.LatencyType);
        }

        private static readonly int[] ReactivatedCustomerLatencyTypes = new[] {
            (int)JobMonitor_IssueType.ReactivatedCustomer_Missing,
            (int)JobMonitor_IssueType.ReactivatedCustomer_IncorrectCustInfo
        };
    }
}
