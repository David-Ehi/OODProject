using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OODProject
{
    /// <summary>
    /// Interaction logic for CreateCharWindow.xaml
    /// </summary>
    public partial class CreateCharWindow : Window
    {



        public CreateCharWindow()
        {
            InitializeComponent();
        }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Api();
        }

        public async Task Api()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.dnd5eapi.co/api/2014/classes/");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            Root AllClasses = JsonConvert.DeserializeObject<Root>(body);
            
            ClassLbx.ItemsSource = AllClasses.results;
        }

        public async Task ClassInfoApi()
        {
            string dndclass = ClassLbx.SelectedItem.ToString().ToLower();
            
            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/classes/{dndclass}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            ClassinfoRoot Classinfo = JsonConvert.DeserializeObject<ClassinfoRoot>(body);

            UpdateCreationPage(Classinfo);

        }

        public void UpdateCreationPage(ClassinfoRoot Classinfo)
        {

            //SubclassLbx.ItemsSource = Classinfo.subclasses;
            //HitDieTblk.Text = Classinfo.hit_die.ToString();
            //if (Classinfo.proficiency_choices != null &&
            //Classinfo.proficiency_choices.Count > 0)
            //{
            //    ProfDesc.Text = Classinfo.proficiency_choices[0].desc;
            //}

            //AbilityScoreProfTxBlk.Text = null;
            //if (Classinfo.proficiencies != null)
            //{
            //    for (int i = 0; i < Classinfo.proficiencies.Count; i++)
            //    {
            //        AbilityScoreProfTxBlk.Text += Classinfo.proficiencies[i].name.ToString() + "\n";
            //    }
            //}
        }

        private void ClassLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SubclassLbx.ItemsSource = null;
            //ClassInfoApi();

            if (ClassLbx.SelectedItem == null) return;
            {
                string selectedClass = ClassLbx.SelectedItem.ToString(); //not sure if this is right, i couldnt get it work otherwise

                switch (selectedClass)
                {
                    case "Fighter":
                        ClassFrame.Navigate(new FighterPage());
                        break;
                }
            }
        }

        private void AddtoDBBtn_Click(object sender, RoutedEventArgs e)
        {
            //PlayerData db = new PlayerData();


            //string playername = PlayerNameTbx.Text;
            //string name = NameTbx.Text;
            //string dndclass = ClassLbx.SelectedItem as string;

            //Player player = new Player();
            //player.Name = name;



            //Characters character = new Characters();
            //character.Name = name;
            //character.Class = dndclass;


            //character.Player = player;
            //player.Characters.Add(character);
            
            //db.Characters.Add(character);
            //db.Players.Add(player);
            //db.SaveChanges();
        }
    }
}
