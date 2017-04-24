using System.Runtime.Serialization;

namespace VirtualFileSystem.Service.Model
{

    // Callbacks

    [DataContract]
    internal sealed class FileSystemChangedData
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

    // Common

    [DataContract]
    public abstract class BaseFault
    {
        [DataMember]
        public string FaultMessage { get; set; }
    }

    // Connect

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

    // Disconnect

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

    // Command

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
    public sealed class FSCommandFault : BaseFault
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

}
