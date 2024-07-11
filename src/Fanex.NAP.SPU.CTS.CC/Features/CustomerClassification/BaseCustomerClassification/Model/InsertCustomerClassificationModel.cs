namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{   
    public class InsertCustomerClassificationModel
    {
        public int FromAction { get; set; }

        public int UplineRoleID { get; set; }

        public long QueueID { get; set; }

        public bool AffectDownline { get; set; }

        public bool IsToQueue { get; set; }

        public string CustClassificationJson { get; set; }
       
    }
}
