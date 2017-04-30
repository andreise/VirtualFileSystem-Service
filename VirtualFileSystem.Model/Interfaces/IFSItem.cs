using System;
using System.Collections.Generic;

namespace VirtualFileSystem.Model
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
        /// <exception cref="ArgumentException">
        /// Throws if this item and new parent item is the same item.
        /// </exception>
        IFSItem Parent { get; set; }

        /// <summary>
        /// Child Items
        /// </summary>
        IReadOnlyCollection<IFSItem> ChildItems { get; }

        /// <summary>
        /// Gets zero-based item level in the file system hierarchy
        /// </summary>
        /// <returns>Zero-based item level in the file system hierarchy</returns>
        int GetLevel();

        /// <summary>
        /// Locks Item
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <exception cref="ArgumentNullException">Throws if user name is null</exception>
        /// <exception cref="ArgumentException">Throws if user name is empty</exception>
        /// <exception cref="InvalidOperationException">Throws if item is not a file or if a file already locked by the specified user</exception>
        void Lock(string userName);

        /// <summary>
        /// Unlock Item
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <exception cref="ArgumentNullException">Throws if user name is null</exception>
        /// <exception cref="ArgumentException">Throws if user name is empty</exception>
        /// <exception cref="InvalidOperationException">Throws if item is not a file or if a file is not locked by the specified user</exception>
        void Unlock(string userName);

        /// <summary>
        /// User list which blocked the item
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if item is not a file</exception>
        IReadOnlyCollection<string> LockedBy { get; }

        /// <summary>
        /// Adds child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child directory or file with same name already exists,
        /// or if the item kind is not volume or directory
        /// </exception>
        /// <returns>Added child directory</returns>
        IFSItem AddChildDirectory(string name);

        /// <summary>
        /// Removes empty child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child item is not a directory, or if child directory is not empty,
        /// or if the item kind is not volume or directory, or if a child item with the specified name is not exists
        /// </exception>
        void RemoveChildDirectory(string name);

        /// <summary>
        /// Removes empty child directory
        /// </summary>
        /// <param name="name">Directory name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child item is not a directory
        /// or if the item kind is not volume or directory, or if a child item with the specified name is not exists
        /// </exception>
        void RemoveChildDirectoryWithTree(string name);

        /// <summary>
        /// Adds child file
        /// </summary>
        /// <param name="name">File name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child directory or file with same name already exists,
        /// or if the item kind is not volume or directory
        /// </exception>
        /// <returns>Added child file</returns>
        IFSItem AddChildFile(string name);

        /// <summary>
        /// Removes child file
        /// </summary>
        /// <param name="name">File name</param>
        /// <exception cref="InvalidOperationException">
        /// Throws if child item is not a file, or if the item kind is not volume or directory,
        /// or if a child item with the specified name is not exists,
        /// or if a child item with the specified name is locked
        /// </exception>
        void RemoveChildFile(string name);

        /// <summary>
        /// Adds child item
        /// </summary>
        /// <param name="child">Child item</param>
        /// <exception cref="ArgumentNullException">Throws if the child item is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if the item is not a volume or a directory, or if the child item is not a directory or a file,
        /// or if the item already have a child with the same name
        /// </exception>
        void AddChild(IFSItem child);

        /// <summary>
        /// Remove child item
        /// </summary>
        /// <param name="child">Child item</param>
        /// <exception cref="ArgumentNullException">Throws if the child item is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if the item is not a volume or a directory,
        /// or if the child item is not a directory or a file,
        /// or if the child item is a locked file,
        /// or if the item do not contain the specified child
        /// </exception>
        void RemoveChild(IFSItem child);

    }

}
