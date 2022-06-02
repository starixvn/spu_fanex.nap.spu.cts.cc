namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NewCustomerClassificationEntity
    {
        public NewCustomerClassificationEntity()
        {
            VVIPClassification = Enumerable.Empty<VVIPCategoryEntity>();
            PAClassification = Enumerable.Empty<InsertProblemAccountEntity>();
            NormalClassification = Enumerable.Empty<NormalRescanEntity>();
        }

        public UInt64 LastCTSCustID { get; set; }

        public IEnumerable<VVIPCategoryEntity> VVIPClassification { get; set; }

        public IEnumerable<InsertProblemAccountEntity> PAClassification { get; set; }

        public IEnumerable<NormalRescanEntity> NormalClassification { get; set; }
    }
}