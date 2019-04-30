namespace Tests
{
	using System.IO;
	using System.Reactive.Linq;
	using System.Reactive.Threading.Tasks;
	using System.Threading.Tasks;
	using NUnit.Framework;
	using RxFileSystemWatcher;

	[TestFixture]
	public class ObservableFileSystemWatcherTests : FileIntegrationTestsBase
	{
		[Test]
		[MaxTime(2000)]
		public async Task WriteToFile_StreamsChanged()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstChanged = watcher.Changed.FirstAsync().ToTask();
				watcher.Start();

				File.WriteAllText(Path.Combine(TempPath, "Changed.Txt"), "foo");

				var changed = await firstChanged;
                Assert.That(changed.ChangeType, Is.EqualTo(WatcherChangeTypes.Changed));
                Assert.That(changed.Name, Is.EqualTo("Changed.Txt"));
			}
		}

		[Test]
		[MaxTime(2000)]
		public async Task CreateFile_StreamsCreated()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstCreated = watcher.Created.FirstAsync().ToTask();
				var filePath = Path.Combine(TempPath, "Created.Txt");
				watcher.Start();

				File.WriteAllText(filePath, "foo");

				var created = await firstCreated;
                Assert.That(created.ChangeType, Is.EqualTo(WatcherChangeTypes.Created));
                Assert.That(created.Name, Is.EqualTo("Created.Txt"));
			}
		}

		[Test]
		[MaxTime(2000)]
		public async Task DeleteFile_StreamsDeleted()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstDeleted = watcher.Deleted.FirstAsync().ToTask();
				var filePath = Path.Combine(TempPath, "ToDelete.Txt");
				File.WriteAllText(filePath, "foo");
				watcher.Start();

				File.Delete(filePath);

				var deleted = await firstDeleted;
                Assert.That(deleted.ChangeType, Is.EqualTo(WatcherChangeTypes.Deleted));
                Assert.That(deleted.Name, Is.EqualTo("ToDelete.Txt"));
			}
		}

		[Test]
		[MaxTime(2000)]
		public async Task DeleteMonitoredDirectory_StreamsError()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstError = watcher.Errors.FirstAsync().ToTask();
				watcher.Start();

				Directory.Delete(TempPath);

				var error = await firstError;
                Assert.That(error.GetException().Message, Is.EqualTo("Access is denied"));
			}
		}

		[Test]
		[MaxTime(2000)]
		public async Task RenameFile_StreamsRenamed()
		{
			using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
			{
				var firstRenamed = watcher.Renamed.FirstAsync().ToTask();
				var originalPath = Path.Combine(TempPath, "Changed.Txt");
				File.WriteAllText(originalPath, "foo");
				watcher.Start();

				var renamedPath = Path.Combine(TempPath, "Renamed.Txt");
				File.Move(originalPath, renamedPath);

				var renamed = await firstRenamed;
                Assert.That(renamed.OldFullPath, Is.EqualTo(originalPath));
                Assert.That(renamed.FullPath, Is.EqualTo(renamedPath));
			}
		}
	}
}