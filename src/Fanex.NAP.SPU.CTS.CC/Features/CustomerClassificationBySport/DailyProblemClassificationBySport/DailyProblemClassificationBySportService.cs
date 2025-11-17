namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyProblemClassificationBySportService
    {
        IDailyProblemClassificationBySportService Start();

        IDailyProblemClassificationBySportService GetDailyPAFromQueue(int batchSize);

        IEnumerable<ProblemAccountLosingBySportEntity> GetProblemAccountLosingClassification(BySport_FunctionId functionId);

        IProblemWLClassificationBySportService GetInstanceProblemWLClassificationBySportService();

        bool UpdateComplete();
    }

    public class DailyProblemClassificationBySportService : IDailyProblemClassificationBySportService
    {
        private readonly IProblemWLClassificationBySportDataAccess _dataAccess;
        private readonly IProblemAccountBySportService _paService;
        private readonly IServiceProvider _serviceProvider;

        private IEnumerable<ProblemAccountInfoFromDailyQueueModel> PACustomers;

        public DailyProblemClassificationBySportService(
            IProblemWLClassificationBySportDataAccess dataAccess,
            IServiceProvider serviceProvider,
            IProblemAccountBySportService paService)
        {
            _dataAccess = dataAccess;
            _serviceProvider = serviceProvider;
            _paService = paService;
        }

        public IDailyProblemClassificationBySportService Start()
        {
            PACustomers = new List<ProblemAccountInfoFromDailyQueueModel>();

            return this;
        }

        public IDailyProblemClassificationBySportService GetDailyPAFromQueue(int batchSize)
        {
            PACustomers = _dataAccess.GetPADailyFromQueue(batchSize);

            return this;
        }

        public IEnumerable<ProblemAccountLosingBySportEntity> GetProblemAccountLosingClassification(BySport_FunctionId functionId)
        {
            if (PACustomers.IsNotNullOrEmpty())
            {
                return _paService.GetPALosingBySport(PACustomers, functionId);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportEntity>();
        }

        public IProblemWLClassificationBySportService GetInstanceProblemWLClassificationBySportService()
        {
            return _serviceProvider.GetService<IProblemWLClassificationBySportService>();
        }

        public bool UpdateComplete()
        {
            var completeCustInfos = string.Empty;

            if (PACustomers.IsNotNullOrEmpty())
            {
                completeCustInfos = JsonConvert.SerializeObject(PACustomers);

                _dataAccess.CompletePADailyFromQueue(completeCustInfos);
            }

            return PACustomers.IsNotNullOrEmpty();
        }
    }
}
