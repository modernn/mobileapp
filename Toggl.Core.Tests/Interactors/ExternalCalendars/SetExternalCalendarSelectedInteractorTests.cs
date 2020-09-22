using System;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Calendar;
using Toggl.Core.Tests.Generators;
using Xunit;

namespace Toggl.Core.Tests.Interactors.ExternalCalendars
{
    public sealed class SetExternalCalendarSelectedInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource)
            {
                Action tryingToConstructWithNull = () =>
                    new SetExternalCalendarSelectedInteractor(
                        useDataSource ? DataSource : null,
                        0, true);

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [InlineData(0, true)]
            [InlineData(0, false)]
            [InlineData(42, true)]
            [InlineData(42, false)]
            [InlineData(1337, true)]
            [InlineData(1337, false)]
            public void UpdatesTheCalendar(long calendarId, bool selected)
            {
                var originalCalendar = new ExternalCalendar(calendarId, 0, "externalId", "calendar", selected, false);
                var updatedCalendar = new ExternalCalendar(calendarId, 0, "externalId", "calendar", !selected, true);

                DataSource.ExternalCalendars
                    .GetById(calendarId)
                    .Returns(Observable.Return(originalCalendar));

                var interactor = new SetExternalCalendarSelectedInteractor(DataSource, calendarId, !selected);
                interactor.Execute();

                DataSource.ExternalCalendars
                    .Received()
                    .Update(Arg.Is<IThreadSafeExternalCalendar>(
                        calendar => calendar.Id == updatedCalendar.Id && calendar.Selected == updatedCalendar.Selected && calendar.NeedsSync));
            }
        }
    }
}
