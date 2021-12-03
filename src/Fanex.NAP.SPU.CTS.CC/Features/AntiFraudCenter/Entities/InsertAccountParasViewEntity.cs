namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using System.Collections.Generic;

    public class InsertAccountParasViewEntity
    {
        public string Key { get; set; }

        public IEnumerable<InsertAccountParasEntity> Data { get; set; }
    }
}