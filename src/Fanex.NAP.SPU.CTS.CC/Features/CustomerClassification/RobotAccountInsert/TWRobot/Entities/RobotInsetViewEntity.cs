namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Newtonsoft.Json;
    using System;

    public class RobotInsetViewEntity
    {
        public long CustID { get; set; }

        public int ReasonID { get; set; }

        //Default value = 0, except Check/Get RobotUser From MainDB
        public int RobotCounter { get; set; }

        [JsonProperty("CreatedBy")]
        public Int64? Creator { get; set; }
    }
}