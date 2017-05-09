using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    public sealed class FileSystemConsoleRequest
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

    [DataContract]
    public sealed class FileSystemConsoleResponse
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
    public sealed class FileSystemConsoleFault
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

}
