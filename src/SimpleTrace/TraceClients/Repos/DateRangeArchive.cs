//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;

//namespace Zonekey.EzTrace.TraceClients.Repos
//{
//    public interface IDateRange
//    {
//        DateTime Start { get; set; }
//        DateTime End { get; set; }
//    }

//    public class DateRangeArchive : IDateRange
//    {
//        private DateRangeArchive()
//        {
//        }

//        public DateTime Start { get; set; }
//        public DateTime End { get; set; }
//        public string DateFormat { get; set; }
//        public string ArchiveId { get; set; }

//        public static DateRangeArchive Create(DateTime start, DateTime end, string dateFormat)
//        {
//            var archive = new DateRangeArchive();
//            var archiveId = CreateArchiveId(start, end, dateFormat);
//            archive.ArchiveId = archiveId;
//            archive.Start = start;
//            archive.End = end;
//            archive.DateFormat = dateFormat;
//            return archive;
//        }
//        public static DateRangeArchive TryParse(string archiveId, string dateFormat)
//        {
//            if (string.IsNullOrWhiteSpace(archiveId))
//            {
//                return null;
//            }

//            var dates = archiveId.Split('_');
//            if (dates.Length != 2)
//            {
//                return null;
//            }

//            var start = TryParseDate(dates[0], dateFormat);
//            if (start == null)
//            {
//                return null;
//            }

//            var end = TryParseDate(dates[1], dateFormat);
//            if (end == null)
//            {
//                return null;
//            }

//            return Create(start.Value, end.Value, dateFormat);
//        }
//        //20190725060000_20190725070000
//        //private string dateFormat = "yyyyMMddHHmmss";
//        private static string CreateArchiveId(DateTime start, DateTime end, string dateFormat)
//        {
//            return string.Format("{0}_{1}",
//                start.ToString(dateFormat),
//                end.ToString(dateFormat));
//        }
//        private static DateTime? TryParseDate(string date, string dateFormat)
//        {
//            //var tryParse = DateTime.TryParse(date, out var result);
//            var tryParse = DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
//            if (tryParse)
//            {
//                return result;
//            }
//            return null;
//        }
//    }

//    public interface IDateRangeArchiveParser
//    {
//        string DateFormat { get; set; }
//        DateRangeArchive CreateDateRangeArchive(DateTime start, DateTime end);
//        DateRangeArchive TryParse(string archiveId);
//    }

//    public class HourDateRangeArchiveParser : IDateRangeArchiveParser
//    {
//        public HourDateRangeArchiveParser()
//        {
//            DateFormat = "yyyyMMddHH";
//        }
//        public string DateFormat { get; set; }
//        public DateRangeArchive CreateDateRangeArchive(DateTime start, DateTime end)
//        {
//            return DateRangeArchive.Create(start, start.AddHours(1), DateFormat);
//        }
//        public DateRangeArchive TryParse(string archiveId)
//        {
//            return DateRangeArchive.TryParse(archiveId, DateFormat);
//        }
//    }

//    public static class DateRangeArchiveParserExtensions
//    {
//        public static IDictionary<string, IList<TempSpan>> GroupArchives(this IDateRangeArchiveParser helper, IEnumerable<TempSpan> spans)
//        {
//            if (helper == null)
//            {
//                throw new ArgumentNullException(nameof(helper));
//            }

//            var dic = new ConcurrentDictionary<string, IList<TempSpan>>();
//            if (spans == null)
//            {
//                return dic;
//            }

//            var tuples = new List<Tuple<DateRangeArchive, TempSpan>>();
//            foreach (var span in spans)
//            {
//                var archive = helper.CreateDateRangeArchive(span.StartUtc, span.FinishUtc.Value);
//                var archiveId = archive.ArchiveId;
//                if (string.IsNullOrWhiteSpace(archiveId))
//                {
//                    continue;
//                }
//                var tuple = new Tuple<DateRangeArchive, TempSpan>(archive, span);
//                tuples.Add(tuple);
//            }

//            foreach (var tuple in tuples)
//            {
//                var archiveId = tuple.Item1.ArchiveId;
//                if (!dic.ContainsKey(archiveId))
//                {
//                    dic.TryAdd(archiveId, new List<TempSpan>());
//                }
//                dic[archiveId].Add(tuple.Item2);
//            }

//            return dic;
//        }


//        public static IEnumerable<DateRangeArchive> FilterDateRangeArchives(this IDateRangeArchiveParser helper, IEnumerable<DateRangeArchive> archiveFiles, LoadArgs args)
//        {
//            if (helper == null)
//            {
//                throw new ArgumentNullException(nameof(helper));
//            }

//            if (args == null || archiveFiles == null)
//            {
//                return Enumerable.Empty<DateRangeArchive>();
//            }

//            var result = archiveFiles.Where(archiveFile => InDateRange(helper, archiveFile, args.Begin, args.End));
//            return result;
//        }

//        public static bool InDateRange(this IDateRangeArchiveParser dateArchive, DateRangeArchive archive, DateTime? start, DateTime? end)
//        {
//            if (dateArchive == null)
//            {
//                throw new ArgumentNullException(nameof(dateArchive));
//            }

//            if (archive == null)
//            {
//                return false;
//            }

//            var inRange = true;
//            if (start != null)
//            {
//                if (archive.End < start.Value)
//                {
//                    inRange = false;
//                }
//            }

//            if (end != null)
//            {
//                if (archive.Start > end.Value)
//                {
//                    inRange = false;
//                }
//            }

//            return inRange;
//        }

//    }
//}
