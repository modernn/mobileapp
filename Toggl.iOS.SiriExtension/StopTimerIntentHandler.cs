using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.iOS.Intents;
using Foundation;
using Toggl.Shared.Models;
using SiriExtension.Models;
using SiriExtension.Exceptions;
using Toggl.iOS.ExtensionKit;
using Toggl.iOS.ExtensionKit.Analytics;
using Toggl.iOS.ExtensionKit.Extensions;
using Toggl.Shared;

namespace SiriExtension
{
    public class StopTimerIntentHandler : StopTimerIntentHandling
    {
        private ITogglApi togglAPI;
        private static ITimeEntry runningEntry;
        private const string stopTimerActivityType = "StopTimer";

        public StopTimerIntentHandler(ITogglApi togglAPI)
        {
            this.togglAPI = togglAPI;
        }

        public override void ConfirmStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion)
        {
            if (togglAPI == null)
            {
                var userActivity = new NSUserActivity(stopTimerActivityType);
                userActivity.SetResponseText(Resources.SiriShortcutLoginToUseShortcut);
                completion(new StopTimerIntentResponse(StopTimerIntentResponseCode.FailureNoApiToken, userActivity));
                return;
            }

            var lastUpdated = SharedStorage.instance.GetLastUpdateDate();
            togglAPI.TimeEntries.GetAll()
                .Select(checkSyncConflicts(lastUpdated))
                .Select(getRunningTimeEntry)
                .Subscribe(
                    runningTE =>
                    {
                        runningEntry = runningTE;
                        var userActivity = new NSUserActivity(stopTimerActivityType);
                        userActivity.SetEntryDescription(runningTE.Description);
                        completion(new StopTimerIntentResponse(StopTimerIntentResponseCode.Ready, userActivity));
                    },
                    exception =>
                    {
                        SharedStorage.instance.AddSiriTrackingEvent(SiriTrackingEvent.Error(exception.Message));
                        completion(responseFromException(exception));
                    });
        }

        public override void HandleStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion)
        {
            SharedStorage.instance.SetNeedsSync(true);

            stopTimeEntry(runningEntry)
                .Subscribe(
                    stoppedTimeEntry =>
                    {
                        var timeSpan = TimeSpan.FromSeconds(stoppedTimeEntry.Duration ?? 0);

                        var response = string.IsNullOrEmpty(stoppedTimeEntry.Description)
                            ? StopTimerIntentResponse.SuccessWithEmptyDescriptionIntentResponseWithEntryDurationString(
                                durationStringForTimeSpan(timeSpan))
                            : StopTimerIntentResponse.SuccessIntentResponseWithEntryDescription(
                                stoppedTimeEntry.Description, durationStringForTimeSpan(timeSpan)
                            );
                        response.EntryStart = stoppedTimeEntry.Start.ToUnixTimeSeconds();
                        response.EntryDuration = stoppedTimeEntry.Duration;

                        SharedStorage.instance.AddSiriTrackingEvent(SiriTrackingEvent.StopTimer());

                        completion(response);
                    },
                    exception =>
                    {
                        SharedStorage.instance.AddSiriTrackingEvent(SiriTrackingEvent.Error(exception.Message));
                        completion(responseFromException(exception));
                    }
                );
        }

        private Func<List<ITimeEntry>, List<ITimeEntry>> checkSyncConflicts(DateTimeOffset lastUpdated)
        {
            return tes =>
            {
                // If there are no changes since last sync, or there are changes in the server but not in the app, we are ok
                if (tes.Count == 0 || tes.OrderBy(te => te.At).Last().At >= lastUpdated)
                {
                    return tes;
                }

                throw new AppOutdatedException();
            };
        }

        private string durationStringForTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.Hours == 0 && timeSpan.Minutes == 0)
            {
                return string.Format(Resources.SiriDurationWithSeconds, timeSpan.Seconds);
            }

            if (timeSpan.Hours == 0)
            {
                return string.Format(Resources.SiriDurationWithMinutesAndSeconds, timeSpan.Minutes, timeSpan.Seconds);
            }

            if (timeSpan.Minutes == 0)
            {
                return string.Format(Resources.SiriDurationWithHoursAndSeconds, timeSpan.Hours, timeSpan.Seconds);
            }

            return string.Format(Resources.SiriDurationWithHoursMinutesAndSeconds, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private ITimeEntry getRunningTimeEntry(IList<ITimeEntry> timeEntries)
        {
            try
            {
                var runningTE = timeEntries.Where(te => te.Duration == null).First();
                return runningTE;
            }
            catch
            {
                throw new NoRunningEntryException();
            }
        }

        private IObservable<ITimeEntry> stopTimeEntry(ITimeEntry timeEntry)
        {
            var duration = (long) (DateTime.Now - timeEntry.Start).TotalSeconds;
            return togglAPI.TimeEntries.Update(
                TimeEntry.from(timeEntry).with(duration)
            );
        }

        private StopTimerIntentResponse responseFromException(Exception exception)
        {
            var userActivity = new NSUserActivity(stopTimerActivityType);
            if (exception is NoRunningEntryException)
            {
                userActivity.SetResponseText(Resources.SiriNoCurrentEntryRunning);
                return new StopTimerIntentResponse(StopTimerIntentResponseCode.FailureNoTimerRunning, userActivity);
            }

            if (exception is AppOutdatedException)
            {
                userActivity.SetResponseText(Resources.SiriShortcutOpenTheAppToSync);
                return new StopTimerIntentResponse(StopTimerIntentResponseCode.FailureSyncConflict, userActivity);
            }

            userActivity.SetResponseText(Resources.SiriShortcutOpenTheAppToSync);
            return new StopTimerIntentResponse(StopTimerIntentResponseCode.Failure, userActivity);
        }
    }
}