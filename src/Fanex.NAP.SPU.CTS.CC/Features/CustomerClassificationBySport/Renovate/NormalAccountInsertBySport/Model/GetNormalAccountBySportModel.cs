namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport
{
    using System.Collections.Generic;

    public class GetNormalAccountBySportModel
    {
        public GetNormalAccountBySportModel()
        {
            this.NormalAccount = new List<GetNormalAccountBySportEntity>();
            this.IsContinute = false;
        }

        public IEnumerable<GetNormalAccountBySportEntity> NormalAccount { get; set; }

        public long ScannedMaxId { get; set; }

        public bool IsContinute { get; set; }
    }
}
