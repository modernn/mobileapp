using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Calendar;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Toggl.Storage.Models.Calendar;
using Xunit;

namespace Toggl.Core.Tests.Interactors.ExternalCalendars
{
    public class PushSelectedExternalCalendarsInteractorTests : BaseInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useApi)
            {
                Action tryingToConstructWithNull = () =>
                    new PushSelectedExternalCalendarsInteractor(
                        useDataSource ? DataSource : null,
                        useApi ? Api : null);

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            public sealed class WhenItSucceeds : BaseInteractorTests
            {
                [Fact, LogIfTooSlow]
                public async Task ItSavesTheUpdatedCalendars()
                {
                    var originalCalendar = new ExternalCalendar(0, 0, "0", "calendar", true, true);
                    var updatedCalendar = new ExternalCalendar(0, 0, "0", "calendar", true, false);

                    DataSource.ExternalCalendars
                        .GetAll(Arg.Any<Func<IDatabaseExternalCalendar, bool>>()).Returns(Observable.Return(new [] { originalCalendar }));

                    Api.ExternalCalendars
                        .SetCalendarSelected(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>())
                        .Returns(updatedCalendar);

                    var interactor = new PushSelectedExternalCalendarsInteractor(DataSource, Api);
                    await interactor.Execute();

                    DataSource.ExternalCalendars.Received().Update(Arg.Is<IThreadSafeExternalCalendar>(calendar => calendar.Id == updatedCalendar.Id && !calendar.NeedsSync));
                }
            }

            public sealed class WhenItFails : BaseInteractorTests
            {
                [Fact, LogIfTooSlow]
                public async Task ItIgnoresApiExceptions()
                {
                    var originalCalendar = new ExternalCalendar(0, 0, "0", "calendar", true, true);

                    DataSource.ExternalCalendars
                        .GetAll(Arg.Any<Func<IDatabaseExternalCalendar, bool>>()).Returns(Observable.Return(new [] { originalCalendar }));

                    var req = Substitute.For<IRequest>();
                    req.Headers.Returns(new HttpHeader[0]);

                    var res = Substitute.For<IResponse>();
                    res.StatusCode.Returns(HttpStatusCode.Forbidden);
                    res.IsJson.Returns(false);
                    res.RawData.Returns("Deleted integration");

                    var exception = new ForbiddenException(req, res);

                    Api.ExternalCalendars
                        .SetCalendarSelected(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>())
                        .ReturnsThrowingTaskOf(exception);

                    var interactor = new PushSelectedExternalCalendarsInteractor(DataSource, Api);

                    await interactor.Execute();

                    DataSource.ExternalCalendars.DidNotReceive().Update(Arg.Any<IThreadSafeExternalCalendar>());
                }

                [Fact, LogIfTooSlow]
                public async Task ItThrowsForUnhandledExceptions()
                {
                    var originalCalendar = new ExternalCalendar(0, 0, "0", "calendar", true, true);

                    DataSource.ExternalCalendars
                        .GetAll(Arg.Any<Func<IDatabaseExternalCalendar, bool>>()).Returns(Observable.Return(new [] { originalCalendar }));

                    Api.ExternalCalendars
                        .SetCalendarSelected(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>())
                        .ReturnsThrowingTaskOf(new Exception("Something bad happened"));

                    var interactor = new PushSelectedExternalCalendarsInteractor(DataSource, Api);

                    Action tryingToExecute = () =>
                        interactor.Execute().Wait();

                    tryingToExecute.Should().Throw<Exception>();
                }
            }
        }
    }
}
