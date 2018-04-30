namespace BlockchainEthereum_WebApi.Models
{
    public class AccountWithBalanceModel : AccountModel
    {
        public decimal Balance { get; set; }
    }

    public class AccountModel
    {
        public string Address { get; set; }
        public string NodeUrl { get; set; }
    }
}