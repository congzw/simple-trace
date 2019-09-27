using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.TraceClients;

namespace SimpleTrace.Server.CallApis
{
    public class ClientSpanRepos
    {
        private readonly AsyncFile _asyncFile;

        public ClientSpanRepos(AsyncFile asyncFile)
        {
            _asyncFile = asyncFile;
        }

        public async Task<IList<IClientSpan>> Read(LoadArgs args)
        {
            var archives = GetArchives(args);

            var results = new List<IClientSpan>();
            foreach (var archive in archives)
            {
                var filePath = CreateFilePath(archive.ArchiveId);
                var jsonLines = await _asyncFile.ReadAllLines(filePath).ConfigureAwait(false);
                if (jsonLines != null)
                {
                    foreach (var jsonLine in jsonLines)
                    {
                        var result = jsonLine.FromJson<IList<ClientSpan>>(null);
                        if (result != null)
                        {
                            results.AddRange(result);
                        }
                    }
                }
            }
            return results;
        }

        public Task Clear(LoadArgs args)
        {
            var archives = GetArchives(args);

            var tasks = new List<Task>();
            foreach (var archive in archives)
            {
                var filePath = CreateFilePath(archive.ArchiveId);
                tasks.Add(_asyncFile.Delete(filePath));
            }
            return Task.WhenAll(tasks);
        }

        private IList<DateTimeRangeArchive> GetArchives(LoadArgs args)
        {
            var begin = DateTime.MinValue;
            var end = DateTime.MaxValue;
            if (args != null)
            {
                if (args.Begin.HasValue)
                {
                    begin = args.Begin.Value;
                }
                if (args.End.HasValue)
                {
                    end = args.End.Value;
                }
            }

            var folderPath = AppDomain.CurrentDomain.Combine("Trace");
            if (!Directory.Exists(folderPath))
            {
                return new List<DateTimeRangeArchive>();
            }
            var logs = Directory.GetFiles(folderPath, "*.log");
            var archiveIds = logs.Select(x => Path.GetFileNameWithoutExtension(new FileInfo(x).Name)).ToList();
            var archives = ParseArchives(archiveIds).FilterOverlaps(begin, end).ToList();
            return archives;
        }

        private IList<DateTimeRangeArchive> ParseArchives(IList<string> archiveIds)
        {
            var archives = new List<DateTimeRangeArchive>();
            foreach (var archiveId in archiveIds)
            {
                var archive = DateTimeRangeArchive.TryParse(archiveId);
                if (archive != null)
                {
                    archives.Add(archive);
                }
            }
            return archives;
        }

        private string CreateFilePath(string archiveId)
        {
            var archiveName = archiveId + ".log";
            var filePath = AppDomain.CurrentDomain.Combine("Trace", archiveName);
            return filePath;
        }
    }

    public class LoadArgs
    {
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
    }
}