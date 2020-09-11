using System;
using System.Collections.Generic;
using Toggl.Networking.Models.Calendar;
using Toggl.Shared.Models.Calendar;
using Xunit;

namespace Toggl.Networking.Tests.Models.Calendar
{
    public sealed class ExternalCalendarsPageTests
    {
        public sealed class TheExternalCalendarsPageModel
        {
            private string validJson
                => "{\"calendars\":[{\"calendar_id\":1,\"external_id\":\"Cal-1\",\"name\":\"Personal\",\"selected\":true},{\"calendar_id\":2,\"external_id\":\"Cal-2\",\"name\":\"Work\",\"selected\":false}],\"next_page_token\":\"next_page\"}";

            private ExternalCalendarsPage validPage => new ExternalCalendarsPage
            {
                Calendars = new List<IExternalCalendar>
                {
                    new ExternalCalendar
                    {
                        Id = 1,
                        ExternalId = "Cal-1",
                        Name = "Personal",
                        Selected = true,
                    },
                    new ExternalCalendar
                    {
                        Id = 2,
                        ExternalId = "Cal-2",
                        Name = "Work",
                        Selected = false,
                    },
                },
                NextPageToken = "next_page",
            };

            [Fact, LogIfTooSlow]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validPage);
            }
        }
    }
}
