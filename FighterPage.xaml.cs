using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace OODProject
{
    /// <summary>
    /// Interaction logic for FighterPage.xaml
    /// </summary>
    public partial class FighterPage : Page
    {
        static string selectedClass = "fighter"; // This should be set based on user selection

        public FighterPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await FighterClassApi();
        }

        public async Task FighterClassApi()
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/classes/fighter");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            ClassinfoRoot Classinfo = JsonConvert.DeserializeObject<ClassinfoRoot>(body);

            UpdateCreationPage(Classinfo);

        }


        public void UpdateCreationPage(ClassinfoRoot Classinfo)
        {
            // Hit Die
            HitDieTxbl.Text = $"Hit Die: d{Classinfo.hit_die}";

            // Saving Throws
            SavingThrowsTxt.Text = "";
            foreach (var save in Classinfo.saving_throws)
            {
                SavingThrowsTxt.Text += save.name + "\n";
            }



            // Subclasses
            if (Classinfo.subclasses != null)
            {
                SubclassCbBx.ItemsSource = Classinfo.subclasses;
            }
        }
        private void RollStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            StrTbx.Text = RollStat().ToString();
            DexTbx.Text = RollStat().ToString();
            ConTbx.Text = RollStat().ToString();
            IntTbx.Text = RollStat().ToString();
            WisTbx.Text = RollStat().ToString();
            ChaTbx.Text = RollStat().ToString();
        }
        public int RollStat()
        {
            Random rnd = new Random();

            List<int> rolls = new List<int>();

            for (int i = 0; i < 4; i++)
                rolls.Add(rnd.Next(1, 7));

            rolls.Sort();
            rolls.RemoveAt(0);

            return rolls.Sum();
        }

        private void RollHpBtn_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();

            int hp = rnd.Next(1, 11); // Fighter d10
            HpTbx.Text = hp.ToString();
        }

        public async Task<List<Feature>> GetFeaturesForLevel(string className, int level) //gets all features for a given class and level, used to populate the listbox with the features of the selected class and level
        {
            var client = new HttpClient();

            string body = await client.GetStringAsync(
                $"https://www.dnd5eapi.co/api/2014/classes/{className}/levels");

            List<ClassLevel> levels =
                JsonConvert.DeserializeObject<List<ClassLevel>>(body);

            ClassLevel levelData = levels.FirstOrDefault(l => l.level == level);

            if (levelData != null)
                return levelData.features;

            return new List<Feature>();
        }

        private async void LvlCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ClassFeaturelbx.ItemsSource = null; // Clear previous features
            int selectedLevel = LvlCbx.SelectedIndex + 1; // Assuming levels start at 1 and ComboBox is zero-indexed

            List<Feature> totalfeature = new List<Feature>();

            for (int i = 0; i < selectedLevel; i++)
                #region getfeaturesforlevel
                if (LvlCbx.SelectedItem is ComboBoxItem item) // Checks if the selected item is a ComboBoxItem, which it should be in this case since we're populating it with ComboBoxItems in XAML
                {

                    var features = await GetFeaturesForLevel(selectedClass, i + 1);

                    totalfeature.AddRange(features);

                }
            ClassFeaturelbx.ItemsSource = totalfeature;

        }

    }
    
}
