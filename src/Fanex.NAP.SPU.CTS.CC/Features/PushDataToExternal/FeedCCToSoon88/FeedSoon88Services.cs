namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.FeedCCToSoon88
{
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFeedSoon88Services
    {
        void FeedCCToSoon88(IEnumerable<LatestClassificationEntity> custsToMainDB, string url, string serviceName, bool isWriteLog);
    }

    public class FeedSoon88Services : IFeedSoon88Services
    {
        private readonly IFeedSoon88DataAccess feedSoon88DataAccess;
        private readonly ISentryClient sentryClient;

        public FeedSoon88Services(IFeedSoon88DataAccess feedSoon88DataAccess, ISentryClient sentryClient)
        {
            this.feedSoon88DataAccess = feedSoon88DataAccess;
            this.sentryClient = sentryClient;
        }

        public void FeedCCToSoon88(IEnumerable<LatestClassificationEntity> custsToMainDB, string url, string serviceName, bool isWriteLog)
        {
            if (custsToMainDB != null && custsToMainDB.Any())
            {
                var custClassList = custsToMainDB.Select(c => new CustomerClassEntity
                {
                    CustID = c.CustId,
                    CustomerClass = c.CustomerClass
                });

                FeedDataToSoon88(custClassList, url);
            }

            Utilities.LogSentryInfo(serviceName, "FeedSoon88 done", custsToMainDB, isWriteLog, sentryClient);
        }

        public void FeedDataToSoon88(IEnumerable<CustomerClassEntity> custClasses, string url)
        {
            var updateList = custClasses.Where(c => c.CustomerClass != -1);
            var deleteList = custClasses.Where(c => c.CustomerClass == -1);

            if (updateList.Any())
            {
                UpdateDataToSoon88(updateList, url);
            }

            if (deleteList.Any())
            {
                DeleteDataInSoon88(deleteList, url);
            }
        }

        private void UpdateDataToSoon88(IEnumerable<CustomerClassEntity> updateList, string url)
        {
            var result = feedSoon88DataAccess.GetAssociationWithCategory(updateList, false);

            var dataGroupByAccount = result.GroupBy(a => a.RegisterName);
            foreach (var groupAcc in dataGroupByAccount)
            {
                var soon88 = StatisticAccountByCategoryInS88(groupAcc.Where(c => c.IsSoon88.HasValue && c.IsSoon88.Value));
                var others = StatisticAccountByCategoryNotInS88(groupAcc.Where(c => c.IsSoon88.HasValue && !c.IsSoon88.Value));

                var tempData = new PreparedUpdateDataToSoon88ViewModel
                {
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = new
                    {
                        soon88,
                        others
                    }
                };

                var finalData = new UpdateDataToSoon88ViewModel
                {
                    reason = groupAcc.FirstOrDefault().CategoryName,
                    account = groupAcc.Key,
                    association = tempData.association,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Constants.FeedSoon88_Salt)
                };

                var returnedData = ApiHelper.PostWithProxy<string>(url, finalData);
                Task.WhenAll(returnedData);
            }
        }

        private void DeleteDataInSoon88(IEnumerable<CustomerClassEntity> deleteList, string url)
        {
            var result = feedSoon88DataAccess.GetAssociationWithCategory(deleteList, true);

            foreach (var item in result)
            {
                var tempData = new PreparedDeleteDataToSoon88ViewModel
                {
                    reason = item.CategoryName,
                    account = item.RegisterName
                };

                var finalData = new DeleteDataToSoon88ViewModel
                {
                    reason = item.CategoryName,
                    account = item.RegisterName,
                    signature = CreateMD5Hash(JsonConvert.SerializeObject(tempData) + Constants.FeedSoon88_Salt)
                };

                var returnedData = ApiHelper.PostWithProxy<string>(url, finalData);
                Task.WhenAll(returnedData);
            }
        }

        private static Dictionary<string, int> StatisticAccountByCategoryNotInS88(IEnumerable<AssociationWithCategoryEntity> assList)
        {
            var statResult = new Dictionary<string, int>();
            foreach (var item in assList.GroupBy(a => a.AssCategoryName))
            {
                statResult.Add(item.Key, item.Count());
            }

            return statResult;
        }

        private static Dictionary<string, IEnumerable<string>> StatisticAccountByCategoryInS88(IEnumerable<AssociationWithCategoryEntity> assList)
        {
            var statResult = new Dictionary<string, IEnumerable<string>>();
            foreach (var item in assList.GroupBy(a => a.AssCategoryName))
            {
                statResult.Add(item.Key, item.Select(b => b.AssRegisterName));
            }

            return statResult;
        }

        private static string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}