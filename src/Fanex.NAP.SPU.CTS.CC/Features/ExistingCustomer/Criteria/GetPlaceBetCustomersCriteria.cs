using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.SPU.CTS.Features.ExistingCustomer.Criteria
{
    public class GetPlaceBetCustomersCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GetPlacedBetCustomers";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
