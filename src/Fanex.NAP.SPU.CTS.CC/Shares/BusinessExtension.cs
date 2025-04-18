namespace Fanex.NAP.SPU.CTS.CC.Shares
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using System;

    public static class BusinessExtension
    {
        public static bool IsTWTaggedCustValid(this TWTaggedCustEntity tWTaggedCust)
        {
            return tWTaggedCust != null && tWTaggedCust.TaggingID != 0;
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
    }
}
