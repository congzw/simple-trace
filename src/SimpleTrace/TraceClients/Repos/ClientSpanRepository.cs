using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace SimpleTrace.TraceClients.Repos
{
    public class ClientSpanRepository : IClientSpanRepository
    {
        private readonly AsyncFile _asyncFile;

        public ClientSpanRepository(AsyncFile asyncFile)
        {
            _asyncFile = asyncFile;
        }
        public Task Add(IList<ClientSpanEntity> spans)
        {
            if (spans == null || spans.Count == 0)
            {
                return 0.AsTask();
            }
            var content = spans.ToJson(false);

            var now = spans.Min(x => x.StartUtc);
            var archive = DateTimeRangeArchive.Create(now);
            var archiveName = archive.ArchiveId + ".log";

            var filePath = AppDomain.CurrentDomain.Combine("Trace", archiveName);
            return _asyncFile.AppendAllText(filePath, content, true);
        }

        public async Task<IList<ClientSpanEntity>> Read(LoadArgs args)
        {
            var folderPath = AppDomain.CurrentDomain.Combine("Trace");
            var logs = Directory.GetFiles(folderPath, "*.log");
            var fileInfos = logs.Select(x => new FileInfo(x)).ToList();
            var end = args.End ?? DateTime.MaxValue;
            var archives = GetArchives(fileInfos).Where(x => x.Overlaps(args.Begin, end)).ToList();
            
            var results = new List<ClientSpanEntity>();
            foreach (var archive in archives)
            {
                var filePath = AppDomain.CurrentDomain.Combine("Trace", archive.ArchiveId + ".log");
                var content = await _asyncFile.ReadAllText(filePath).ConfigureAwait(false);
                var result = content.FromJson<IList<ClientSpanEntity>>(null);
                if (result != null)
                {
                    results.AddRange(result);
                }
            }
            return results;
        }

        private IList<DateTimeRangeArchive> GetArchives(IList<FileInfo> fileInfos)
        {
            var archives = new List<DateTimeRangeArchive>();
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Exists)
                {
                    var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    var archive = DateTimeRangeArchive.TryParse(fileName);
                    if (archive != null)
                    {
                        archives.Add(archive);
                    }
                }
            }

            return archives;
        }

        private IList<DateTimeRangeArchive> GetArchives(IList<string> archiveIds)
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
    }

    //public interface IDateRangeArchive
    //{
    //    string CreateArchiveName(DateTime createAt);
    //    IList<string> CreateArchiveNames(DateTime start, DateTime end);
    //    bool IsInRange(string archiveName, DateTime start, DateTime end);
    //}

    //public class HourDateRangeArchive : IDateRangeArchive
    //{
    //    public string CreateArchiveName(DateTime createAt)
    //    {
    //        var startName = createAt.ToString("yyyyMMddHH");
    //        return startName + ".log";
    //    }

    //    public IList<string> CreateArchiveNames(DateTime start, DateTime end)
    //    {
    //        var archiveNames = new List<string>();
    //        var processing = true;

    //        var current = start;
    //        while (processing)
    //        {
    //            var archiveName = CreateArchiveName(current);
    //            archiveNames.Add(archiveName);

    //            current = current.AddHours(1);
    //            processing = IsInRange(archiveName, current, end);
    //        }

    //        return archiveNames;
    //    }

    //    public bool IsInRange(string archiveName, DateTime start, DateTime end)
    //    {
    //        if (string.IsNullOrWhiteSpace(archiveName))
    //        {
    //            return false;
    //        }

    //        var hourDateValue = archiveName.ToLower().Replace(".log", string.Empty);
    //        var tryParse = DateTime.TryParse(hourDateValue, out var hourDate);
    //        if (!tryParse)
    //        {
    //            return false;
    //        }

    //        var startHour = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
    //        var endHour = new DateTime(end.Year, end.Month, end.Day, end.Hour, 0, 0);
    //        return hourDate <= startHour && hourDate >= endHour;
    //    }
    //}

    //public interface IDateRangeArchiveFactory
    //{
    //    IDateRangeArchive Create();
    //}

    //public class DateRangeArchiveFactory : IDateRangeArchiveFactory
    //{
    //    public IDateRangeArchive Create()
    //    {
    //        //todo create by config
    //        return new HourDateRangeArchive();
    //    }


    //    #region for di extensions

    //    private static readonly Lazy<IDateRangeArchiveFactory> Instance = new Lazy<IDateRangeArchiveFactory>(() => new DateRangeArchiveFactory());
    //    public static Func<IDateRangeArchiveFactory> Resolve { get; set; } = () => Instance.Value;

    //    #endregion
    //}
}
