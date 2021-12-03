namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using System.Collections.Generic;

    public class DeleteAccountParasViewEntity
    {
        public string Key { get; set; }

        public IEnumerable<DeleteAccountParasEntity> Data { get; set; }
    }
}