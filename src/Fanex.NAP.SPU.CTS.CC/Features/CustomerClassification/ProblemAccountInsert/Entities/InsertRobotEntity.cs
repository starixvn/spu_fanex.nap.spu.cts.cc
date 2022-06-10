namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities
{
    using System;

    public class InsertRobotEntity
    {
        public InsertRobotEntity()
        {
            IsFromTW = false;
            IsFromCTS = false;
            IsFromAI = false;
            IsFromTVS = false;
        }

        public Int64 CustID { get; set; }

        public int CategoryID { get; set; }

        public int? RobotCounter { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        //True: Robot from TW, False: Robot Classification
        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }

        public bool IsDirect { get; set; }

        public bool IsFromAI { get; set; }

        public string Remark { get; set; }

        public Int64? CreatedBy { get; set; }
    }
}