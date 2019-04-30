namespace RxFileSystemWatcher
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;

    /// <inheritdoc />
    public class FileDropWatcher : IDisposable, IFileDropWatcher
	{
		private readonly string _Path;
		private readonly string _Filter;
		private readonly ObservableFileSystemWatcher _Watcher;
		private readonly Subject<FileDropped> _PollResults = new Subject<FileDropped>();

		public IObservable<FileDropped> Dropped { get; private set; }

		public FileDropWatcher(string path, string filter)
		{
			_Path = path;
			_Filter = filter;
			_Watcher = new ObservableFileSystemWatcher(w =>
			{
				w.Path = path;
				w.Filter = filter;
				// note: filtering on changes can help reduce excessive notifications, make sure to verify any changes with integration tests
				w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
			});

			var renames = _Watcher.Renamed.Select(r => new FileDropped(r));
			var creates = _Watcher.Created.Select(c => new FileDropped(c));
			var changed = _Watcher.Changed.Select(c => new FileDropped(c));

			Dropped = creates
				.Merge(renames)
				.Merge(changed)
				.Merge(_PollResults);
		}

		public void Start()
		{
			_Watcher.Start();
		}

		public void Stop()
		{
			_Watcher.Stop();
		}

		public void Dispose()
		{
			_Watcher.Dispose();
		}

        /// <inheritdoc />
		public void PollExisting()
		{
            foreach (var existingFile in Directory.GetFiles(_Path, _Filter))
            {
                _PollResults.OnNext(new FileDropped(existingFile));
            }
        }
	}
}