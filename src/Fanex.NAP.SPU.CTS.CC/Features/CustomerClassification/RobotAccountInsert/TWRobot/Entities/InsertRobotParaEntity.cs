namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;

    public class InsertRobotParaEntity
    {
        public PushDataToExternalEntity PushDataExternal { get; set; }

        public int BatchInternalSize { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsDirect { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }
    }
}