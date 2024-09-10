namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAIDetectionService
    {
        IRobotAIDetectionService Start();

        IRobotAIDetectionService GetRobotDetections(CTS_SystemParameters lastRobotAIIDPara);

        bool InsertAIRobotPackage(int internalSize, PushDataToExternalEntity pushConfig);

        IRobotAIDetectionService UpdateLastScannedId(CTS_SystemParameters lastAIRobotIDPara);
    }

    public class RobotAIDetectionService : ProblemAccountServices, IRobotAIDetectionService
    {
        private readonly IRobotAIDetectionDataAccess dataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private IEnumerable<RobotAIEntity> robotDetections;

        public RobotAIDetectionService(IRobotAIDetectionDataAccess dataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ISentryClient sentryClient) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.dataAccess = dataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;         
        }
        
        public IEnumerable<RobotAIEntity> RobotDetections => robotDetections;

        public IRobotAIDetectionService Start()
        {
            this.robotDetections = new List<RobotAIEntity>();
            
            return this;
        }

        public IRobotAIDetectionService GetRobotDetections(CTS_SystemParameters lastRobotAIIDPara)
        {
            var lastRobotAIID = systemParameterServices.GetLastScannedId(lastRobotAIIDPara);

            if (lastRobotAIID > 0)
            {
                robotDetections = this.dataAccess.GetRobotDetections(lastRobotAIID);                                              
            }

            return this;
        }

        public bool InsertAIRobotPackage(int internalSize, PushDataToExternalEntity pushConfig)
        {
            if (this.robotDetections != null && this.robotDetections.Any())
            {
                var robotDetectionChunks = this.robotDetections.Chunk(internalSize);

                foreach (var robot in robotDetectionChunks)
                {

                    var insertedPA = robot.ToList().Select(c => new InsertProblemAccountEntity(c));

                    var custIds = InsertProblemAccountPackage(insertedPA, FunctionId.RobotUserJob, CCInputFlow.InsertPACategory, false, pushConfig.ServiceName);

                    if (custIds != null && custIds.Any())
                    {
                        var isSuccess = custClassToMainDBService.PushExternalPackageByCustId(custIds, pushConfig);

                        if (!isSuccess)
                        {
                            return false;
                        }
                    }

                    Thread.Sleep(RobotUsers_SleepTimes);
                }
            }

            return true;
        }

        public IRobotAIDetectionService UpdateLastScannedId(CTS_SystemParameters lastAIRobotIDPara)
        {
            if (this.robotDetections != null && this.robotDetections.Any())
            {               
                var lastAIRobotID = this.robotDetections.Max(x => x.RobotDetectionID);

                if (lastAIRobotID > 0)
                {
                    systemParameterServices.UpdateTimingExecution(lastAIRobotID.ToString(), lastAIRobotIDPara);
                }
            }

            return this;
        }   
    }
}