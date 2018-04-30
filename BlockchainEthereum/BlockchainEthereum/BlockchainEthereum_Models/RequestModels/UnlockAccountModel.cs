namespace BlockchainEthereum_Models.RequestModels
{
    public class UnlockAccountModel : NodeUrlModel
    {
        public string AccountAddress { get; set; }
        public string PassPhrase { get; set; }
    }
}