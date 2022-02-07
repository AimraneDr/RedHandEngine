using Editor.Utilities.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor.Utilities
{
    /// <summary>
    /// Interaction logic for DebuggerView.xaml
    /// </summary>
    public partial class DebuggerView : UserControl
    {
        public DebuggerView()
        {
            InitializeComponent();
        }

        

        private void OnClear_Btn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Clear();
        }

        private void ONMessageFilter_BtnClick(object sender, RoutedEventArgs e)
        {
            var filter = 0x0;
            if (ToggleInfo.IsChecked == true) filter |= (int)MessageType.Info;
            if (ToggleWaringss.IsChecked == true) filter |= (int)MessageType.Warning;
            if (ToggleErrors.IsChecked == true) filter |= (int)MessageType.Error;
            Logger.SetMessageFilter(filter);
        }
    }
}
