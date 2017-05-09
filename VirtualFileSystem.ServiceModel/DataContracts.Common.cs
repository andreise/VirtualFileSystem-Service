using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
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

    // Faults

    [DataContract]
    public abstract class BaseFault
    {
        [DataMember]
        public string FaultMessage { get; set; }
    }

}
