namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Collections.Generic;
    using System.Linq;

    public class NewCTSCustomerClassificationEntity
    {
        public NewCTSCustomerClassificationEntity()
        {
            VVIPClassification = Enumerable.Empty<VVIPCustomerEntity>();
            PAClassification = Enumerable.Empty<InsertProblemAccountEntity>();
            NormalClassification = Enumerable.Empty<NormalRescanEntity>();
            LastCTSCustID = null;
        }
        
        public long? LastCTSCustID { get; set; }

        public IEnumerable<VVIPCustomerEntity> VVIPClassification { get; set; }

        public IEnumerable<InsertProblemAccountEntity> PAClassification { get; set; }
        
        public IEnumerable<NormalRescanEntity> NormalClassification { get; set; }
    }
}