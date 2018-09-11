using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using JustAssembly.ViewModels;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;

namespace JustAssembly
{
    public class JustAssemblyBootstrapper : MefBootstrapper
	{
        private readonly string[] args;

        public JustAssemblyBootstrapper(string[] args)
        {
            this.args = args;
        }

		protected override DependencyObject CreateShell()
		{
            return new Shell(new ShellViewModel(args), args);
		}

		protected override void InitializeShell()
		{
			this.ComposeExportedValues();

			var shell = (Shell)this.Shell;

			base.InitializeShell();

			Application.Current.MainWindow = shell;

			Application.Current.MainWindow.Show();
		}

		private void ComposeExportedValues()
		{
		}

		protected override void ConfigureAggregateCatalog()
		{
			ConfigureServiceCatalogs();
		}

		private void ConfigureServiceCatalogs()
		{
			List<Type> services = new List<Type>();

			this.AggregateCatalog.Catalogs.Add(new TypeCatalog(services));
		}

		protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
		{
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            
            return mappings;
		}
	}
}