namespace Fanex.NAP.SPU.CTS.CC.Shares
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;

    public static class BusinessExtension
    {
        public static bool IsTWTaggedCustValid(this TWTaggedCustEntity tWTaggedCust)
        {
            return tWTaggedCust != null && tWTaggedCust.TaggingID != 0;
        }
    }
}
