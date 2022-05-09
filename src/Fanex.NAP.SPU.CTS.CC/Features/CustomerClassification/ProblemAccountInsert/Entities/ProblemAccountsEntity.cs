namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Newtonsoft.Json;
    using System;

    public class ProblemAccountsEntity
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        //Default value = 0, except Check/Get RobotUser From MainDB
        public int RobotCounter { get; set; }

        [JsonProperty("CreatedBy")]
        public Int64? Creator { get; set; }
    }
}