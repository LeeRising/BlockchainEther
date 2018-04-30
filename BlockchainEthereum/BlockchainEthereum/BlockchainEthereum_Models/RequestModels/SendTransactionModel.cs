namespace BlockchainEthereum_Models.RequestModels
{
    public class SendTransactionModel : NodeUrlModel
    {
        public string AccountAddressSender { get; set; }
        public string AccountAddressReviever { get; set; }
        public uint Value { get; set; }
    }
}
