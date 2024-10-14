namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RescanParametersEntity
    {
        public RescanParametersEntity()
        {
            IsWriteLog = false;
        }

        public bool IsToQueue { get; set; }

        public FunctionId FunctionId { get; set; }

        public RescanNormalType RescanNormalType { get; set; }

        public int InternalBatchSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        public string AffectDownlineAgencyURL { get; set; }

        public PushDataToExternalEntity PushDataExternal { get; set; }

        public FunctionId FuncId_CheckRobotUser
        {
            get
            {
                switch (FunctionId)
                {
                    case FunctionId.InsertNewCustJob:
                        return FunctionId.InsertNewCus_CheckRobotUser;

                    case FunctionId.RemoveVIPWeb:
                        return FunctionId.RemoveVVIP_CheckRobotUser;

                    case FunctionId.RemoveSpecialCCWeb:
                        return FunctionId.RemoveSpecial_CheckRobotUser;

                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// True for removing specialCC
        /// </summary>
        public bool IsDataChanged
        {
            get
            {
                if (FunctionId == FunctionId.RemoveSpecialCCWeb)
                {
                    return true;
                }

                return false;
            }
        }
    }
}