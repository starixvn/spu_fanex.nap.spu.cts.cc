namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using System.Collections.Generic;

    public class GetNotificationMonitoringEntity
    {
        public IEnumerable<GetInvalidDataMonitoringEntity> InvalidDataList { get; set; }

        public bool IsContinue { get; set; } = false;
    }
}