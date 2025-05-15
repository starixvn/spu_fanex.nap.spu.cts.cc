namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using System.Threading.Tasks;
    using Features.TWClassification;

    public interface ITWClassificationJobService
    {
        Task<bool> TWResponseHaifaService();
    }

    public class TWClassificationJobService : ITWClassificationJobService
    {
        private readonly ITWClassificationService twClassificationService;

        public TWClassificationJobService(ITWClassificationService twClassificationService)
        {
            this.twClassificationService = twClassificationService;
        }

        public Task<bool> TWResponseHaifaService()
        {
            return twClassificationService.SendDataToTW();
        }
    }
}
