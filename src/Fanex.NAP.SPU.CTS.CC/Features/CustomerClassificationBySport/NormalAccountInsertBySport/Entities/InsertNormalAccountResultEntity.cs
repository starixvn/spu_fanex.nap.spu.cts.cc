namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using System.Collections.Generic;

    public class InsertNormalAccountResultEntity
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<InsertNormalAccountCustInfoEntity> CustInfos { get; set; }
    }
}
