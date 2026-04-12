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
using System.Windows.Shapes;

namespace OODProject
{
    /// <summary>
    /// Interaction logic for CharFeatures.xaml
    /// </summary>
    public partial class CharFeatures : Window
    {
        private Characters character;
        public CharFeatures(Characters charactersFromMain)
        {
            InitializeComponent();
            character = charactersFromMain;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFeaturesForSelectedLevel();

        }





        private List<ClassLevel> _cachedLevels; //to make thing more efficient, we cache the levels for the selected class so we dont have to make multiple api calls when the user changes the level selection
        private async Task LoadClassLevels()
        {
            if (_cachedLevels != null) return;

            var client = new HttpClient();

            string body = await client.GetStringAsync(
                $"https://www.dnd5eapi.co/api/2014/classes/{character.Class.ToLower()}/levels");

            _cachedLevels = JsonConvert.DeserializeObject<List<ClassLevel>>(body);
                
        }

        private void LvlCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Featurelbx.ItemsSource = null; // Clear previous features
            LoadFeaturesForSelectedLevel();


        }
        private async void LoadFeaturesForSelectedLevel()
        {
            await LoadClassLevels(); // Ensure levels are loaded and cached

            int selectedLevel = character.Level;

            var totalFeatures = _cachedLevels //we use the cached levels to get the features for all levels up to the selected level, this way we dont have to make multiple api calls when the user changes the level selection
                .Where(l => l.level <= selectedLevel)
                .SelectMany(l => l.features)
                .ToList();

            Featurelbx.ItemsSource = totalFeatures;
        }

        private async void Featurelbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Feature selectedfeature = Featurelbx.SelectedItem as Feature;
            string featurename = Featurelbx.SelectedItem.ToString();
            await GetFeatureDesc(selectedfeature.index);


        }

        private async Task GetFeatureDesc(string index)
        {
            var client = new HttpClient();

            string body = await client.GetStringAsync(
                $"https://www.dnd5eapi.co/api/2014/features/{index}");
            
            Features featureDesc = JsonConvert.DeserializeObject<Features>(body);

            FeatureDescTbx.Text = string.Join("\n", featureDesc.desc); //the description is a list of strings, we join them together with newlines to display them in the textbox
        }

    }
}
