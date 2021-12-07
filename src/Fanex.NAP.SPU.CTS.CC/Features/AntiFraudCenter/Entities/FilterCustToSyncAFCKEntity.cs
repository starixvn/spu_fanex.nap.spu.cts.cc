namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using System.Collections.Generic;
    using System.Linq;

    public class FilterCustToSyncAFCKEntity
    {
        public int CustID { get; set; }

        public string FraudID { get; set; }

        public IEnumerable<int> FraudIDList
        {
            get
            {
                if (!string.IsNullOrEmpty(FraudID))
                {
                    return FraudID.Split(',').Select(int.Parse);
                }

                return new List<int>();
            }
        }
    }
}