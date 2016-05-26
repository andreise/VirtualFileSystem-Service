using System;

namespace VFSClient.Model
{

    /// <summary>
    /// File System Item
    /// </summary>
    internal abstract class FSItem : IFSItem
    {
        /// <summary>
        /// FSItem Kind
        /// </summary>
        public FSItemKind Kind { get; }

        /// <summary>
        /// FSItem Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        protected virtual void ValidateName(string name)
        {
            if ((object)name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Length == 0)
                throw new ArgumentException(nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="kind">FSItem Kind</param>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the kind is incorrect</exception>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty</exception>
        public FSItem(FSItemKind kind, string name)
        {
            if (!Enum.IsDefined(typeof(FSItemKind), kind))
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "FSItem kind is incorrect.");

            this.ValidateName(name);

            this.Kind = kind;

            this.Name = name;
        }
    }

    /// <summary>
    /// File System Volume
    /// </summary>
    internal class FSVolume : FSItem
    {

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid volume name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidVolumeName(name))
                throw new ArgumentException("The name is not a valid volume name.", nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Volume Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid volume name</exception>
        public FSVolume(string name) : base(FSItemKind.Volume, name)
        {
        }

    }

    /// <summary>
    /// File System Directory
    /// </summary>
    internal class FSDirectory : FSItem
    {

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid directory name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidDirectoryName(name))
                throw new ArgumentException("The name is not a valid directory name.", nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Directory Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid directory name</exception>
        public FSDirectory(string name) : base(FSItemKind.Directory, name)
        {
        }

    }

    /// <summary>
    /// File System File
    /// </summary>
    internal class FSFile : FSItem
    {

        /// <summary>
        /// Validates name
        /// </summary>
        /// <param name="name">FSItem Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file name</exception>
        protected override void ValidateName(string name)
        {
            base.ValidateName(name);

            if (!FSPath.IsValidFileName(name))
                throw new ArgumentException("The name is not a valid file name.", nameof(name));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">File Name</param>
        /// <exception cref="ArgumentNullException">Throws if the name is null</exception>
        /// <exception cref="ArgumentException">Throws if the name is empty or is not a valid file name</exception>
        public FSFile(string name) : base(FSItemKind.File, name)
        {
        }

    }

}
