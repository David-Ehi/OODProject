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
    /// Interaction logic for CharWindow.xaml
    /// </summary>
    public partial class CharWindow : Window
    {
        private Characters character;
        Random rand = new Random();
        public CharWindow(Characters charactersFromMain)
        {
            InitializeComponent();
            //Sets the character variable to the character passed from the main window
            character = charactersFromMain;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (character.IsSpellCaster == false)
            {
                SpellColumn.Width = new GridLength(0); //If the character is not a spellcaster, we set the width of the spell column to 0 so that it is not visible
                SpellNameColumn.Width = new GridLength(0);
            }

            if (character.IsSpellCaster == true)
            {
                LoadSpellLbx();
            }

            //Loads the characters information into the labels and textboxes
            CharNameLabel.Text = $"{character.Name} ({character.Class} {character.Level})";
            MaxHpLbl.Content = character.HP.ToString();
            HpTxBx.Text = character.HP.ToString();

            AcTxbl.Text = character.AC.ToString();
            ProfBonusTxbl.Text = "+" + character.ProficencyBonus.ToString();

            // initiative is just the DEX modifier
            int initiativeMod = (character.Dexterity - 10) / 2;
            InitiativeTxbl.Text = initiativeMod >= 0 ? $"+{initiativeMod}" : initiativeMod.ToString();

            HpBar.Maximum = character.HP;
            HpBar.Value = character.HP;

            character.Notes = character.Notes ?? ""; // Ensure Notes is not null to avoid issues when displaying or saving notes
            NotesTbx.Text = character.Notes;
            LoadAbilityScores();
            SkillsLbBx.ItemsSource = CalculateSkills();
            LoadSpellSlots();
            LoadClassFeatures();
        }

        private void LoadAbilityScores()
        {
            //Loads the characters ability scores into the listbox
            AbilitiesLbBx.Items.Add("STR: " + character.Strength);
            AbilitiesLbBx.Items.Add("DEX: " + character.Dexterity);
            AbilitiesLbBx.Items.Add("CON: " + character.Constitution);
            AbilitiesLbBx.Items.Add("WIS: " + character.Wisdom);
            AbilitiesLbBx.Items.Add("INT: " + character.Intelligence);
            AbilitiesLbBx.Items.Add("CHA: " + character.Charisma);
        }

        private void D20Btn_Click(object sender, RoutedEventArgs e)
        {
            //Simulates rolling a d20 and shows the result in a message box
            int roll = rand.Next(1, 21);
            MessageBox.Show($"You rolled a {roll}");
        }

        private void D12Btn_Click(object sender, RoutedEventArgs e)
        {
            int roll = rand.Next(1, 13);
            MessageBox.Show($"You rolled a {roll}");
        }

        private void D10Btn_Click(object sender, RoutedEventArgs e)
        {
            int roll = rand.Next(1, 11);
            MessageBox.Show($"You rolled a {roll}");
        }

        private void D8Btn_Click(object sender, RoutedEventArgs e)
        {
            int roll = rand.Next(1, 9);
            MessageBox.Show($"You rolled a {roll}");
        }

        private void D6Btn_Click(object sender, RoutedEventArgs e)
        {
            int roll = rand.Next(1, 7);
            MessageBox.Show($"You rolled a {roll}");
        }

        private void D4Btn_Click(object sender, RoutedEventArgs e)
        {
            int roll = rand.Next(1, 4);
            MessageBox.Show($"You rolled a {roll}");
        }


        private void SpellEditor_Click(object sender, RoutedEventArgs e)
        {
            SpellAdder window = new SpellAdder(character);
            window.Owner = this;
            window.Show();
        }

        private void HpTxBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(HpTxBx.Text, out int currentHp))
            {
                if (currentHp < 0)
                {
                    currentHp = 0;

                }
                if (currentHp > character.HP)
                {
                    currentHp = character.HP;

                }
                HpBar.Value = currentHp;

            }
        }
        public int GetModifier(int abilityScore)
        {
            return (abilityScore - 10) / 2;
        }

        public List<string> CalculateSkills() //god this took ages to figure out, this method calculates the skill modifiers for the character based on their ability scores and proficiencies, it returns a list of strings that can be displayed in the UI
        {
            int prof = character.ProficencyBonus;

            List<string> proficientSkills;
            if (character.Skills != null)
            {
                proficientSkills = character.Skills.Split(',').ToList();
            }
            else
            {
                proficientSkills = new List<string>();
            }

            string Format(string name, int mod)
            {
                bool isProficient = proficientSkills.Contains(name);
                int total = mod;
                if (isProficient)
                {
                    total += prof;
                }
                string sign = total >= 0 ? "+" : "";

                string marker;
                if (isProficient)
                {
                    marker = "●"; //stole the icons off the internet, the filled circle means proficient and the empty circle means not proficient

                }
                else
                {
                    marker = "○";
                }
                return $"{marker} {name}: {sign}{total}";
            }

            int STR = GetModifier(character.Strength);
            int DEX = GetModifier(character.Dexterity);
            int INT = GetModifier(character.Intelligence);
            int WIS = GetModifier(character.Wisdom);
            int CHA = GetModifier(character.Charisma);

            return new List<string>
            {
                Format("Athletics", STR),

                Format("Acrobatics", DEX),
                Format("Sleight of Hand", DEX),
                Format("Stealth", DEX),

                Format("Arcana", INT),
                Format("History", INT),
                Format("Investigation", INT),
                Format("Nature", INT),
                Format("Religion", INT),

                Format("Animal Handling", WIS),
                Format("Insight", WIS),
                Format("Medicine", WIS),
                Format("Perception", WIS),
                Format("Survival", WIS),

                Format("Deception", CHA),
                Format("Intimidation", CHA),
                Format("Performance", CHA),
                Format("Persuasion", CHA),
            };
        }

        private void AddSpellBtn_Click(object sender, RoutedEventArgs e)
        {
            SpellAdder window = new SpellAdder(character);
            window.Owner = this;
            window.Show();
        }

        private void RefreshSpellBtn_Click(object sender, RoutedEventArgs e)
        {
            SpellsLbx.Items.Clear();

            LoadSpellLbx();

        }
        public void LoadSpellLbx()
        {
            using (var db = new PlayerData())
            {
                character = db.Characters
                              .Include("Spells")
                              .Include("Player")
                              .FirstOrDefault(c => c.CharacterId == character.CharacterId);
            }
            character.Spells.ToList().ForEach(s => SpellsLbx.Items.Add(s.Name + $" (Level {s.Level})")); //Loads the characters spells into the listbox
        }

        public void LoadSpellSlots()
        {
            Slot1Txbl.Text = character.SpellSlotsLevel1.ToString();
            Slot2Txbl.Text = character.SpellSlotsLevel2.ToString();
            Slot3Txbl.Text = character.SpellSlotsLevel3.ToString();
            Slot4Txbl.Text = character.SpellSlotsLevel4.ToString();
            Slot5Txbl.Text = character.SpellSlotsLevel5.ToString();
            Slot6Txbl.Text = character.SpellSlotsLevel6.ToString();
            Slot7Txbl.Text = character.SpellSlotsLevel7.ToString();
            Slot8Txbl.Text = character.SpellSlotsLevel8.ToString();
            Slot9Txbl.Text = character.SpellSlotsLevel9.ToString();
        }

        private void SpellsLbx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            Spell selectedspell = character.Spells.FirstOrDefault(s => s.Name + $" (Level {s.Level})" == SpellsLbx.SelectedItem.ToString());
            PopupSpellDetailTxt.Text = selectedspell.Description;
            SpellDescPopup.IsOpen = true;
            PopupSpellNameTxt.Text = SpellsLbx.SelectedItem.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SpellDescPopup.IsOpen = false;
        }

        private void SkillsLbBx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RollSkillCheck();
        }

        public void RollSkillCheck()
        {
            string selectedSkill = SkillsLbBx.SelectedItem.ToString();
            int skillModifier = int.Parse(GetNumbers(selectedSkill));
            MessageBox.Show($"You rolled a {rand.Next(1, 21) + skillModifier} on {selectedSkill}");
        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        private async void ClassFeatureLbx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClassFeatureLbx.SelectedItem == null) return;

            PopupNameTxt.Text = (ClassFeatureLbx.SelectedItem as Feature).name;

            PopupDetailTxt.Text = $"Feature Index: {(ClassFeatureLbx.SelectedItem as Feature).index}\n" +
                $"URL: {(ClassFeatureLbx.SelectedItem as Feature).url}";
            await GetFeatureDesc((ClassFeatureLbx.SelectedItem as Feature).index); // we get the feature description using the index of the selected feature, this way we can display the description in the popup

            FeatureDescPopup.IsOpen = true;
        }

        public async Task GetFeatureDesc(string index)
        {
            var client = new HttpClient();

            string body = await client.GetStringAsync(
                $"https://www.dnd5eapi.co/api/2014/features/{index}");

            Features featureDesc = JsonConvert.DeserializeObject<Features>(body);

            PopupDetailTxt.Text = string.Join("\n", featureDesc.desc); //the description is a list of strings, we join them together with newlines to display them in the textbox
        }


        public async void LoadClassFeatures()
        {
            await LoadClassLevels(); // Ensure levels are loaded and cached

            int selectedLevel = character.Level;

            var totalFeatures = _cachedLevels //we use the cached levels to get the features for all levels up to the selected level, this way we dont have to make multiple api calls when the user changes the level selection
                .Where(l => l.level <= selectedLevel)
                .SelectMany(l => l.features)
                .ToList();

            ClassFeatureLbx.ItemsSource = totalFeatures;
        }
        private static readonly HttpClient client = new HttpClient(); // we use a static http client to make the api calls, this way we can reuse the same client for multiple calls and avoid the overhead of creating a new client for each call

        private List<ClassLevel> _cachedLevels; //to make thing more efficient, we cache the levels for the selected class so we dont have to make multiple api calls when the user changes the level selection
        private async Task LoadClassLevels()
        {
            if (_cachedLevels != null) return;


            string body = await client.GetStringAsync(
                $"https://www.dnd5eapi.co/api/2014/classes/{character.Class.ToLower()}/levels");

            _cachedLevels =
                JsonConvert.DeserializeObject<List<ClassLevel>>(body);
        }

        private void SaveNotesBtn_Click(object sender, RoutedEventArgs e)
        {
            string NotesText = NotesTbx.Text;

                using (var db = new PlayerData())
                {
                    var charToUpdate = db.Characters.FirstOrDefault(c => c.CharacterId == character.CharacterId);
                    if (charToUpdate != null)
                    {
                        charToUpdate.Notes = NotesText;
                        db.SaveChanges();
                        MessageBox.Show(this, "Notes saved successfully!");
                    }
                    else
                    {
                        MessageBox.Show(this, "Character not found. Notes could not be saved." );
                    }
                }
        }
    }
}
