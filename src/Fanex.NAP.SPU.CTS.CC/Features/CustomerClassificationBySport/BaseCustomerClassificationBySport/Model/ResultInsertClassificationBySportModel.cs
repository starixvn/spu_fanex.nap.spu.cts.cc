namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using System.Collections.Generic;

    public class ResultInsertClassificationBySportModel
    {
        public IEnumerable<CustInfoClassificationBySportEntity> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}