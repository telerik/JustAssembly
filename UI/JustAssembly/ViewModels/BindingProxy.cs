using System.Windows;

namespace JustAssembly.ViewModels
{
    // This class is used to make the DataContext property available in to GridViewColumn. This is needed because the GridViewColumn doesn't
    // inherit the DataContext property by default, because it's not part of the visual tree. Also, because it's not part of the visual tree,
    // it cannot access parent's (GridView) DataContext. See also http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
