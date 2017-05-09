using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    public sealed class DisconnectRequest
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }
    }

    [DataContract]
    public sealed class DisconnectResponse
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public sealed class DisconnectFault : BaseFault
    {
        [DataMember]
        public string UserName { get; set; }
    }

}
