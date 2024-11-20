namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification.NewCustomerCustomerClassification.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NewCustomerClassificationModel
    {
        public NewCustomerClassificationModel()
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
