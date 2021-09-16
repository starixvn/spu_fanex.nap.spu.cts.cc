namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using System.Collections.Generic;

    public class CustClassificationRealtimeViewEntity
    {
        public List<CustClassificationRealtimeEntity> CustClassification { get; set; }

        public List<CustClassificationRealtimeEntity> CustCategories { get; set; }
    }
}