using System;

namespace Common
{
    public interface IDateTimeRange
    {
        DateTime Begin { get; }
        DateTime End { get; }
    }

    public static class DateRangeExtensions
    {
        /// <summary>
        /// 查看两个时间段是否重叠
        /// </summary>
        /// <param name="dateTimeRange"></param>
        /// <param name="another"></param>
        /// <param name="overlapsInclude"></param>
        /// <returns></returns>
        public static bool Overlaps(this IDateTimeRange dateTimeRange, IDateTimeRange another, OverlapsInclude overlapsInclude = OverlapsInclude.None)
        {
            switch (overlapsInclude)
            {
                case OverlapsInclude.None:
                    return dateTimeRange.Begin < another.End && dateTimeRange.End > another.Begin;
                case OverlapsInclude.Begin:
                    return dateTimeRange.Begin <= another.End && dateTimeRange.End > another.Begin;
                case OverlapsInclude.End:
                    return dateTimeRange.Begin < another.End && dateTimeRange.End >= another.Begin;
                case OverlapsInclude.Both:
                    return dateTimeRange.Begin <= another.End && dateTimeRange.End >= another.Begin;
                default:
                    throw new ArgumentOutOfRangeException(nameof(overlapsInclude), overlapsInclude, null);
            }
        }
    }

    public enum OverlapsInclude
    {
        None = 0,
        Begin = 0x01,
        End = 0x10,
        Both = 0x11
    }

    public class DateTimeRange : IDateTimeRange
    {
        protected DateTimeRange() { }
        public DateTimeRange(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }
        public DateTimeRange(DateTime begin, TimeSpan duration)
        {
            Begin = begin;
            End = begin.Add(duration);
        }

        public DateTime Begin { get; private set; }
        public DateTime End { get; private set; }

        public DateTimeRange NewEnd(DateTime newEnd)
        {
            return new DateTimeRange(this.Begin, newEnd);
        }
        public DateTimeRange NewDuration(TimeSpan newDuration)
        {
            return new DateTimeRange(this.Begin, newDuration);
        }
        public DateTimeRange NewBegin(DateTime newBegin)
        {
            return new DateTimeRange(newBegin, this.End);
        }

        public static DateTimeRange Create(DateTime begin, DateTime end)
        {
            return new DateTimeRange(begin, end);
        }
    }
}
