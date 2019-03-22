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

namespace NavigationDrawerPopUpMenu2
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //Let the user drag the window
        private void MovesOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void exit_btn(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("The system is now closing");
            this.Close();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    usc = new UserControlHome();
                    GridMain.Children.Add(usc);
                    break;
                case "ItemCreate":
                    usc = new UserControlCreate();
                    
                    GridMain.Children.Add(usc);
                    break;
                case "ItemImport":
                    usc = new UserControlImport();
                    GridMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }
    }
}
