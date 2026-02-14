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
            //Test Push
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

            //Serialise the character to JSON and save it to a file
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
    }
}
