namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWClassification;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class TWClassificationTest
    {
        private readonly ITWClassificationService _twClassificationService;

        private readonly ITWClassificationDataAccess _twClassificationDataAccess;

        public TWClassificationTest()
        {            
            _twClassificationDataAccess = Substitute.For<ITWClassificationDataAccess>();
            _twClassificationService = new TWClassificationService(_twClassificationDataAccess);
        }

        [Fact]        
        public void GetSpecialLicSubCCScan_WhenSuccess()
        {
            // Arrange
            var batchSize = 100;
            var expectedResult = new List<SpecialLicSubCCScanGetEntity>
            {
                new SpecialLicSubCCScanGetEntity { ID = 1, CustID = 1001 },
                new SpecialLicSubCCScanGetEntity { ID = 2, CustID = 1002 }
            };

            _twClassificationDataAccess.GetSpecialLicSubCCScan(batchSize).Returns(expectedResult);

            // Act
            var result = _twClassificationService.GetSpecialLicSubCCScan(batchSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Count, result.Count());
            Assert.Equal(expectedResult[0].ID, result.First().ID);
            Assert.Equal(expectedResult[0].CustID, result.First().CustID);
            Assert.Equal(expectedResult[1].ID, result.Last().ID);
            Assert.Equal(expectedResult[1].CustID, result.Last().CustID);
        }

        [Fact]
        public void GetSpecialLicSubCCScan_ThrowException()
        {
            // Arrange
            var batchSize = 100;
            var expectedException = new Exception("Database connection error");
            
            _twClassificationDataAccess.GetSpecialLicSubCCScan(batchSize).Returns(x => throw expectedException);

            // Act & Assert
            var exception = Assert.Throws<System.Exception>(() => _twClassificationService.GetSpecialLicSubCCScan(batchSize));

            Assert.Equal(expectedException.Message, exception.Message);
        }

        [Fact]       
        public void PreprocessSpecialLicSubCCScan_WhenSuccess()
        {
            // Arrange
                var custLicSubs = new List<SpecialLicSubCCScanGetEntity>
                {
                    new SpecialLicSubCCScanGetEntity { ID = 1, CustID = 1001 },
                    new SpecialLicSubCCScanGetEntity { ID = 2, CustID = 1002 },
                    new SpecialLicSubCCScanGetEntity { ID = 3, CustID = 1001 } // Duplicate CustID to test distinct
                };

                string capturedCustIdList = null;
                _twClassificationDataAccess
                    .When(x => x.PreprocessSpecialLicSubCCScan(Arg.Any<string>()))
                    .Do(x => capturedCustIdList = x.Arg<string>());

                // Act
                _twClassificationService.PreprocessSpecialLicSubCCScan(custLicSubs);

                // Assert
                Assert.Equal("1001,1002", capturedCustIdList);
                _twClassificationDataAccess.Received(1).PreprocessSpecialLicSubCCScan(Arg.Any<string>());
        }
       
        [Fact]
        public void PreprocessSpecialLicSubCCScan_ThrowException()
        {
            // Arrange
            var custLicSubs = new List<SpecialLicSubCCScanGetEntity>
            {
                new SpecialLicSubCCScanGetEntity { ID = 1, CustID = 1001 },
                new SpecialLicSubCCScanGetEntity { ID = 2, CustID = 1002 }
            };

            var expectedException = new Exception("Database connection error");

            _twClassificationDataAccess
                .When(x => x.PreprocessSpecialLicSubCCScan(Arg.Any<string>()))
                .Do(x => throw expectedException);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _twClassificationService.PreprocessSpecialLicSubCCScan(custLicSubs));

            Assert.Equal(expectedException.Message, exception.Message);
        }      

        [Fact]
        public void CompleteSpecialLicSubCCScan_WhenSuccess()
        {
            // Arrange
            var custLicSubs = new List<SpecialLicSubCCScanGetEntity>
            {
                new SpecialLicSubCCScanGetEntity { ID = 1, CustID = 1001 },
                new SpecialLicSubCCScanGetEntity { ID = 2, CustID = 1002 },
                new SpecialLicSubCCScanGetEntity { ID = 3, CustID = 1003 }
            };

            string capturedIds = null;
            _twClassificationDataAccess
                .When(x => x.CompleteSpecialLicSubCCScan(Arg.Any<string>()))
                .Do(x => capturedIds = x.Arg<string>());

            // Act
            _twClassificationService.CompleteSpecialLicSubCCScan(custLicSubs);

            // Assert
            Assert.Equal("1,2,3", capturedIds);
            _twClassificationDataAccess.Received(1).CompleteSpecialLicSubCCScan(Arg.Any<string>());
        }

        [Fact]
        public void CompleteSpecialLicSubCCScan_ThrowException()
        {
            // Arrange
            var custLicSubs = new List<SpecialLicSubCCScanGetEntity>
            {
                new SpecialLicSubCCScanGetEntity { ID = 1, CustID = 1001 },
                new SpecialLicSubCCScanGetEntity { ID = 2, CustID = 1002 }
            };

            var expectedException = new Exception("Database connection error");
            _twClassificationDataAccess
                .When(x => x.CompleteSpecialLicSubCCScan(Arg.Any<string>()))
                .Do(x => throw expectedException);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _twClassificationService.CompleteSpecialLicSubCCScan(custLicSubs));
            Assert.Equal(expectedException.Message, exception.Message);
        }
    }
}
