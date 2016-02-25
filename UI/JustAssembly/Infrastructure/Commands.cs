using System;
using System.Linq;
using Microsoft.Practices.Prism.Commands;

namespace JustAssembly.Infrastructure
{
    public static class Commands
    {
        public static CompositeCommand TabItemCloseCommand = new CompositeCommand();

        public static CompositeCommand CloseAllButThisCommand = new CompositeCommand();

        public static CompositeCommand CloseAllCommand = new CompositeCommand();
    }
}
