using iText.Forms;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            if (CharLbx.SelectedItem == null)
            {
                MessageBox.Show("Please select a character to view.");
                return;
            }
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

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Characters selectedChar = CharLbx.SelectedItem as Characters;
            if (selectedChar == null)
            {
                MessageBox.Show("Please select a character to delete.");
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                $"Are you sure you want to delete {selectedChar.Name}?",
                "Confirm Delete",
                MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                using (var db = new PlayerData())
                {
                    Characters charToDelete = db.Characters.Find(selectedChar.CharacterId);
                    db.Characters.Remove(charToDelete);
                    db.SaveChanges();
                }
                LoadCharacters();
            }
        }

        private void EditDescBtn_Click(object sender, RoutedEventArgs e)
        {
            Characters selectedChar = CharLbx.SelectedItem as Characters;
            if (selectedChar == null)
            {
                MessageBox.Show("Please select a character first.");
                return;
            }

            string newDesc = Microsoft.VisualBasic.Interaction.InputBox( //found this method online, it opens a simple input dialog box
                "Enter a new description:",
                "Edit Description",
                selectedChar.Description);

            if (string.IsNullOrEmpty(newDesc)) return;

            using (var db = new PlayerData())
            {
                var charToUpdate = db.Characters.Find(selectedChar.CharacterId);
                charToUpdate.Description = newDesc;
                db.SaveChanges();
            }

            LoadCharacters();
            CharTbx.Text = newDesc;
        }

        private void ExportPdfBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
