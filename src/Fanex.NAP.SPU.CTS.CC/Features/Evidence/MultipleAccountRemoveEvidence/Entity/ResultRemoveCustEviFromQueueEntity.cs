namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using System.Collections.Generic;

    public class ResultRemoveCustEviFromQueueEntity
    {
        public bool IsRemoveSuccess { get; set; }

        public IEnumerable<long> CustIds { get; set; }
    }
}