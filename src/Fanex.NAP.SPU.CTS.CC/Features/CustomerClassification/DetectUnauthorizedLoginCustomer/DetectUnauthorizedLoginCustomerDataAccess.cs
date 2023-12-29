namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IDetectUnauthorizedLoginCustomerDataAccess
    {
        IEnumerable<CustFirstBetDateEntity> GetCustWithFirstBetDate(GetCustWithFirstBetDateParam param);

        IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson);
    }

    public class DetectUnauthorizedLoginCustomerDataAccess : IDetectUnauthorizedLoginCustomerDataAccess
    {
        private readonly IDynamicRepository repository;

        public DetectUnauthorizedLoginCustomerDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustFirstBetDateEntity> GetCustWithFirstBetDate(GetCustWithFirstBetDateParam param)
        {
            var criteria = new GetCustWithFirstBetDateCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsLicensee = param.IsLicensee,
                LastCustID = param.LastCustID,
                BatchSize = param.BatchSize
            };

            return repository.Fetch<CustFirstBetDateEntity>(criteria);
        }

        public IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson)
        {
            var criteria = new DetectUnauthorizedLoginCustomerCriteria
            {
                CustJson = custJson
            };

            return repository.Fetch<DetectUnauthorizedLoginCustEntity>(criteria);
        }


    }
}
