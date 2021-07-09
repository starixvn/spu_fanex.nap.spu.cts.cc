namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.MatchMonitor;
    using Fanex.NAP.SPU.CTS.Features.MatchMonitor.Entities;
    using Fanex.NAP.SPU.CTS.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorJobService
    {
        Task<bool> Execute(int batchSize);

        Task<bool> UpdateWriteLogSetting(bool isWriteLog);
    }

    public class MatchMonitorJobService : IMatchMonitorJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly IMatchMonitorServices matchMonitorServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly IAppSettings appSettings;

        public MatchMonitorJobService(
            ISentryClient sentryClient,
            IMatchMonitorServices matchMonitorServices,
            ISystemParameterServices commonServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.matchMonitorServices = matchMonitorServices;
            this.systemParameterServices = commonServices;
            this.appSettings = appSettings;
            this.serviceName = "MatchMonitor";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                if (appSettings.MatchMonitorNumberOfThread > 0 && batchSize > 0)
                {
                    JobRun(batchSize);
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "Invalid NumberOfThread, BatchSize", (appSettings.MatchMonitorNumberOfThread, batchSize), new NullReferenceException(), sentryClient);
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateWriteLogSetting(bool isWriteLog)
        {
            try
            {
                bool result = appSettings.UpdateIsWriteLogSetting(serviceName, isWriteLog);

                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                sentryClient.CaptureException(e);
                return Task.FromResult(false);
            }
        }

        #region Private Methods

        private void JobRun(int batchSize)
        {
            Int64 lastScannedTransId = GetLastScannedTransId();

            var matchTrans = GetMatchTrans(lastScannedTransId, batchSize);

            if (matchTrans != null && matchTrans.Any())
            {
                InsertMatchMonitor(matchTrans);
            }
        }

        private Int64 GetLastScannedTransId()
        {
            var lastScannedTransId = systemParameterServices.GetLastScannedId(SystemParameters.MatchMonitorLastScannedTrans);

            Utilities.LogSentryInfo(serviceName, "Get LastScannedTransId", lastScannedTransId, appSettings.MatchMonitorIsWriteLog, sentryClient);

            return lastScannedTransId;
        }

        private IEnumerable<TicketDetailEntity> GetMatchTrans(Int64 lastScannedTransId, int batchSize)
        {
            var matchTrans = new List<TicketDetailEntity>();

            if (batchSize > 0)
            {
                matchTrans = matchMonitorServices.GetMatchTrans(lastScannedTransId, batchSize).ToList();
            }

            Utilities.LogSentryInfo(serviceName, "Get MatchTrans", matchTrans, appSettings.MatchMonitorIsWriteLog, sentryClient);
            return matchTrans;
        }

        private void InsertMatchMonitor(IEnumerable<TicketDetailEntity> matchTrans)
        {
            if (matchTrans != null && matchTrans.Any())
            {
                var groupIds = matchMonitorServices.InsertMatchMonitorStaging(JsonConvert.SerializeObject(matchTrans));

                Utilities.LogSentryInfo(serviceName, "Get GroupIds", groupIds, appSettings.MatchMonitorIsWriteLog, sentryClient);

                if (groupIds != null && groupIds.Any())
                {
                    InsertParallelMatchRule(groupIds.ToList());
                    Utilities.LogSentryInfo(serviceName, "Insert Rule done", string.Empty, appSettings.MatchMonitorIsWriteLog, sentryClient);
                }

                matchMonitorServices.UpdateMatchTransComplete();
                Utilities.LogSentryInfo(serviceName, "Update milestone done", matchTrans.Select(c => c.TransId).Max(), appSettings.MatchMonitorIsWriteLog, sentryClient);
            }
        }

        private void InsertParallelMatchRule(List<int> groupIds)
        {
            var log = new ConcurrentQueue<int>();

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = appSettings.MatchMonitorNumberOfThread
            };

            try
            {
                Parallel.For(0, groupIds.Count, options, i =>
                {
                    try
                    {
                        log.Enqueue(groupIds[i]);

                        matchMonitorServices.InsertMatchMonitorRule(groupIds[i]);

                        Thread.Sleep(5);
                    }
                    catch (Exception ex)
                    {
                        Utilities.LogSentryError(serviceName, $"InsertMatchMonitorRule fail", groupIds[i], ex, sentryClient);
                        throw new Exception($"{serviceName} - InsertMatchMonitorRule fail - Data {JsonConvert.SerializeObject(groupIds)}", ex);
                    }
                });
            }
            catch (AggregateException ex)
            {
                Utilities.LogSentryInfo(serviceName, "Check duplicate  ", log, true, sentryClient);
                throw new AggregateException($"{serviceName} - InsertMatchMonitorRule fail - Data {JsonConvert.SerializeObject(groupIds)}", ex);
            }
        }

        #endregion Private Methods
    }
}