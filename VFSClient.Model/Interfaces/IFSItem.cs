using System.Collections.Generic;

namespace VFSClient.Model
{

    /// <summary>
    /// File System Item
    /// </summary>
    public interface IFSItem
    {
        /// <summary>
        /// Item Kind
        /// </summary>
        FSItemKind Kind { get; }

        /// <summary>
        /// Item Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parent Item
        /// </summary>
        IFSItem Parent { get; }

        /// <summary>
        /// Child Items
        /// </summary>
        IReadOnlyCollection<IFSItem> ChildItems { get; }
    }

}
