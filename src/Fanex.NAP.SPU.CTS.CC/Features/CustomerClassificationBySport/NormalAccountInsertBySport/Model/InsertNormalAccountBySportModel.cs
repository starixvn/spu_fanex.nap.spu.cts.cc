namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using System.Collections.Generic;

    public class InsertNormalAccountBySportModel
    {
        public IEnumerable<InsertNormalAccountCustInfoEntity> CustNAInserted { get; set; }

        public string ErrorMessage { get; set; }
    }
}
