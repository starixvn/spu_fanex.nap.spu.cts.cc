namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using System.Collections.Generic;

    public class DailyNormalAccountsClassificationBySportEntity
    {
        public int LastCategoryID { get; set; }

        public ulong LastCustID { get; set; }

        public uint LastSportID { get; set; }

        public IEnumerable<NormalAccountBySportEntity> NormalAccounts { get; set; }
    }
}
