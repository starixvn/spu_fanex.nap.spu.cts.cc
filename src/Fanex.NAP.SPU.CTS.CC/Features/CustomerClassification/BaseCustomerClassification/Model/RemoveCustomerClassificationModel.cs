namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    public class RemoveCustomerClassificationModel
    {
        public string CustClassificationJson { get; set; }

        public int FromAction { get; set; }

        public bool AffectDownline { get; set; }

        public bool IsFromQueue { get; set; }
    }
}
