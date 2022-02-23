namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter.Entities
{
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class B2CAccountResultEntity
    {
        public IEnumerable<GetAccountEntity> B2CAccounts { get; set; }

        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}