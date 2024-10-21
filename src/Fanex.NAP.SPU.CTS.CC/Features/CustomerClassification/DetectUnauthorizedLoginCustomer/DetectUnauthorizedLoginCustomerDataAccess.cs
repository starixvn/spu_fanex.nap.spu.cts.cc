namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public interface IDetectUnauthorizedLoginCustomerDataAccess
    {
        IEnumerable<CustFirstBetDateEntity> GetCustWithFirstBetDate(GetCustWithFirstBetDateParam param);

        IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson, DateTime processDate);
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

        public IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson, DateTime processDate)
        {
            var criteria = new DetectUnauthorizedLoginCustomerCriteria
            {
                CustJson = custJson,
                ProcessDate = processDate
            };

            return repository.Fetch<DetectUnauthorizedLoginCustEntity>(criteria);
        }


    }
}
