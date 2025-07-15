namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationService
    {
        GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs);

        ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model, string serviceName);

        ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model, string serviceName);

        int? GetCategoryByCategoryTypeName(string categoryTypeName);
    }

    public class BaseCustomerClassificationService : IBaseCustomerClassificationService
    {
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;

        public BaseCustomerClassificationService(IBaseCustomerClassificationDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs)
        {
            return this.baseDataAccess.GetRescanCustomerClassification(custIDs);
        }

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model, string serviceName)
        {
            var result = this.baseDataAccess.InsertCustomerClassification(inputFlow, model);

            if (result != null && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Utilities.LogSentryError(serviceName, "InsertCustomerClassification throw Exception", model, new Exception(result.ErrorMessage));
            }

            return result;
        }

        public ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model, string serviceName)
        {
            var result = this.baseDataAccess.RemoveCustomerClassification(inputFlow, model);

            if (result != null && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Utilities.LogSentryError(serviceName, "RemoveCustomerClassification throw Exception", model, new Exception(result.ErrorMessage));
            }

            return result;
        }

        public int? GetCategoryByCategoryTypeName(string categoryTypeName)
        {
            return this.baseDataAccess.GetCategoryByCategoryTypeName(categoryTypeName);
        }
    }
}
