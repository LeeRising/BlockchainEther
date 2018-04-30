using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using BlockchainEthereum_Models.RequestModels;
using BlockchainEthereum_WebApi.Models;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace BlockchainEthereum_WebApi.Controllers
{
    [RoutePrefix("api/EthereumController")]
    public class EthereumController : ApiController
    {
        private const string MainNodeUrl = "http://192.168.1.141:8545/";
        private Web3Geth _web3Client;

        [Route("GetAccountList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountList()
        {
            _web3Client = new Web3Geth(MainNodeUrl);

            var accountList = new List<AccountWithBalanceModel>();

            var mainAccountList = await _web3Client.Personal.ListAccounts.SendRequestAsync();

            foreach (var account in mainAccountList)
            {
                var ballance = await _web3Client.Eth.GetBalance.SendRequestAsync(account);
                accountList.Add(new AccountWithBalanceModel
                {
                    Address = account,
                    NodeUrl = MainNodeUrl,
                    Balance = Web3.Convert.FromWei(ballance, 18)
                });
            }

            var peersResponse = await _web3Client.Admin.Peers.SendRequestAsync();
            var peerList = peersResponse.ToObject<List<PeerModel>>();

            foreach (var peerModel in peerList)
            {
                var peerRpcUrl = $"http://{peerModel.Network.RemoteAddress.Split(':')[0]}:8545/";
                _web3Client = new Web3Geth(peerRpcUrl);

                var peerAccountList = await _web3Client.Eth.Accounts.SendRequestAsync();

                foreach (var account in peerAccountList)
                {
                    var ballance = await _web3Client.Eth.GetBalance.SendRequestAsync(account);
                    accountList.Add(new AccountWithBalanceModel
                    {
                        Address = account,
                        NodeUrl = peerRpcUrl,
                        Balance = Web3.Convert.FromWei(ballance, 18)
                    });
                }
            }

            return Ok(accountList);
        }

        [Route("UnlockAccount")]
        [HttpPost]
        public async Task<IHttpActionResult> UnlockAccount(UnlockAccountModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                _web3Client = new Web3Geth($"http://{model.NodeUrl}:8545/");

                var isAccountUnlocked = await _web3Client.Personal.UnlockAccount.SendRequestAsync(model.AccountAddress, model.PassPhrase, 100);

                return Ok(isAccountUnlocked);
            }
            catch (Exception)
            {
                BadRequest("Somthing went wrong");
            }

            return Ok();
        }

        [Route("SendTransaction")]
        [HttpPost]
        public async Task<IHttpActionResult> SendTransaction(SendTransactionModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                _web3Client = new Web3Geth(model.NodeUrl);

                var transactionHash = await _web3Client.Eth.TransactionManager.SendTransactionAsync(model.AccountAddressSender, model.AccountAddressReviever, new HexBigInteger(model.Value));

                return Ok(transactionHash);
            }
            catch (Exception ex)
            {
                BadRequest("Somthing went wrong");
            }

            return Ok();
        }

        //var s = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(new TransactionInput("0xe94bfbeff6357bbe16c392cbfdce846a069090745f20dedab19cf240545f1da8", node3Acc, node1Acc, new HexBigInteger(900000), new HexBigInteger(5)));
        //var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, node1Acc, new HexBigInteger(900000), new HexBigInteger(5));

        //for (int i = 1; i < 400; i++)
        //{
        //    var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(i));

        //    foreach (var transaction in block.Transactions)
        //    {
        //        Console.WriteLine($"Hash {transaction.Value.Value}");
        //        Console.WriteLine($"From {transaction.From}");
        //        Console.WriteLine($"To {transaction.To}");
        //        Console.WriteLine($"Value {transaction.Value.Value}");
        //        Console.WriteLine($"BHash {transaction.BlockHash}");
        //        Console.WriteLine($"BNum {transaction.BlockNumber.Value}");

        //        Console.WriteLine();
        //    }

        //    //foreach (var transactionHash in block.TransactionHashes)
        //    //    Console.WriteLine(transactionHash);
        //}
    }
}