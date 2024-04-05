namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models.ArchiveCustomer
{
    using System.Collections.Generic;

    public class CheckExistCTSArchiveCustModel
    {
        public IEnumerable<long> ExistCusts { get; set; }

        public IEnumerable<long> NotExistCusts { get; set; }

        public bool IsPushCCSuccess { get; set; }
    }
}
