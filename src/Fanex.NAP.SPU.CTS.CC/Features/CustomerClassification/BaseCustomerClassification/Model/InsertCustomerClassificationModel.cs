namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System;

    public class InsertCustomerClassificationModel
    {      
        public string CustInfoJson { get; set; }

        public int FromAction { get; set; }

        public int? UplineRoleID { get; set; }

        public Int64? QueueID { get; set; }

        public bool? IsAffectDownline { get; set; }

        public bool IsToQueue { get; set; }
    }
}
