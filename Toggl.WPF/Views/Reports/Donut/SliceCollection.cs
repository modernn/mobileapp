using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Toggl.Shared;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;

namespace Toggl.WPF.Views.Reports.Donut
{
    public class SlicesCollection : IEnumerable<Slice>
    {
        private const float fullCircle = 360f;
        private readonly ImmutableList<Slice> slices;

        public SlicesCollection(ImmutableList<PercentageDecoratedSegment> percentageSegments)
        {
            if (percentageSegments.Count == 0)
            {
                this.slices = ImmutableList<Slice>.Empty;
                return;
            }

            var angleOffset = 0f;

            var slices = new List<Slice>();
            foreach (var percentageSegment in percentageSegments)
            {
                var percentage = (float)percentageSegment.NormalizedPercentage;
                var sweepAngle = fullCircle * percentage;

                slices.Add(new Slice(percentageSegment, angleOffset, sweepAngle));
                angleOffset += sweepAngle;
            }

            slices[^1].CorrectEndAngle();

            this.slices = slices.ToImmutableList();
        }

        public Slice GetSliceForPercentageValue(float angle)
        {
            angle = angle.NormalizedAngle();
            return slices.FirstOrDefault(s => s.ContainsAngle(angle));
        }

        public IEnumerator<Slice> GetEnumerator()
            => slices.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => slices.GetEnumerator();
    }
}
