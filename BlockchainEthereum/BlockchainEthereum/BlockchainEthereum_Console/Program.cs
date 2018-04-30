using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BlockchainEthereum_Models.RequestModels;
using BlockchainEthereum_WebApi.Models;
using Newtonsoft.Json;

namespace BlockchainEthereum_Console
{
    class Program
    {
        private static HttpClient _client;
        private static string StringContentType => "application/json";

        private const string node1Acc1 = "0x95579ac8d54a87c00185581dd9e4cc33861ae993";
        private const string node1Acc2 = "0x755a43d535c2f2e31d2b98deb87dde952e5de9e9";
        private const string node2Acc1 = "0x2a72dbc15f788123119495b6a64e2030cbe6cae8";

        private static List<AccountWithBalanceModel> _accountList = new List<AccountWithBalanceModel>();

        static void Main(string[] args)
        {
            Console.ReadKey();

            _client = new HttpClient { BaseAddress = new Uri("http://localhost:60614/") };

            try
            {
                GetAccountList().Wait();
                //UnLockAccount("0x755a43d535c2f2e31d2b98deb87dde952e5de9e9", "2").Wait();
                SendTransaction(node1Acc1, node2Acc1, 0x50).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }

        private static async Task GetAccountList()
        {
            var endpoint = "api/EthereumController/GetAccountList";
            Console.WriteLine(endpoint);

            var response = await _client.GetAsync(endpoint);
            var str = await response.Content.ReadAsStringAsync();

            _accountList = JsonConvert.DeserializeObject<List<AccountWithBalanceModel>>(str);

            foreach (var account in _accountList)
                Console.WriteLine($"Address: {account.Address}\nBalance: {account.Balance}\n");

            Console.WriteLine();
        }

        private static async Task UnLockAccount(string accountAddress, string passPhrase)
        {
            var endpoint = "api/EthereumController/UnlockAccount";
            Console.WriteLine(endpoint);

            var jsonStr = JsonConvert.SerializeObject(new UnlockAccountModel
            {
                AccountAddress = accountAddress,
                PassPhrase = passPhrase
            });

            var response = await _client.PostAsync(endpoint, new StringContent(jsonStr, Encoding.UTF8, StringContentType));

            var str = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Account {accountAddress} unlocked status {JsonConvert.DeserializeObject(str)}");

            Console.WriteLine();
        }

        private static async Task SendTransaction(string accountAddressSender, string accountAddressReviever, uint value)
        {
            var endpoint = "api/EthereumController/SendTransaction";
            Console.WriteLine(endpoint);

            var jsonStr = JsonConvert.SerializeObject(new SendTransactionModel
            {
                AccountAddressSender = accountAddressSender,
                AccountAddressReviever = accountAddressReviever,
                NodeUrl = _accountList.FirstOrDefault(x=>x.Address == accountAddressSender)?.NodeUrl,
                Value = value
            });

            var response = await _client.PostAsync(endpoint, new StringContent(jsonStr, Encoding.UTF8, StringContentType));
            var str = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Account {accountAddressSender} send to {accountAddressReviever} value: {value}\nTransaction hash: {JsonConvert.DeserializeObject(str)}");
            Console.WriteLine();
            //foreach (var acc in _accountList)
            //{
            //    if (acc.Address == accountAddressReviever)
            //        continue;

            //    var jsonStr = JsonConvert.SerializeObject(new SendTransactionModel
            //    {
            //        AccountAddressSender = acc.Address,
            //        AccountAddressReviever = accountAddressReviever,
            //        NodeUrl = acc.NodeUrl,
            //        Value = value
            //    });

            //    var response = await _client.PostAsync(endpoint, new StringContent(jsonStr, Encoding.UTF8, StringContentType));
            //    var str = await response.Content.ReadAsStringAsync();

            //    Console.WriteLine($"Account {accountAddressSender} send to {accountAddressReviever} value: {value}\nTransaction hash: {JsonConvert.DeserializeObject(str)}");
            //    Console.WriteLine();
            //}
        }
    }
}