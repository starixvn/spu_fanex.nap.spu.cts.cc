namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities
{
    using System;

    public class RobotAIEntity
    {
        public long RobotDetectionID { get; set; }

        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}
