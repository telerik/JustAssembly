using System;
using System.Windows.Media;

namespace JustAssembly
{
	public interface IWindowView
	{
		string Title { get; }

		ImageSource Icon { get; }

		bool TrySetFocus();
		
		void OnShowWindow();

		event EventHandler SendCloseRequest;
	}
}
