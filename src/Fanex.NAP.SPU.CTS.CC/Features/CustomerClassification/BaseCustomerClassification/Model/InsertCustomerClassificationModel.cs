namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class InsertCustomerClassificationModel
    {      
        public string CustClassificationJson { get; set; }

        public bool IsToQueue { get; set; }
    }
}
