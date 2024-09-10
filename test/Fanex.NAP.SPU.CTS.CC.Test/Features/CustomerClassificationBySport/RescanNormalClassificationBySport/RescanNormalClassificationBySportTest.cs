using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RescanNormalClassificationBySport;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RescanNormalClassificationBySport
{
    public class RescanNormalClassificationBySportTest
    {
        private readonly IRescanNormalClassificationBySportDataAccess _dataAccess;

        private readonly IRescanNormalClassificationBySportServices _service;

        public RescanNormalClassificationBySportTest()
        {
            _dataAccess = Substitute.For<IRescanNormalClassificationBySportDataAccess>();

            _service = new RescanNormalClassificationBySportServices(_dataAccess);
        }

        #region PreprocessRescanNormalBySportClassification
        [Fact]
        public void PreprocessRescanNormalBySportClassification_WhenSuccess()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _dataAccess.PreprocesRescanNormalClassificationBySport(Arg.Any<string>(), Arg.Any<BySport_FunctionId>());

            _service.PreprocessRescanNormalBySportClassification(custIds, BySport_FunctionId.InsertNewCustJob);

            // Assert
            _dataAccess.Received(1).PreprocesRescanNormalClassificationBySport(Arg.Any<string>(), Arg.Any<BySport_FunctionId>());
        }
        #endregion
    }
}
