namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using System.Collections.Generic;

    public class CustomerInfoEqualityComparer : IEqualityComparer<CustomerMainDBEntity>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1067:Expressions should not be too complex", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S121:Control structures should use curly braces", Justification = "<Pending>")]
        public bool Equals(CustomerMainDBEntity x, CustomerMainDBEntity y)
        {
            if (x.CustID == y.CustID
                && x.CustStatusID == y.CustStatusID
                && x.Danger1 == y.Danger1
                && x.Danger2 == y.Danger2
                && x.Danger3 == y.Danger3
                && x.Danger4 == y.Danger4
                && x.Danger5 == y.Danger5
                && x.DangerSabaSc == y.DangerSabaSc
                && x.DangerSabaBkb == y.DangerSabaBkb)
                return true;

            return false;
        }

        public int GetHashCode(CustomerMainDBEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}