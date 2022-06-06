namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;
    using System.Collections.Generic;

    public class TVSProblemAccountResultEntity
    {
        public IEnumerable<TVSProblemAccountEntity> ProblemAccounts { get; set; }

        public DateTime? MaxTimeRequestActive { get; set; }
    }
}