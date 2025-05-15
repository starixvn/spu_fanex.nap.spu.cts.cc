namespace Fanex.NAP.SPU.CTS.CC.Shares.Models
{
    using System.Net;

    public class APIHelperModel<TResult> : APIHelperModel
    {
        public TResult? Data { get; set; }
    }

    public class APIHelperModel
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; }
    }
}
