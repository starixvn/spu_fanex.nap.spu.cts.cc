namespace Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification
{
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class VVIPCustomerInsertEntity
    {
        public VVIP_FromAction FromAction { get; set; }

        public int? UplineRoleID { get; set; }

        public int? QueueID { get; set; }

        public bool? IsAffectDownline { get; set; }        

        public IEnumerable<VVIPCustomerEntity> CustInfo { get; set; }
    }
}
