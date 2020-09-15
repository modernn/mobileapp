using System;

using Qml.Net;
using Qml.Net.Runtimes;
using Toggl.Core.Models.Interfaces;

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
                    engine.SetContextProperty("toggl", toggl);
                    engine.Load("Main.qml");
                    return app.Exec();
                }
            }
        }
    }
}
