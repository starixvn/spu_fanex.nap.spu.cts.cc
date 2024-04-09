namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.TWGBTaggingClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class TWGBTaggingClassificationTest
    {
        private readonly ITWGBTaggingClassificationDataAccess _twGBTaggingClassificationDataAccess;
        private readonly IRealtimeClassificationDataAccess _realtimeClassificationDataAccess;
        private readonly ITWGBTaggingClassificationService _twGBTaggingClassificationService;

        public TWGBTaggingClassificationTest()
        {
            _twGBTaggingClassificationDataAccess = Substitute.For<ITWGBTaggingClassificationDataAccess>();
            _realtimeClassificationDataAccess = Substitute.For<IRealtimeClassificationDataAccess>();
            _twGBTaggingClassificationService = new TWGBTaggingClassificationService(_twGBTaggingClassificationDataAccess, _realtimeClassificationDataAccess);
        }

        #region GetChanges
        [Fact]
        public void TWGBTaggingGetChanges_WhenSuccess()
        {
            // Arrange

            // Act
            _twGBTaggingClassificationService.GetChanges();

            // Assert
            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();
        }

        [Fact]
        public void TWGBTaggingGetChanges_WhenException()
        {
            // Arrange

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Throws<Exception>();

            Action action = () => _twGBTaggingClassificationService.GetChanges();

            // Assert
            Assert.Throws<Exception>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();
        }
        #endregion

        #region UpdateNextScannedTime
        [Fact]
        public void TWGBTaggingUpdateNextScannedTime_WhenSuccess()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime();

            // Assert
            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(1).UpdateNextScannedTime(twGBTaggingCheckChange.NextScannedDateTime);
        }

        [Fact]
        public void TWGBTaggingUpdateNextScannedTime_WhenException()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _twGBTaggingClassificationDataAccess.When(x => x.UpdateNextScannedTime(twGBTaggingCheckChange.NextScannedDateTime)).Do(call => throw new Exception());

            Action action = () => _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime();

            // Assert
            Assert.Throws<Exception>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(1).UpdateNextScannedTime(Arg.Any<DateTime>());
        }
        #endregion

        #region Preprocess
        [Fact]
        public void TWGBTaggingPreprocess_WhenSuccess()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            var getCustomerCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long> { 1, 2 },
                CustomerPADailyScan = new List<long> { 3 }
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(getCustomerCategory);

            _twGBTaggingClassificationDataAccess.Preprocess(Arg.Any<string>());

            _twGBTaggingClassificationService.GetChanges().PreprocessWithBatch(1);

            // Assert
            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _realtimeClassificationDataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(4).Preprocess(Arg.Any<string>());
        }

        [Fact]
        public void TWGBTaggingPreprocess_WhenParameterEmpty()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            var getCustomerCategory = new GetCustomerCategoryRealtimeModel();

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(getCustomerCategory);

            _twGBTaggingClassificationService.GetChanges().PreprocessWithBatch(1);

            // Assert
            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _realtimeClassificationDataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(0).Preprocess(Arg.Any<string>());
        }

        [Fact]
        public void TWGBTaggingPreprocess_WhenException()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            var getCustomerCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long> { 1, 2 },
                CustomerPADailyScan = new List<long> { 3 }
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(getCustomerCategory);

            _twGBTaggingClassificationDataAccess.When(x => x.Preprocess(Arg.Any<string>())).Do(call => throw new Exception());

            Action action = () => _twGBTaggingClassificationService.GetChanges().PreprocessWithBatch(1);

            // Assert
            Assert.Throws<AggregateException>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _realtimeClassificationDataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(4).Preprocess(Arg.Any<string>());
        }
        #endregion

        #region TWGBTaggingClassification Flow
        [Fact]
        public void TWGBTaggingClassificationFlow_WhenSuccess()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            var getCustomerCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long> { 1, 2 },
                CustomerPADailyScan = new List<long> { 3 }
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _twGBTaggingClassificationDataAccess.UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(getCustomerCategory);

            _twGBTaggingClassificationDataAccess.Preprocess(Arg.Any<string>());

            _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime().PreprocessWithBatch(1);

            // Assert
            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(1).UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(4).Preprocess(Arg.Any<string>());
        }

        [Fact]
        public void TWGBTaggingClassificationFlow_WhenExceptionFromGetChange()
        {
            // Arrange

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Throws<Exception>();

            _twGBTaggingClassificationDataAccess.UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Preprocess(Arg.Any<string>());

            Action action = () => _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime().PreprocessWithBatch(1);

            // Assert
            Assert.Throws<Exception>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(0).UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.Received(0).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(0).Preprocess(Arg.Any<string>());
        }

        [Fact]
        public void TWGBTaggingClassificationFlow_WhenExceptionFromUpdateNextScannedTime()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _twGBTaggingClassificationDataAccess.When(x => x.UpdateNextScannedTime(Arg.Any<DateTime>())).Do(call => throw new Exception());

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Preprocess(Arg.Any<string>());

            Action action = () => _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime().PreprocessWithBatch(1);

            // Assert
            Assert.Throws<Exception>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(1).UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.Received(0).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(0).Preprocess(Arg.Any<string>());
        }

        [Fact]
        public void TWGBTaggingClassificationFlow_WhenExceptionFromGetCustomerCategory()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _twGBTaggingClassificationDataAccess.UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>()).Throws<Exception>();

            _twGBTaggingClassificationDataAccess.Preprocess(Arg.Any<string>());

            Action action = () => _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime().PreprocessWithBatch(1);

            // Assert
            Assert.Throws<AggregateException>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(1).UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(0).Preprocess(Arg.Any<string>());
        }

        [Fact]
        public void TWGBTaggingClassificationFlow_WhenExceptionFromPreprocess()
        {
            // Arrange
            var twGBTaggingCheckChange = new TWGBTaggingCheckChangeModel()
            {
                CustIDs = new List<long> { 0, 1, 2, 3 },
                NextScannedDateTime = new DateTime()
            };

            var getCustomerCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long> { 1, 2 },
                CustomerPADailyScan = new List<long> { 3 }
            };

            // Act
            _twGBTaggingClassificationDataAccess.GetChanges().Returns(twGBTaggingCheckChange);

            _twGBTaggingClassificationDataAccess.UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(getCustomerCategory);

            _twGBTaggingClassificationDataAccess.When(x => x.Preprocess(Arg.Any<string>())).Do(call => throw new Exception());

            Action action = () => _twGBTaggingClassificationService.GetChanges().UpdateNextScannedTime().PreprocessWithBatch(1);

            // Assert
            Assert.Throws<AggregateException>(action);

            _twGBTaggingClassificationDataAccess.Received(1).GetChanges();

            _twGBTaggingClassificationDataAccess.Received(1).UpdateNextScannedTime(Arg.Any<DateTime>());

            _realtimeClassificationDataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());

            _twGBTaggingClassificationDataAccess.Received(4).Preprocess(Arg.Any<string>());
        }
        #endregion
    }
}