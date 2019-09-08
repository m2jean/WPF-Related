using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using System;
using System.Threading;
using System.Windows;

namespace BenchmarkUI
{
    /// <summary>
    /// An App class that can be run in BenchmarkDotNet without congesting the dispatcher
    /// </summary>
    [Config(typeof(Config))]
    public partial class App : Application
    {
        MainWindow window = null;
        public App()
        {
            Thread t = new Thread(() =>
            {
                window = new MainWindow();
                window.Title = "2";
                window.Show();
                window.Closed += (sender, args) => window.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                System.Windows.Threading.Dispatcher.Run();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;

            Exit += (_1, _2) =>
            {
                window?.Dispatcher.BeginInvoke(new Action(()=>window?.Close()));
                window?.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                t.Join();
            };

            t.Start();
        }

        [GlobalSetup]
        [STAThread]
        public void GlobalSetup()
        {

        }

        [Benchmark]
        [STAThread]
        public void Test()
        {
            Thread.Sleep(100);
        }


        private class Config : ManualConfig
        {
            public Config()
            {
                Add(new ManualConfig().With(ConfigOptions.DisableOptimizationsValidator));
            }
        }
    }
}
