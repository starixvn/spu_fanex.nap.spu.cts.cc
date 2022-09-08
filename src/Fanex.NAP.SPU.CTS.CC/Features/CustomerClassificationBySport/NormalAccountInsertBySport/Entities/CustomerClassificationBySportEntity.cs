namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CustomerClassificationBySportEntity
    {
        public long CustId { get; set; }

        public int CategoryId { get; set; }

        public int SportId { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public Int64 BetCount { get; set; }

        public int ActiveDays { get; set; }
    }
}
