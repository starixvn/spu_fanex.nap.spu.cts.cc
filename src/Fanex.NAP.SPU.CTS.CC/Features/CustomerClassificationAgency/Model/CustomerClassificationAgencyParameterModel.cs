namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency.Model
{
    public class CustomerClassificationAgencyParameterModel<T>
    {
        public T Data { get; set; }

        public int InternalBatchSize { get; set; }

        public int ExternalBatchSize { get; set; }
    }
}
