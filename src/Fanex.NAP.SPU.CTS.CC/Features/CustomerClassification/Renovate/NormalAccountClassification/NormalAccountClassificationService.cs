namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;

    public interface INormalAccountClassificationService
    {
        GetNormalPoolCustomerModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentCustomerCategoryEntity> GetCurrentCustomerCategory(string custIDList);

        INormalAccountClassificationService ClassifyNormalPoolCustomers(string customerXML);

        INormalAccountClassificationService ClearNormalPoolCustomers(string custIdList, long scannedMaxId);
    }

    public class NormalAccountClassificationService : INormalAccountClassificationService
    {
        private readonly INormalAccountClassificationDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public NormalAccountClassificationService(ISentryClient sentryClient, INormalAccountClassificationDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;            
        }

        public GetNormalPoolCustomerModel GetNormalPoolCustomers()
        {
            return dataAccess.GetNormalPoolCustomers();
        }

        public IEnumerable<GetCurrentCustomerCategoryEntity> GetCurrentCustomerCategory(string custIDList)
        {
            return dataAccess.GetCurrentCustomerCategory(custIDList);
        }

        public INormalAccountClassificationService ClassifyNormalPoolCustomers(string customerXML)
        {            
            dataAccess.ClassifyNormalPoolCustomers(customerXML);
            
            return this;
        }

        public INormalAccountClassificationService ClearNormalPoolCustomers(string custIdList, long scannedMaxId)
        {
            if (scannedMaxId > 0)
            {
                dataAccess.ClearNormalPoolCustomers(custIdList, scannedMaxId);
            }

            return this;
        }

        
    }

}
