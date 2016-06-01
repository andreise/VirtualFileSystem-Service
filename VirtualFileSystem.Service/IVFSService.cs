using System.Runtime.Serialization;
using System.ServiceModel;

[assembly: ContractNamespace("http://andrey.sergeev.vfsservice", ClrNamespace = "VirtualFileSystem.Service")]

namespace VirtualFileSystem.Service
{

    /// <summary>
    /// Virtual File System Service
    /// </summary>
    //[ServiceContract(SessionMode = SessionMode.Required)]
    [ServiceContract()]
    public interface IVFSService
    {
        [OperationContract]
        [FaultContract(typeof(ConnectFault))]
        ConnectResponse Connect(ConnectRequest request);

        [OperationContract]
        [FaultContract(typeof(DisconnectFault))]
        DisconnectResponse Disconnect(DisconnectRequest request);

        [OperationContract]
        [FaultContract(typeof(FSCommandFault))]
        FSCommandResponse FSCommand(FSCommandRequest request);
    }

    [DataContract]
    public class BaseFault
    {
        [DataMember]
        public string FaultMessage { get; set; }
    }

    [DataContract]
    public class ConnectFault : BaseFault
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class DisconnectFault : BaseFault
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class FSCommandFault: BaseFault
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine;
    }

    [DataContract]
    public class ConnectRequest
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class ConnectResponse
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token { get; set; }

        [DataMember]
        public int TotalUsers { get; set; }
    }

    [DataContract]
    public class DisconnectRequest
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public byte[] Token;
    }

    [DataContract]
    public class DisconnectResponse
    {
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class FSCommandRequest
    {
        [DataMember]
        public string UserName;

        [DataMember]
        public byte[] Token;

        [DataMember]
        public string CommandLine;
    }

    [DataContract]
    public class FSCommandResponse
    {
        [DataMember]
        public string UserName;

        [DataMember]
        public string CommandLine;

        [DataMember]
        public string ResponseMessage;
    }

}
