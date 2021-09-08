namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities
{
    using System.Collections.Generic;

    public class GeneralCustomerClassificationEnity
    {
        public IEnumerable<CustomerClassificationEntity> GeneralCustClassifications { get; set; }

        public bool IsContinute { get; set; }
    }
}