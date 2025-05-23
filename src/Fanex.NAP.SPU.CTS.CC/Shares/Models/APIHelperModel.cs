namespace Fanex.NAP.SPU.CTS.CC.Shares.Models
{
    public class APIHelperModel<TResult> : APIHelperModel
    {
        public TResult? Data { get; set; }
    }

    public class APIHelperModel
    {
        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
