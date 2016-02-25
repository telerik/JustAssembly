using System;
using System.Windows;
using System.Windows.Threading;

namespace JustAssembly
{
    public static class DispatcherObjectExt
    {
        public static void BeginInvoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            Application.Current.Dispatcher.BeginInvoke(action, priority);
        }

        public static void Invoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            Application.Current.Dispatcher.Invoke(action, priority);
        }
    }
}
