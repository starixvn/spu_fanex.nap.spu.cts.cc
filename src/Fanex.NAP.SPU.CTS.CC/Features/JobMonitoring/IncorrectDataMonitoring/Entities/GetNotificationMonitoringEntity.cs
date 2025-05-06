namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability Code Smell", "S3052:Members should not be initialized to default values", Justification = "<Pending>")]
    public class GetNotificationMonitoringEntity
    {
        public IEnumerable<GetInvalidDataMonitoringEntity> InvalidDataList { get; set; }

        public bool IsContinue { get; set; } = false;
    }
}