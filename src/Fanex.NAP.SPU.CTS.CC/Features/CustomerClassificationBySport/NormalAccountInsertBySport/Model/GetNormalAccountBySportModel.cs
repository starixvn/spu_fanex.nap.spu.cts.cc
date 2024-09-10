namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using System.Collections.Generic;

    public class GetNormalAccountBySportModel
    {
        public GetNormalAccountBySportModel()
        {
            this.NormalAccount = new List<NormalAccountBySportEntity>();
        }

        public IEnumerable<NormalAccountBySportEntity> NormalAccount { get; set; }

        public long ScannedMaxId { get; set; }
    }
}