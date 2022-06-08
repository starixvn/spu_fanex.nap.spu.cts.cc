namespace Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.DataLogTracking;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDataLogTrackingDataAccess
    {
        DBResult LogInputData(IEnumerable<LogInputDataEntity> content, FunctionId funcId);
    }

    public class DataLogTrackingDataAccess : IDataLogTrackingDataAccess
    {
        private readonly IDynamicRepository repository;

        public DataLogTrackingDataAccess()
        {
            repository = new DynamicRepository();
        }

        public DBResult LogInputData(IEnumerable<LogInputDataEntity> content, FunctionId funcId)
        {
            var criteria = new InputDataLogInsertCriteria
            {
                InputData = JsonConvert.SerializeObject(content),
                FunctionID = (int)funcId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : 1,
                ErrorMsg = criteria.ErrorMessage
            };
        }
    }
}