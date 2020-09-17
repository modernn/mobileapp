using System;

using Qml.Net;
using Qml.Net.Runtimes;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Models;

namespace Linux
{
    class Program
    {
        static int Main(string[] args)
        {
            var toggl = new Toggl();
            LinuxDependencyContainer.EnsureInitialized(toggl);
            toggl.Bootup();
            RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();
            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    // Register our new type to be used in Qml
                    Qml.Net.Qml.RegisterType<Toggl>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<IThreadSafeTimeEntry>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<ITimeEntry>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<QMLTimeEntry>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<QMLProject>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<QMLTask>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<QMLUser>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<QMLClient>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<QMLTag>("toggl", 1, 0);
                    Qml.Net.Qml.RegisterType<TimeEntriesViewModel>("toggl", 1, 0);
                    engine.SetContextProperty("toggl", toggl);
                    engine.Load("UI/MainWindow.qml");
                    return app.Exec();
                }
            }
        }
    }
}
