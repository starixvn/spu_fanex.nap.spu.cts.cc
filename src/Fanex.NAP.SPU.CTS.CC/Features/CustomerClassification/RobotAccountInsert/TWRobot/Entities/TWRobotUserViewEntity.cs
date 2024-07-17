namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using System.Collections.Generic;
    using System.Linq;

    public class TWRobotUserViewEntity
    {
        public IEnumerable<TWRobotUserEntity> RobotUsers { get; set; }

        public long MaxTimestamp { get; set; }

        public void SetRobotUserCategory(int? robotCategoryID)
        {
            if (RobotUsers != null 
                && RobotUsers.Any()
                && robotCategoryID != null)
            {
                RobotUsers.ToList().ForEach(c =>
                {
                    c.CategoryID = robotCategoryID;
                });
            }
        }
    }
}