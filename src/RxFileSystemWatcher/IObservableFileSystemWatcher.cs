using System;
using System.IO;

namespace RxFileSystemWatcher
{
    /// <summary>
    ///     This is a wrapper around a file system watcher to use the Rx framework instead of event handlers to handle
    ///     notifications of file system changes.
    /// </summary>
    public interface IObservableFileSystemWatcher
    {
        IObservable<FileSystemEventArgs> Changed { get; }
        IObservable<RenamedEventArgs> Renamed { get; }
        IObservable<FileSystemEventArgs> Deleted { get; }
        IObservable<ErrorEventArgs> Errors { get; }
        IObservable<FileSystemEventArgs> Created { get; }
        void Start();
        void Stop();
    }
}
