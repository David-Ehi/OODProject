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
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.IO;

namespace OODProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            //Opens the character creation window when the select button is clicked
            OpenCharWindow();
        }

        List<Characters> characters = new List<Characters>();


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            LoadCharacters();


        }

        private void LoadCharacters()
        {
            //loads the characters from the database and displays them in the listbox
            List<Characters> characters = new List<Characters>();

            using (var db = new PlayerData())
            {

                characters = db.Characters
                               .Include("Player")
                               .ToList();
            }

            CharLbx.ItemsSource = characters;
        }

        //When the user selects a character from the listbox, display the description in the textbox
        private void CharLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Characters selectedChar = CharLbx.SelectedItem as Characters;

            if (selectedChar != null)
            {
                CharTbx.Text = selectedChar.Description;
            }
        }

        public void OpenCharWindow()
        {
            //opend the character window and passes the selected character to it
            Characters MainChar = CharLbx.SelectedItem as Characters;
            CharWindow charWindow = new CharWindow(MainChar);
            charWindow.Owner = this;
            charWindow.Show();
        }



        private void CreateCharBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateCharWindow createWindow = new CreateCharWindow();
            createWindow.Show();

        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadCharacters();
        }
    }
}
