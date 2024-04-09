namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalTaggingClassification.Entities;
    using System.Collections.Generic;

    public class GetNormalTaggingScanningModel
    {
        public int LastCategoryID { get; set; }

        public ulong LastCustID { get; set; }

        public IEnumerable<GetNormalTaggingScanningViewModel> CustInfos { get; set; }
    }
}
