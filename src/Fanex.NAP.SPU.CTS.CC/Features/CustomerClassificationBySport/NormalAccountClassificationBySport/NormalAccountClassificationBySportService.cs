namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport
{
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
        private readonly INormalAccountClassificationBySportDataAccess dataAccess;

        public NormalAccountClassificationBySportService(INormalAccountClassificationBySportDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
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
