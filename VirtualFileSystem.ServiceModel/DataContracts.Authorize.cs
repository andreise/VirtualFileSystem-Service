using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    public sealed class AuthorizeRequest
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public sealed class AuthorizeResponse
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }

        [DataMember]
        public int TotalUsers { get; set; }
    }

    [DataContract]
    public sealed class AuthorizeFault
    {
        [DataMember]
        public string UserName { get; set; }
    }

}
