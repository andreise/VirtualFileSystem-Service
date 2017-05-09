using System.Runtime.Serialization;

namespace VirtualFileSystem.ServiceModel
{

    [DataContract]
    internal sealed class FileSystemChangedData
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CommandLine { get; set; }
    }

}
