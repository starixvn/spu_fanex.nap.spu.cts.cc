namespace Fanex.NAP.SPU.CTS.CC.Features.SystemParameter
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter.Criteria;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ISystemParameterDataAccess
    {
        SystemParameterEntity GetPreviousTimingExecutionForCTS(CTS_SystemParameters paramID);

        void UpdateTimingExecutionForCTS(string paraValue, CTS_SystemParameters paramId);

        SystemParameterEntity GetPreviousTimingExecutionForMain(Main_SystemParameters paramID);

        void UpdateTimingExecutionForMain(string paraValue, Main_SystemParameters paramId);
    }

    public class SystemParameterDataAccess : ISystemParameterDataAccess
    {
        private readonly IDynamicRepository repository;

        public SystemParameterDataAccess()
        {
            repository = new DynamicRepository();
        }

        public SystemParameterEntity GetPreviousTimingExecutionForCTS(CTS_SystemParameters paramID)
        {
            var criteria = new PreviousTimingExecutionGetDCCriteria
            {
                ParamID = (int)paramID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<SystemParameterEntity>(criteria);
            }
        }

        public void UpdateTimingExecutionForCTS(string paraValue, CTS_SystemParameters paramId)
        {
            var criteria = new PreviousTimingExecutionUpdateDCCriteria
            {
                ParamId = (int)paramId,
                ParamValue = paraValue
            };

            repository.Execute(criteria);
        }

        public SystemParameterEntity GetPreviousTimingExecutionForMain(Main_SystemParameters paramID)
        {
            var criteria = new PreviousTimingExecutionGetMainCriteria
            {
                ParamID = (int)paramID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<SystemParameterEntity>(criteria);
            }
        }

        public void UpdateTimingExecutionForMain(string paraValue, Main_SystemParameters paramId)
        {
            var criteria = new PreviousTimingExecutionUpdateMainCriteria
            {
                ParamId = (int)paramId,
                ParamValue = paraValue
            };

            repository.Execute(criteria);
        }
    }
}