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
using Newtonsoft.Json;

namespace OODProject
{
    /// <summary>
    /// Interaction logic for SpellAdder.xaml
    /// </summary>
    public partial class SpellAdder : Window
    {
        private Characters character;

        public SpellAdder(Characters character)
        {

            InitializeComponent();
            this.character = character; //We set the character variable to the character passed from the previous window so we can add spells to that character

            
        }

        private static readonly HttpClient client = new HttpClient();
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetAllSpells(); //When the window is loaded, we call the GetAllSpells method to get all the spells for the selected class and display them in the listbox
        }


        public async Task GetAllSpells()
        {

            AvailableSpellsLbx.Items.Clear(); //We clear the listbox before adding the spells to it so we dont have duplicates when the user changes the level selection
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/classes/{character.Class}/levels/{character.Level}/spells");
            request.Headers.Add("Accept", "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var spells = JsonConvert.DeserializeObject<SpellListRoot>(body); //We deserialize the json response from the api call into a list of spells

            foreach (var spell in spells.results)
            {
                AvailableSpellsLbx.Items.Add(spell + " (Level " + spell.level + ")"); //We add each spell to the listbox so the user can select which spells they want to add to their character
            }


        }

        private void SpellLevelCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
            if (SpellLevelCbx.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLevel = selectedItem.Content.ToString();
                if (selectedLevel == "All Spells")
                {
                    // Call method to get all spells
                    _ = GetAllSpells();
                }
                else if (selectedLevel == "Cantrip")
                {
                    // Call method to get cantrips (level 0 spells)
                    _ = GetSpellLevelFilter(0);
                }
                else if (int.TryParse(selectedLevel, out int level))
                {
                    // Call method to get spells of the selected level
                    _ = GetSpellLevelFilter(level);
                }
            }
        }
        public async Task GetSpellLevelFilter(int level)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/spells?level={level}&classes={character.Class}");
            request.Headers.Add("Accept", "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var spells = JsonConvert.DeserializeObject<SpellListRoot>(body); //We deserialize the json response from the api call into a list of spells
            AvailableSpellsLbx.Items.Clear();
            
            foreach (var spell in spells.results)
            {
                AvailableSpellsLbx.Items.Add(spell + " (Level " + spell.level + ")"); //We add each spell to the listbox so the user can select which spells they want to add to their character
            }
        }

        private void AddSpellBtn_Click(object sender, RoutedEventArgs e)
        {
            var spell = AvailableSpellsLbx.SelectedItem;
            
        }



        private void RemoveSpellBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddedSpellsLbx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void AvailableSpellsLbx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void SaveSpellsBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
