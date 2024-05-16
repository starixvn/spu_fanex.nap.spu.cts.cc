namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Newtonsoft.Json;
    using System;

    public class ProblemAccountLosingClassificationModel
    {
        public ProblemAccountLosingClassificationModel(ProblemAccountLosingClassificationEntity data)
        {
            this.CustId = data.CustId;
            this.WinlossStatus = data.WinlossStatus;
            this.TurnoverRM = data.TurnoverRM;
            this.WinlossRM = data.WinlossRM;
            this.BetCount = data.BetCount;
            this.ActiveDays = data.ActiveDays;           
        }

        [JsonProperty("CustID")]
        public Int64 CustId { get; set; }

        public int WinlossStatus { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }

        public string Remark { get; set; }
    }
}
