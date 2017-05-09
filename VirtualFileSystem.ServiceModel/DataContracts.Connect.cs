using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    public sealed class ConnectRequest
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public sealed class ConnectResponse
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }

        [DataMember]
        public int TotalUsers { get; set; }
    }

    [DataContract]
    public sealed class ConnectFault : BaseFault
    {
        [DataMember]
        public string UserName { get; set; }
    }

}
