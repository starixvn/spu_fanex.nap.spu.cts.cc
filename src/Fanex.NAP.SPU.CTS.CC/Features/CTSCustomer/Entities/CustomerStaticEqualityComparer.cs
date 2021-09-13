namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System.Collections.Generic;

    public class CustomerStaticEqualityComparer : IEqualityComparer<CustomerMainDBModel>
    {
        public bool Equals(CustomerMainDBModel x, CustomerMainDBModel y)
        {
            if (x.CustID == y.CustID && x.CustStatusID == y.CustStatusID)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(CustomerMainDBModel obj)
        {
            return obj.GetHashCode();
        }
    }
}