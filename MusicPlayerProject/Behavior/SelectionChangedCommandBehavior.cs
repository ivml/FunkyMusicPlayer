using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MusicPlayerProject.Behavior
{
    public static class SelectionChangedCommandBehavior
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(SelectionChangedCommandBehavior), new PropertyMetadata(null, OnCommandChanged));

        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(SelectionChangedCommandBehavior), new PropertyMetadata(null));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector s = d as Selector;
            if (s != null)
            {
                s.SelectionChanged += OnSelection;
            }
        }

        private static void OnSelection(object sender, SelectionChangedEventArgs e)
        {
            Selector s = (Selector)sender;
            ICommand cmd = s.GetValue(SelectionChangedCommandBehavior.CommandProperty) as ICommand;
            object param = e.AddedItems.FirstOrDefault();
            if (param == null)
            {
                param = e.RemovedItems.First();
            }
            if (cmd != null && cmd.CanExecute(param))
            {
                cmd.Execute(param);
            }
        }
    }
}
