namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using System.Collections.Generic;

    public class InsertNormalAccountResultEntity
    {       
        public IEnumerable<long> CustIds { get; set; }

        public string ErrorMessage { get; set; }
    }
}
