namespace VFSClient.Model
{

    /// <summary>
    /// FSItem
    /// </summary>
    public interface IFSItem
    {
        /// <summary>
        /// FSItem Kind
        /// </summary>
        FSItemKind Kind { get; }

        /// <summary>
        /// FSItem Name
        /// </summary>
        string Name { get; }
    }

}
