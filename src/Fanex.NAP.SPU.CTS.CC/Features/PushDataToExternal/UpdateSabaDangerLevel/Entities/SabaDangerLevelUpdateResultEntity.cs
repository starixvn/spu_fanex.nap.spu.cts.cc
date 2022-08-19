namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    public class SabaDangerLevelUpdateResultEntity
    {
        public SabaDangerLevelUpdateResultEntity()
        {
            IsSuccess = true;
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public bool IsSuccess { get; set; }
    }
}