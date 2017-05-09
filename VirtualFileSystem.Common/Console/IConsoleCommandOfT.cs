namespace VirtualFileSystem.Common.Console
{

    /// <summary>
    /// Console Command
    /// </summary>
    public interface IConsoleCommand<TCommandCodeEnum> : IConsoleCommand where TCommandCodeEnum : struct
    {

        /// <summary>
        /// Command Code
        /// </summary>
        TCommandCodeEnum? CommandCode { get; }

    }

}
