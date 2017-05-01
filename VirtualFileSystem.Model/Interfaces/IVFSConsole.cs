namespace VirtualFileSystem.Model
{

    public interface IVFSConsole
    {

        string MakeDirectory(string currentDirectory, string newDirectory);

        string ChangeDirectory(string currentDirectory, string directory);

        string RemoveDirectory(string currentDirectory, string directory);

        string DeleteTree(string currentDirectory, string directory);

        string MakeFile(string currentDirectory, string fileName);

        string DeleteFile(string currentDirectory, string fileName);

        string LockFile(string userName, string currentDirectory, string fileName);

        string UnlockFile(string userName, string currentDirectory, string fileName);

        void Copy(string currentDirectory, string sourcePath, string destPath);

        void Move(string currentDirectory, string sourcePath, string destPath);

        bool PrintTreeRoot { get; }

        string PrintTree();

    }

}
