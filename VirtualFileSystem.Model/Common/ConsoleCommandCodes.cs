namespace VirtualFileSystem.Model
{

    /// <summary>
    /// Console Command Codes
    /// </summary>
    public enum ConsoleCommandCode
    {

        /// <summary>
        /// Make Directory Command
        /// </summary>
        MakeDirectory = 1,

        /// <summary>
        /// Make Directory Command
        /// </summary>
        MD = MakeDirectory,

        /// <summary>
        /// Change Directory Command
        /// </summary>
        ChangeDirectory,

        /// <summary>
        /// Change Directory Command
        /// </summary>
        CD = ChangeDirectory,

        /// <summary>
        ///  Remove Directory Command
        /// </summary>
        RemoveDirectory,

        /// <summary>
        ///  Remove Directory Command
        /// </summary>
        RD = RemoveDirectory,

        /// <summary>
        /// Delete Tree Command
        /// </summary>
        DeleteTree,

        /// <summary>
        /// Delete Tree Command
        /// </summary>
        DELTREE = DeleteTree,

        /// <summary>
        /// Make File Command
        /// </summary>
        MakeFile,

        /// <summary>
        /// Make File Command
        /// </summary>
        MF = MakeFile,

        /// <summary>
        /// Delete File Command
        /// </summary>
        DeleteFile,

        /// <summary>
        /// Delete File Command
        /// </summary>
        DEL = DeleteFile,

        /// <summary>
        /// Lock File Command
        /// </summary>
        LockFile,

        /// <summary>
        /// Lock File Command
        /// </summary>
        LOCK = LockFile,

        /// <summary>
        /// Unlock File Command
        /// </summary>
        UnlockFile,

        /// <summary>
        /// Unlock File Command
        /// </summary>
        UNLOCK = UnlockFile,

        /// <summary>
        /// Copy File or Directory Command
        /// </summary>
        Copy,

        /// <summary>
        /// Copy File or Directory Command
        /// </summary>
        COPY = Copy,

        /// <summary>
        /// Move File or Directory Command
        /// </summary>
        Move,

        /// <summary>
        /// Move File or Directory Command
        /// </summary>
        MOVE = Move,

        /// <summary>
        /// Print Tree Command
        /// </summary>
        PrintTree,

        /// <summary>
        /// Print Tree Command
        /// </summary>
        PRINT = PrintTree
    }

}
