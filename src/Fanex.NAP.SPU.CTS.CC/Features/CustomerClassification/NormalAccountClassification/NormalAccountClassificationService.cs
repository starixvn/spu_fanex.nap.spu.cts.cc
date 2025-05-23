namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountClassificationService
    {
        GetNormalPoolCustomerModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(IEnumerable<GetCurrentCCParamModel> custInfos);

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

        public IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(IEnumerable<GetCurrentCCParamModel> custInfos)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custInfosJson = JsonConvert.SerializeObject(custInfos);

                return dataAccess.GetCurrentCustomerCategory(custInfosJson);
            }

            return Enumerable.Empty<GetCurrentNormalCategoryEntity>();
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
