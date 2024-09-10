namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    public class RemoveCustomerClassificationModel
    {
        public string CustInfoJson { get; set; }

        public int FromAction { get; set; }

        public int? UplineRoleID { get; set; }

        public long? QueueID { get; set; }

        public bool? IsAffectDownline { get; set; }

        public bool IsFromQueue { get; set; }
    }
}
