using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
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
            using (var db = new PlayerData()) //loads the current spells from database on character into listbox
            {
                var dbChar = db.Characters.Include("Spells").FirstOrDefault(c => c.CharacterId == character.CharacterId);
                foreach (var spell in dbChar.Spells)
                {
                    AddedSpellsLbx.Items.Add(new SpellResult { name = spell.Name, level = spell.Level });
                }
            }

            SpellLevelCbx.SelectedIndex = 0; //When the window is loaded, we set the selected index of the spell level combo box to 0 so that the user can see all spells by default
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
                AvailableSpellsLbx.Items.Add(spell); //We add each spell to the listbox so the user can select which spells they want to add to their character
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
                AvailableSpellsLbx.Items.Add(spell); //We add each spell to the listbox so the user can select which spells they want to add to their character
            }
        }

        private void AddSpellBtn_Click(object sender, RoutedEventArgs e)
        {
            SpellResult selectedSpell = AvailableSpellsLbx.SelectedItem as SpellResult; //We get the selected spell from the available spells listbox and add it to the added spells listbox so the user can see which spells they have added to their character
            if (selectedSpell != null && !AddedSpellsLbx.Items.Contains(selectedSpell))
            {
                AddedSpellsLbx.Items.Add(selectedSpell);
            }
            else
            {
                MessageBox.Show("Spell already added.");
            }
        }



        private void RemoveSpellBtn_Click(object sender, RoutedEventArgs e)
        {
            SpellResult selectedSpell = AddedSpellsLbx.SelectedItem as SpellResult; //We get the selected spell from the added spells listbox and remove it from the listbox so the user can see which spells they have removed from their character
            if (selectedSpell != null)
            {
                RemoveSelectedSpell();
                AddedSpellsLbx.Items.Remove(selectedSpell);
            }
            else
            {
                MessageBox.Show("No spell selected to remove.");
            }
        }

        private async void SaveSpellsBtn_Click(object sender, RoutedEventArgs e)
        {



            using (var db = new PlayerData())
            {
                var characterToUpdate = db.Characters.Find(character.CharacterId);
                foreach (SpellResult spell in AddedSpellsLbx.Items)
                {

                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/spells/{spell.index}");
                    request.Headers.Add("Accept", "application/json");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    var spellDetail = JsonConvert.DeserializeObject<SpellDetail>(body);

                    Spell existingSpell = db.Spells.FirstOrDefault(s => s.Name == spell.name);
                    if (existingSpell == null)
                    {
                        existingSpell = new Spell
                        {
                            Name = spell.name,
                            Level = spell.level,
                            Description = string.Join("\n", spellDetail.desc) 
                        };
                        db.Spells.Add(existingSpell);
                    }
                    if (!characterToUpdate.Spells.Any(s => s.Name == spell.name))
                    {
                        characterToUpdate.Spells.Add(existingSpell);
                    }
                }
                db.SaveChanges();
                MessageBox.Show("Spells saved to character!");
                SpellAdder.GetWindow(this).Close();
            }


        }
        private void RemoveSelectedSpell() 
        {
            SpellResult selected = AddedSpellsLbx.SelectedItem as SpellResult;
            if (selected == null) return;

            AddedSpellsLbx.Items.Remove(selected);

            using (var db = new PlayerData()) 
            {
                var dbChar = db.Characters.Include("Spells").FirstOrDefault(c => c.CharacterId == character.CharacterId);
                var spellToRemove = dbChar.Spells.FirstOrDefault(s => s.Name == selected.name);
                if (spellToRemove != null)
                    dbChar.Spells.Remove(spellToRemove);

                db.SaveChanges();
            }

        }
    }
}
