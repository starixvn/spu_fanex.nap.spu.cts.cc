namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;    

    public class CustomerCategoryQueueModel
    {
        public CustomerCategoryQueueModel()
        {
            this.CustomerCategoryQueue = Enumerable.Empty<CustomerCategoryEntity>();
            this.QueueInfo = new QueueInfoEntity();
        }

        public IEnumerable<CustomerCategoryEntity> CustomerCategoryQueue { get; set; }

        public QueueInfoEntity QueueInfo { get; set; }

    }
}
