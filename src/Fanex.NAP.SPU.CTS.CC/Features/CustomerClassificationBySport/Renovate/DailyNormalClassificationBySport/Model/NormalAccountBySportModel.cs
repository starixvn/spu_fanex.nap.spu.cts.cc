namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using System.Collections.Generic;

    public class NormalAccountBySportModel
    {
        public int LastCategoryID { get; set; }

        public ulong LastCustID { get; set; }

        public IEnumerable<NormalAccountBySportEntity> NormalAccounts { get; set; }
    }
}
