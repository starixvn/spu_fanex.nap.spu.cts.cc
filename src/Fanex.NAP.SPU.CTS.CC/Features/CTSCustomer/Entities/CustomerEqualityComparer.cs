namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using System.Collections.Generic;

    public class CustomerEqualityComparer : IEqualityComparer<CustomerMainDBEntity>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1067:Expressions should not be too complex", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S121:Control structures should use curly braces", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        public bool Equals(CustomerMainDBEntity x, CustomerMainDBEntity y)
        {
            if (x.CustID == y.CustID
                && string.Equals(x.UserName, y.UserName)
                && string.Equals(x.UserName2, y.UserName2)
                && x.SiteID == y.SiteID
                && string.Equals(x.Site, y.Site)
                && x.RoleID == y.RoleID
                && x.CurrencyID == y.CurrencyID
                && string.Equals(x.Currency, y.Currency)
                && x.SRecommend == y.SRecommend
                && x.MRecommend == y.MRecommend
                && x.Recommend == y.Recommend)
                return true;

            return false;
        }

        public int GetHashCode(CustomerMainDBEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}