namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections.Generic;

    public class ResultRemoveCustomerEvidence
    {
        public DBResult DBResult { get; set; }

        public IEnumerable<CustomerEvidenceRemovedEntity> CustEviRemovedList { get; set; }
    }
}