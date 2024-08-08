namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using System.Collections.Generic;

    public class ResultRemoveCustomerClassificationBySportModel
    {
        public IEnumerable<ResultRemoveCustomerClassificationEntity> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}