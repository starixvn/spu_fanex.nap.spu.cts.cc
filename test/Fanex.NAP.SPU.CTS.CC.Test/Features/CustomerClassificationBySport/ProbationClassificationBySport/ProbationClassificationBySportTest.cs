using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;

using System.Collections.Generic;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.ProbationClassificationBySportConsts;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    public class ProbationClassificationBySportTest
    {
        private readonly IProbationClassificationBySportDataAccess _dataAccess;
        private readonly ISystemParameterDataAccess _systemParameterDataAccess;

        private readonly IProbationClassificationBySportService _service;

        public ProbationClassificationBySportTest()
        {
            _dataAccess = Substitute.For<IProbationClassificationBySportDataAccess>();
            _systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();

            _service = new ProbationClassificationBySportService(_dataAccess, _systemParameterDataAccess);
        }

        #region GetProbationScanClassificationBySport
        [Fact]
        public void GetProbationScanClassificationBySport_WhenSuccess()
        {
            // Arrange
            var probationDataField = TestExtension.GetPrivateFieldInfo(typeof(ProbationClassificationBySportService), PrivateFieldName.ProbationData);
            probationDataField.SetValue(_service, new ProbationBySportModel());

            // Act
            _dataAccess.GetProbationScanClassificationBySport().Returns(new ProbationBySportModel());

            _service.GetProbationScanClassificationBySport();

            // Assert
            _dataAccess.Received(1).GetProbationScanClassificationBySport();
        }
        #endregion

        #region PreprocessProbationClassificationBySport
        [Fact]
        public void PreprocessProbationClassificationBySport_WhenSuccess()
        {
            // Arrange
            var probationDataField = TestExtension.GetPrivateFieldInfo(typeof(ProbationClassificationBySportService), PrivateFieldName.ProbationData);
            probationDataField.SetValue(_service, new ProbationBySportModel
            {
                CustProbations = new List<ProbationScanClassificationBySportEntity> {
                    new ProbationScanClassificationBySportEntity { CustId = 1, SportGroup = 1 },
                    new ProbationScanClassificationBySportEntity { CustId = 2, SportGroup = 1 },
                    new ProbationScanClassificationBySportEntity { CustId = 3, SportGroup = 1 },
                }
            });

            // Act
            _dataAccess.PreprocessProbationClassificationBySport(Arg.Any<IEnumerable<ProbationScanClassificationBySportEntity>>());

            _service.PreprocessProbationClassificationBySport();

            // Assert
            _dataAccess.Received(1).PreprocessProbationClassificationBySport(Arg.Any<IEnumerable<ProbationScanClassificationBySportEntity>>());
        }
        #endregion

        #region UpdateCompleteProbationScanBySport
        [Fact]
        public void UpdateCompleteProbationScanBySport_WhenSuccess()
        {
            // Arrange
            var probationDataField = TestExtension.GetPrivateFieldInfo(typeof(ProbationClassificationBySportService), PrivateFieldName.ProbationData);
            probationDataField.SetValue(_service, new ProbationBySportModel
            {
                CustProbations = new List<ProbationScanClassificationBySportEntity> {
                    new ProbationScanClassificationBySportEntity { CustId = 1, SportGroup = 1 },
                    new ProbationScanClassificationBySportEntity { CustId = 2, SportGroup = 1 },
                    new ProbationScanClassificationBySportEntity { CustId = 3, SportGroup = 1 },
                },
                LastCustID = 999
            });

            // Act
            _systemParameterDataAccess.UpdateTimingExecutionForCTS(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            var result = _service.UpdateCompleteProbationScanBySport();

            // Assert
            _systemParameterDataAccess.Received(1).UpdateTimingExecutionForCTS(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            Assert.True(result);
        }
        #endregion
    }
}
