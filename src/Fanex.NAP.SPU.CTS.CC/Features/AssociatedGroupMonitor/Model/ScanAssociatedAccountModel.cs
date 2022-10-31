namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class ScanAssociatedAccountModel
    {
        public ScanAssociatedAccountModel()
        {
            ScanAssociatedAccountList = Enumerable.Empty<ScanAssociatedAccountEntity>();
        }

        public IEnumerable<ScanAssociatedAccountEntity> ScanAssociatedAccountList { get; set; }

        public long GroupID { get; set; }

        public int ABI { get; set; }

        public int Ori { get; set; }

        public int PACategoryID { get; set; }

        public long LastCTSCustID { get; set; }
    }
}
