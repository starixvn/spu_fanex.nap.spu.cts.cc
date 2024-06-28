namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System.Collections.Generic;

    public class ResultInsertCustomerClassificationModel
    {
        public IEnumerable<CustInfoClassificationEntity> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
