using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable CheckNamespace

namespace Common
{
    [TestClass]
    public class DateTimeRangeSpec
    {
        private readonly DateTime _mockNow = new DateTime(2000, 1, 1, 0, 0, 0);

        [TestMethod]
        public void Overlaps_IncludeNone_ShouldOk()
        {
            TestOverlaps(OverlapsInclude.None, _mockNow);
        }

        [TestMethod]
        public void Overlaps_IncludeBegin_ShouldOk()
        {
            TestOverlaps(OverlapsInclude.Begin, _mockNow);
        }

        [TestMethod]
        public void Overlaps_IncludeEnd_ShouldOk()
        {
            TestOverlaps(OverlapsInclude.End, _mockNow);
        }

        [TestMethod]
        public void Overlaps_IncludeBoth_ShouldOk()
        {
            TestOverlaps(OverlapsInclude.Both, _mockNow);
        }
        
        private void TestOverlaps(OverlapsInclude overlapsInclude, DateTime now)
        {
            //current   =>          |****|
            //1         => |****|
            //2         =>      |****|
            //3         =>           |**|
            //4         =>              |****|
            //5         =>                  |****|
            //6         =>     |***>|
            //7         =>               |<***|

            var current = DateTimeRange.Create(now.AddHours(1), now.AddHours(2));

            var dateTimeRange1 = DateTimeRange.Create(_mockNow.AddHours(0), _mockNow.AddHours(0.5));
            var dateTimeRange2 = DateTimeRange.Create(_mockNow.AddHours(0.5), _mockNow.AddHours(1.5));
            var dateTimeRange3 = DateTimeRange.Create(_mockNow.AddHours(1.2), _mockNow.AddHours(1.6));
            var dateTimeRange4 = DateTimeRange.Create(_mockNow.AddHours(1.6), _mockNow.AddHours(2.5));
            var dateTimeRange5 = DateTimeRange.Create(_mockNow.AddHours(2.5), _mockNow.AddHours(3));
            var dateTimeRange6 = DateTimeRange.Create(_mockNow.AddHours(0.5), _mockNow.AddHours(1));
            var dateTimeRange7 = DateTimeRange.Create(_mockNow.AddHours(2), _mockNow.AddHours(3));
            current.Overlaps(dateTimeRange1, overlapsInclude).ShouldFalse();
            current.Overlaps(dateTimeRange2, overlapsInclude).ShouldTrue();
            current.Overlaps(dateTimeRange3, overlapsInclude).ShouldTrue();
            current.Overlaps(dateTimeRange4, overlapsInclude).ShouldTrue();
            current.Overlaps(dateTimeRange5, overlapsInclude).ShouldFalse();
            if (overlapsInclude == OverlapsInclude.Begin || overlapsInclude == OverlapsInclude.Both)
            {
                current.Overlaps(dateTimeRange6, overlapsInclude).ShouldTrue();
            }
            else
            {
                current.Overlaps(dateTimeRange6, overlapsInclude).ShouldFalse();
            }


            if (overlapsInclude == OverlapsInclude.End || overlapsInclude == OverlapsInclude.Both)
            {
                current.Overlaps(dateTimeRange7, overlapsInclude).ShouldTrue();
            }
            else
            {
                current.Overlaps(dateTimeRange7, overlapsInclude).ShouldFalse();
            }
        }
    }
}
