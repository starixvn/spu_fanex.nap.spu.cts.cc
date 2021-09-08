namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    public class ProblemAccountsEntity
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        //Default value = 0, except RobotUser job
        public int RobotCounter { get; set; }
    }
}