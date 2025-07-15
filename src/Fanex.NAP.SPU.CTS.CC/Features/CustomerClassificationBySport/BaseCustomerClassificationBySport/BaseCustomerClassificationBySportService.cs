namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationBySportService
    {
        ResultInsertClassificationBySportModel InsertCustomerClassificationBySport(CCInputFlow inputFlow, InsertCustomerClassificationBySportModel model, string serviceName);

        ResultRemoveCustomerClassificationBySportModel RemoveCustomerClassificationBySport(CCInputFlow inputFlow, RemoveCustomerClassificationBySportModel model, string serviceName);
    }

    public class BaseCustomerClassificationBySportService : IBaseCustomerClassificationBySportService
    {
        private readonly IBaseCustomerClassificationBySportDataAccess baseDataAccess;

        public BaseCustomerClassificationBySportService(IBaseCustomerClassificationBySportDataAccess baseDataAccess)
        {
            this.baseDataAccess = baseDataAccess;
        }

        public ResultInsertClassificationBySportModel InsertCustomerClassificationBySport(CCInputFlow inputFlow, InsertCustomerClassificationBySportModel model, string serviceName)
        {
            var result = this.baseDataAccess.InsertCustomerClassificationBySport(inputFlow, model);

            if (result != null && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Utilities.LogSentryError(serviceName, "InsertCustomerClassificationBySport throw Exception", model, new Exception(result.ErrorMessage));
            }

            return result;
        }

        public ResultRemoveCustomerClassificationBySportModel RemoveCustomerClassificationBySport(CCInputFlow inputFlow, RemoveCustomerClassificationBySportModel model, string serviceName)
        {
            var result = this.baseDataAccess.RemoveCustomerClassificationBySport(inputFlow, model);

            if (result != null && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Utilities.LogSentryError(serviceName, "RemoveCustomerClassificationBySport throw Exception", model, new Exception(result.ErrorMessage));
            }

            return result;
        }
    }
}