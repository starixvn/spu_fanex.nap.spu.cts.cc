namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.AffectedDownline
{
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class VVIPCustomerInsertEntity
    {
        public VVIP_FromAction FromAction { get; set; }

        public int? UplineRoleID { get; set; }

        public Int64? QueueID { get; set; }

        public bool? IsAffectDownline { get; set; }

        public IEnumerable<VVIPCategoryEntity> CustInfo { get; set; }
    }
}