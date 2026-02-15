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
            //Test Character for example
            Characters Test = new Characters();
            Test.Name = "Test";
            Test.Description = "This is a test character";
            string fileName = "Characters.json";


            //Characters Test2 = new Characters();
            //Test.Name = "David";
            //Test.Description = "This is a test character";

            //Serialise the character to JSON and save it to a file             Commented out to prevent overwriting the file every time the programs is run, this was how i made the json originally, now i just edit the json file directly to add characters until i make the character creation screen
            //string jsonString = JsonSerializer.Serialize(Test);
            //File.WriteAllText(fileName, jsonString);

            string textFromFile = File.ReadAllText(fileName);
            List<Characters> deserializedCharacters = JsonSerializer.Deserialize<List<Characters>>(textFromFile);
            foreach (Characters character in deserializedCharacters)
            {
                characters.Add(character);
            }

            //characters.Add(Test)
            CharLbx.ItemsSource = characters;


        }

        //When the user selects a character from the listbox, display the description in the textbox
        private void CharLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Characters selectedChar = CharLbx.SelectedItem as Characters;

            CharTbx.Text = selectedChar.Description;
        }

        public void OpenCharWindow()
        {
            //opend the character window and passes the selected character to it
            Characters MainChar = CharLbx.SelectedItem as Characters;
            CharWindow charWindow = new CharWindow(MainChar);
            charWindow.Owner = this;
            charWindow.Show();
        }

    }
}
