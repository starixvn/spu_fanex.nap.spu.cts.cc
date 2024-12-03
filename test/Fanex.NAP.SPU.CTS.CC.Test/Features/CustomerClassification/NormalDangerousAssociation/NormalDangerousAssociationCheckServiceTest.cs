using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalDangerousAssociation
{
    public class NormalDangerousAssociationCheckServiceTest
    {
        private readonly INormalDangerousAssociationCheckDataAccess _dataAccess;
        private readonly INormalDangerousAssociationCheckService _service;

        public NormalDangerousAssociationCheckServiceTest()
        {
            _dataAccess = Substitute.For<INormalDangerousAssociationCheckDataAccess>();

            _service = new NormalDangerousAssociationCheckService(_dataAccess);
        }

        [Fact]
        public void PreprocessDangerousAssociation_WhenSuccess()
        {
            // Arrange

            // Act
            _service.PreprocessDangerousAssociation(new List<PreprocessDangerousAssociationModel> { new PreprocessDangerousAssociationModel() });

            // Assert
            _dataAccess.Received(1).PreprocessDangerousAssociation(Arg.Any<IEnumerable<PreprocessDangerousAssociationModel>>());
        }

        [Fact]
        public void PreprocessDangerousAssociation_WhenEmpty()
        {
            // Arrange

            // Act
            _service.PreprocessDangerousAssociation(new List<PreprocessDangerousAssociationModel>());

            // Assert
            _dataAccess.Received(0).PreprocessDangerousAssociation(Arg.Any<IEnumerable<PreprocessDangerousAssociationModel>>());
        }

        [Fact]
        public void PreprocessDangerousAssociation_ThrowException()
        {
            // Arrange

            // Act
            _dataAccess.When(x => x.PreprocessDangerousAssociation(Arg.Any<IEnumerable<PreprocessDangerousAssociationModel>>())).Throw<Exception>();

            Action action = () => _service
                                        .PreprocessDangerousAssociation(
                                            new List<PreprocessDangerousAssociationModel>
                                            {
                                                new PreprocessDangerousAssociationModel()
                                            });

            // Assert
            _dataAccess.Received(0).PreprocessDangerousAssociation(Arg.Any<IEnumerable<PreprocessDangerousAssociationModel>>());
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void ScanAssociationByDevice_WhenSuccess()
        {
            // Arrange

            // Act
            _service.ScanAssociationByDevice(5000);

            // Assert
            _dataAccess.Received(1).ScanAssociationByDevice(Arg.Any<int>());
        }

        [Fact]
        public void ScanAssociationByDevice_ThrowException()
        {
            // Arrange

            // Act
            _dataAccess.ScanAssociationByDevice(Arg.Any<int>()).Throws<Exception>();

            Action action = () => _service.ScanAssociationByDevice(5000);

            // Assert
            _dataAccess.Received(0).ScanAssociationByDevice(Arg.Any<int>());

            Assert.Throws<Exception>(action);
        }
    }
}
