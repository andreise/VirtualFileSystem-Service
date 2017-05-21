using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    internal sealed class CommandPerformedData
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string ResponseMessage { get; set; }
    }

}
