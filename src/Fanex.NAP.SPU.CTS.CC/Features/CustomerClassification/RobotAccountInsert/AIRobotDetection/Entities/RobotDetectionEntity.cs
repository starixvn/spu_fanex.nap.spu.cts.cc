namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities
{
    using System;

    public class RobotDetectionEntity
    {
        public long RobotDetectionID { get; set; }

        public long RobotImpervaLogID { get; set; }

        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public DateTime LastCreateTime { get; set; }

        public DateTime LastTimeOrderBy
        {
            get
            {
                if (this.LastModifiedDate != DateTime.MinValue)
                {
                    return this.LastModifiedDate;
                }
                else
                {
                    return this.LastCreateTime;
                }
            }
        }

        public bool IsFromAI { get; set; }

        public bool IsFromImperva { get; set; }
    }
}