using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    public sealed class FSCommandRequest
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

    [DataContract]
    public sealed class FSCommandResponse
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CurrentDirectory { get; set; }

        [DataMember]
        public string CommandLine { get; set; }

        [DataMember]
        public string ResponseMessage { get; set; }
    }

    [DataContract]
    public sealed class FSCommandFault
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

}
