using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace VirtualFileSystem.Service
{
    interface IVFSServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void FileSystemChangedNotify(FileSystemChangedData data);

    }

    [DataContract]
    public class FileSystemChangedData
    {
        [DataMember]
        public string UserName;

        [DataMember]
        public string CommandLine;
    }
}
