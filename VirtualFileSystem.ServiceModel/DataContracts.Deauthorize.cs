using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    public sealed class DeauthorizeRequest
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }
    }

    [DataContract]
    public sealed class DeauthorizeResponse
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public sealed class DeauthorizeFault
    {
        [DataMember]
        public string UserName { get; set; }
    }

}
