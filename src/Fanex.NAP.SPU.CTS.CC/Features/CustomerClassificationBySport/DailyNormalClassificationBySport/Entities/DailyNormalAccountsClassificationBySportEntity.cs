namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using System.Collections.Generic;

    public class DailyNormalAccountsClassificationBySportEntity
    {
        public int LastCategoryID { get; set; }

        public long LastCustID { get; set; }

        public int LastSportID { get; set; }

        public IEnumerable<long> CustID { get; set; }
    }
}
