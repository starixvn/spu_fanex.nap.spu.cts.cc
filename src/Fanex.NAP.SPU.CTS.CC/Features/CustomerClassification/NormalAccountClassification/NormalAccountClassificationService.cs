namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using System.Collections.Generic;

    public interface INormalAccountClassificationService
    {
        GetNormalPoolCustomerModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(string custIDList);

        INormalAccountClassificationService ClassifyNormalPoolCustomers(string customerXML);

        INormalAccountClassificationService ClearNormalPoolCustomers(string custIdList, long scannedMaxId);
    }

    public class NormalAccountClassificationService : INormalAccountClassificationService
    {
        private readonly INormalAccountClassificationDataAccess dataAccess;

        public NormalAccountClassificationService(INormalAccountClassificationDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public GetNormalPoolCustomerModel GetNormalPoolCustomers()
        {
            return dataAccess.GetNormalPoolCustomers();
        }

        public IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(string custIDList)
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
