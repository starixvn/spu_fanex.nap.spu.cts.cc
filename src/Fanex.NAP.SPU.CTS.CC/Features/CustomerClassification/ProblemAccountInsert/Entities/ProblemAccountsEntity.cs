namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using System;

    public class ProblemAccountsEntity
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        //Default value = 0, except Check/Get RobotUser From MainDB
        public int RobotCounter { get; set; }

        public Int64? Creator { get; set; }
    }
}