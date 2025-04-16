namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System.Collections.Generic;

    public class CustomerStaticEqualityComparer : IEqualityComparer<CustomerMainDBEntity>
    {
        public bool Equals(CustomerMainDBEntity x, CustomerMainDBEntity y)
        {
            if (x.CustID == y.CustID && x.CustStatusID == y.CustStatusID)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(CustomerMainDBEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}