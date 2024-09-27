namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using System;
 
    public class SMARealtimeCheckChangeEntities
    {
        public int Recommend { get; set; }

        public int CustID { get { return this.Recommend; } }

        public DateTime? LastTicketDate { get; set; }
    }
}
