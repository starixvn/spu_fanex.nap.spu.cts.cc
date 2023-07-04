namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport
{
    public class NormalAccountBySportEntity
    {
        public long CustId { get; set; }        

        public int CategoryId { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public long BetCount { get; set; }

        public int ActiveDays { get; set; }

        public int SportId { get; set; }    
        
    }
}
