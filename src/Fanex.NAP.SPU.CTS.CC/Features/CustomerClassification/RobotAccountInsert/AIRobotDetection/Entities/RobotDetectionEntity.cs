namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities
{
    using System;

    public class RobotDetectionEntity
    {
        public long CustID { get; set; }

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