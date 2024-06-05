namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.PAPotentialClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class PAPotentialClassificationServiceTest
    {
        private readonly IPAPotentialClassificationDataAccess paPotentialDataAccess;
        private readonly IPAPotentialClassificationService paPotentialService;

        public PAPotentialClassificationServiceTest()
        {
            paPotentialDataAccess = Substitute.For<IPAPotentialClassificationDataAccess>();
            paPotentialService = new PAPotentialClassificationService(paPotentialDataAccess);
        }

        [Fact]
        public void SetTWInitialSmartSmarts_WhenSuccess()
        {
            // Arrange
            var paDailyFromQueueModel = new PADailyScanFromQueueModel();
            
            // Act
            Action action = () => paPotentialService.SetTWInitialSmartSmarts(paDailyFromQueueModel);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void SetTWInitialGBs_WhenSuccess()
        {
            // Arrange
            var paDailyFromQueueModel = new PADailyScanFromQueueModel();
            
            // Act
            Action action = () => paPotentialService.SetTWInitialSmartSmarts(paDailyFromQueueModel);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void BuildPAPotentialInfo_WhenSuccess()
        {
            // Arrange
            var problems = new List<ProblemAccountLosingClassificationEntity>();

            // Act
            var result = paPotentialService.BuildPAPotentialInfo(problems);

            // Assert
            Assert.IsAssignableFrom<string>(result);
        }
    }
}
