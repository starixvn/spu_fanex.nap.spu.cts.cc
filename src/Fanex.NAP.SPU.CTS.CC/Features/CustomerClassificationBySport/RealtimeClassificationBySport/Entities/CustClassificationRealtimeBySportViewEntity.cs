namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using System.Collections.Generic;

    public class CustClassificationRealtimeBySportViewEntity
    {
        public List<CustClassificationRealtimeBySportEntity> CustClassification { get; set; }

        public List<CustClassificationRealtimeBySportEntity> CustCategories { get; set; }
    }
}