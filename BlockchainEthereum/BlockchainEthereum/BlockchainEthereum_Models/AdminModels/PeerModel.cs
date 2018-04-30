using System.Collections.Generic;

namespace BlockchainEthereum_WebApi.Models
{
    public class PeerModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Caps { get; set; }
        public Network Network { get; set; }
        public Protocols Protocols { get; set; }
    }

    public class Network
    {
        public string LocalAddress { get; set; }
        public string RemoteAddress { get; set; }
        public bool Inbound { get; set; }
        public bool Trusted { get; set; }
        public bool Static { get; set; }
    }

    public class Protocols
    {
        public Eth Eth { get; set; }
    }

    public class Eth
    {
        public int Version { get; set; }
        public int Difficulty { get; set; }
        public string Head { get; set; }
    }
}