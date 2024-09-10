namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System.Collections.Generic;

    public class ResultRemoveCustomerClassificationModel
    {
        public IEnumerable<ResultRemoveCustomerClassificationEntity> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
