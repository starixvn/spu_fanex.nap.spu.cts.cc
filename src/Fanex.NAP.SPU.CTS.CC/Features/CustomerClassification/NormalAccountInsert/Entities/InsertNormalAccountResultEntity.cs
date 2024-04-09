namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using System.Collections.Generic;

    public class InsertNormalAccountResultEntity
    {       
        public IEnumerable<long> CustIds { get; set; }

        public string ErrorMessage { get; set; }
    }
}
