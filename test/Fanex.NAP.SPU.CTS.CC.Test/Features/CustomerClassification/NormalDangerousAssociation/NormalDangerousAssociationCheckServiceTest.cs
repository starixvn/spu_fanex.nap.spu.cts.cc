using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
using NSubstitute;
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
        public void PreprocessDangerousAssociation_Success()
        {
            // Arrange

            // Act
            _service.PreprocessDangerousAssociation(new List<PreprocessDangerousAssociationModel> { new PreprocessDangerousAssociationModel() });

            // Assert
            _dataAccess.Received(1).PreprocessDangerousAssociation(Arg.Any<IEnumerable<PreprocessDangerousAssociationModel>>());
        }

        [Fact]
        public void PreprocessDangerousAssociation_Empty()
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
    }
}
