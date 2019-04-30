using System;

namespace RxFileSystemWatcher
{
    /// <summary>
    ///     An observable abstraction to monitor for files dropped into a directory
    /// </summary>
    public interface IFileDropWatcher
    {
        IObservable<FileDropped> Dropped { get; }
        void Start();
        void Stop();

        /// <summary>
        ///     Use this to scan for files and raise dropped events for any results.
        ///     This is great to use right after starting the watcher to find existing files.
        ///     Existing files will trigger dropped events through the Dropped stream.
        /// </summary>
        void PollExisting();
    }
}
