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

    // TO DO
    // link api
    // save to characters to database instead of json file




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

            #region
            ////Test player for example
            //PlayerData db = new PlayerData();

            //using (db)
            //{
            //    // Create a test player
            //    Player TestPlayer = new Player();
            //    TestPlayer.Name = "David Ehiagwina";

            //    // Create a test character with full stats
            //    Characters TestCharacter = new Characters();
            //    TestCharacter.Name = "Arthas the Brave";
            //    TestCharacter.Description = "A courageous knight from the kingdom of Lordaeron. Known for his strength and honor.";
            //    TestCharacter.Class = "Paladin";
            //    TestCharacter.Level = 5;
            //    TestCharacter.Strength = 18;
            //    TestCharacter.Constitution = 16;
            //    TestCharacter.Wisdom = 14;
            //    TestCharacter.Intelligence = 12;
            //    TestCharacter.Dexterity = 13;
            //    TestCharacter.Charisma = 15;
            //    TestCharacter.HP = 45;
            //    TestCharacter.AC = 17;

            //    // Link character to the player
            //    TestCharacter.Player = TestPlayer;
            //    TestPlayer.Characters.Add(TestCharacter);

            //    // Add to database
            //    db.Players.Add(TestPlayer);
            //    db.Characters.Add(TestCharacter);

            //    db.SaveChanges();

            //    MessageBox.Show("Player and test character added to database!");
            //}

            #endregion

            //^^^^^ New test data code that adds a player and character to the database. Uncomment to use. ^^^^^

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

            this.Close();

            System.Windows.Application.Current.MainWindow = createWindow;
        }
    }
}
