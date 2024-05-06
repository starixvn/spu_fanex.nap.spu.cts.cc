namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport
{
    using Sentry;
    using System.Collections.Generic;

    public interface INormalAccountClassificationBySportService
    {
        GetNormalPoolCustomerBySportModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentNormalCategoryBySportEntity> GetCurrentCustomerCategory(string custSportList);

        INormalAccountClassificationBySportService ClassifyNormalPoolCustomers(string customersXML);

        INormalAccountClassificationBySportService ClearNormalPoolCustomers(string customersXML, long scannedMaxId);
    }

    public class NormalAccountClassificationBySportService : INormalAccountClassificationBySportService
    {
        public readonly INormalAccountClassificationBySportDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public NormalAccountClassificationBySportService(ISentryClient sentryClient, INormalAccountClassificationBySportDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public GetNormalPoolCustomerBySportModel GetNormalPoolCustomers()
        {
            return dataAccess.GetNormalPoolCustomers();
        }

        public IEnumerable<GetCurrentNormalCategoryBySportEntity> GetCurrentCustomerCategory(string custSportList)
        {
            return dataAccess.GetCurrentCustomerCategory(custSportList);
        }

        public INormalAccountClassificationBySportService ClassifyNormalPoolCustomers(string customersXML)
        {
            dataAccess.ClassifyNormalPoolCustomers(customersXML);

            return this;
        }

        public INormalAccountClassificationBySportService ClearNormalPoolCustomers(string customersXML, long scannedMaxId)
        {
            if (scannedMaxId > 0)
            {
                dataAccess.ClearNormalPoolCustomers(customersXML, scannedMaxId);
            }

            return this;
        }
    }
}
