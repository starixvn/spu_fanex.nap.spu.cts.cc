namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter.Entities
{
    public class AFCParaModel
    {
        public string InsertURL { get; set; }

        public string UpdateURL { get; set; }
        
        public string DeleteURL { get; set; }

        public string GetURL { get; set; }
        
        public bool IsSyncAFCData { get; set; }
        
        public string InternalKey { get; set; }
    }
}