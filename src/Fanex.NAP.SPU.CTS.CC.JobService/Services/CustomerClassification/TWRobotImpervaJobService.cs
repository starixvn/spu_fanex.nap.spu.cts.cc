namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWRobotImpervaJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class TWRobotImpervaJobService : ITWRobotImpervaJobService
    {
        private readonly string serviceName;
        private readonly ITWRobotImpervaService twRobotImpervaService;
        private readonly ISentryClient sentryClient;

        public TWRobotImpervaJobService(ISentryClient sentryClient, ITWRobotImpervaService twRobotImpervaService)
        {
            this.sentryClient = sentryClient;
            this.twRobotImpervaService = twRobotImpervaService;
            this.serviceName = "RobotImperva";
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                var lastLogID = twRobotImpervaService.GetLastScannedID(CTS_DC_SystemParameters.TWRobotImperva_LastLogID);                

                var robots = twRobotImpervaService.GetTWRobotImperva(lastLogID);                

                if (robots.RobotInfos.Any())
                {
                    twRobotImpervaService.InsertTWRobotImperva(robots.RobotInfos);

                    twRobotImpervaService.UpdateLastScannedID(CTS_DC_SystemParameters.TWRobotImperva_LastLogID, robots.MaxLogID);
                }              
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

    }
}
