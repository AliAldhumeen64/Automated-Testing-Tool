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
    
    public partial class UserControlCreate : UserControl
    {
        public UserControlCreate()
        {
            InitializeComponent();

            List<Commands> items = new List<Commands>();
            items.Add(new Commands() { Name = "Command One", Description = "Something" });
            items.Add(new Commands() { Name = "Command Two", Description = "Something" });
            items.Add(new Commands() { Name = "Command Three", Description = "Something" });
            items.Add(new Commands() { Name = "Command Four", Description = "Something" });
            lvUsers.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
        }
    }

    //Object command to test
    public class Commands
    {
        public string Name { get; set; }

        public String Description { get; set; }
    }
}
