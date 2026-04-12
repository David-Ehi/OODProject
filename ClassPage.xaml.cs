using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OODProject
{
    /// <summary>
    /// Interaction logic for FighterPage.xaml
    /// </summary>
    public partial class ClassPage : Page
    {

        private static readonly HttpClient client = new HttpClient(); // we use a static http client to make the api calls, this way we can reuse the same client for multiple calls and avoid the overhead of creating a new client for each call
        private string selectedClass;
        public ClassPage(string Class)
        {
            
            InitializeComponent();
            selectedClass = Class;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ClassTitleTxt.Text = $"{selectedClass.ToUpper()} Creation"; // sets the title of the page to the selected class
            if (selectedClass != "fighter") // if the selected class is not fighter, we hide the fighting style panel since it is only relevant for fighters
            {
                FightingStylePanel.Visibility = Visibility.Collapsed;
            }
            await GeneralClassApi();
            LoadSkillsForClass(selectedClass);
            LvlCbx.SelectedIndex = 0; // set the level combo box to level 1 by default
        }

        public async Task GeneralClassApi()
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/classes/{selectedClass}");
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

            if (LvlCbx.SelectedItem == null) return; // Guard clause to prevent errors when no level is selected

            await LoadClassLevels(); // Ensure levels are loaded and cached

            int selectedLevel = LvlCbx.SelectedIndex + 1;

            var totalFeatures = _cachedLevels //we use the cached levels to get the features for all levels up to the selected level, this way we dont have to make multiple api calls when the user changes the level selection
                .Where(l => l.level <= selectedLevel)
                .SelectMany(l => l.features)
                .ToList();

            ClassFeaturelbx.ItemsSource = totalFeatures;


            #region if subclass lvl

            int level = LvlCbx.SelectedIndex + 1;
            int subclassLevel = GetSubclassLevel(selectedClass);

            if (level >= subclassLevel)
            {
                SubclassPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SubclassPanel.Visibility = Visibility.Collapsed;
            }

            #endregion


            #region Proficiency Bonus
            Profbonistxbk.Text = $"Proficiency Bonus: {DetermineProficiencyBonus(selectedLevel)}";
            #endregion


        }

        #region Subclass Level
        public int GetSubclassLevel(string className)
        {
            switch (className.ToLower())
            {
                case "cleric":
                case "warlock":
                    return 1;

                case "wizard":
                    return 2;

                default:
                    return 3;
            }
        }   
        #endregion


        #region caching class levels
        private List<ClassLevel> _cachedLevels; //to make thing more efficient, we cache the levels for the selected class so we dont have to make multiple api calls when the user changes the level selection
        private async Task LoadClassLevels()
        {
            if (_cachedLevels != null) return;


            string body = await client.GetStringAsync(
                $"https://www.dnd5eapi.co/api/2014/classes/{selectedClass}/levels");

            _cachedLevels =
                JsonConvert.DeserializeObject<List<ClassLevel>>(body);
        }
        #endregion


            
        public int DetermineProficiencyBonus(int level)
        {
            if (level >= 1 && level <= 4)
                return 2;
            else if (level >= 5 && level <= 8)
                return 3;
            else if (level >= 9 && level <= 12)
                return 4;
            else if (level >= 13 && level <= 16)
                return 5;
            else if (level >= 17 && level <= 20)
                return 6;
            else
                throw new ArgumentException("Level must be between 1 and 20");

        }

        public List<string> GetSelectedSkills()
        {
            var selectedSkills = new List<string>();
            if (SkillAthletics.IsChecked == true) selectedSkills.Add("Athletics");
            if (SkillAcrobatics.IsChecked == true) selectedSkills.Add("Acrobatics");
            if (SkillAnimalHandling.IsChecked == true) selectedSkills.Add("Sleight of Hand");
            if (SkillInsight.IsChecked == true) selectedSkills.Add("Stealth");
            if (SkillIntimidation.IsChecked == true) selectedSkills.Add("Arcana");
            if (SkillHistory.IsChecked == true) selectedSkills.Add("History");
            if (SkillPerception.IsChecked == true) selectedSkills.Add("Investigation");
            if (SkillSurvival.IsChecked == true) selectedSkills.Add("Nature");
            return selectedSkills;
        }

        private async void SaveCharacter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PlayerData db = new PlayerData();

                int allowed = GetAllowedSkillCount(selectedClass);
                int selected = GetSelectedSkillCount();
                var selectedSkills = GetSelectedSkills();
                int level = LvlCbx.SelectedIndex + 1;
                string charactername = CharacterNametbx.Text;
                string playername = playernametbx.Text;
                var levelData = _cachedLevels.FirstOrDefault(l => l.level == level); // we get the level data for the selected level from the cached levels, this will be used to determine the spellcasting abilities of the character if they are a spellcaster

                if (playername == "") // we validate the input, if the player name is empty we show a message box and return, we do this for all required fields
                {
                    MessageBox.Show($"Please enter a player name");
                    return;
                }

                if (charactername == "")
                {
                    MessageBox.Show($"Please enter a character name");
                    return;
                }

                if (selected != allowed)
                {
                    MessageBox.Show($"Please select exactly {allowed} skills for your {selectedClass}.");
                    return;
                }

                if (HpTbx.Text == "")
                {
                    MessageBox.Show($"Please enter a health value or roll for one.");
                    return;
                }

                int Str = int.Parse(StrTbx.Text);
                int Con = int.Parse(ConTbx.Text);
                int Dex = int.Parse(DexTbx.Text);
                int Cha = int.Parse(ChaTbx.Text);
                int Wis = int.Parse(WisTbx.Text);
                int Int = int.Parse(IntTbx.Text);

                int Profbonus = DetermineProficiencyBonus(level);


                Player player = new Player();
                player.Name = playername;

                Characters character = new Characters();
                character.Name = charactername;
                character.Level = level;
                character.Class = selectedClass;
                character.HP = int.Parse(HpTbx.Text);
                character.Strength = Str;
                character.Constitution = Con;
                character.Dexterity = Dex;
                character.Wisdom = Wis;
                character.Intelligence = Int;
                character.Charisma = Cha;
                character.ProficencyBonus = Profbonus;
                character.Skills = string.Join(",", GetSelectedSkills());


                character.IsSpellCaster =IsSpellcasterMethod(selectedClass); // we determine if the character is a spellcaster based on the selected class, this will be used later to determine if we need to show spell slots and spells for the character

                if (levelData?.spellcasting != null)
                {
                    character.IsSpellCaster = true;
                    character.CantripsKnown = levelData.spellcasting.cantrips_known;
                    character.SpellSlotsLevel1 = levelData.spellcasting.spell_slots_level_1;
                    character.SpellSlotsLevel2 = levelData.spellcasting.spell_slots_level_2;
                    character.SpellSlotsLevel3 = levelData.spellcasting.spell_slots_level_3;
                    character.SpellSlotsLevel4 = levelData.spellcasting.spell_slots_level_4;
                    character.SpellSlotsLevel5 = levelData.spellcasting.spell_slots_level_5;
                    character.SpellSlotsLevel6 = levelData.spellcasting.spell_slots_level_6;
                    character.SpellSlotsLevel7 = levelData.spellcasting.spell_slots_level_7;
                    character.SpellSlotsLevel8 = levelData.spellcasting.spell_slots_level_8;
                    character.SpellSlotsLevel9 = levelData.spellcasting.spell_slots_level_9;
                }
                else
                {
                    character.IsSpellCaster = false;
                }

                character.Player = player;
                player.Characters.Add(character);

                db.Characters.Add(character);
                db.Players.Add(player);
                db.SaveChanges();


                CreateCharWindow.GetWindow(this).Close(); // Close the character creation window after saving


                MessageBox.Show("Character saved successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving character: {ex.Message}");
            }



        }

        #region RollStats
        Random rnd = new Random();
        private void RollStatsBtn_Click_1(object sender, RoutedEventArgs e)
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

            List<int> rolls = new List<int>();

            for (int i = 0; i < 4; i++)
                rolls.Add(rnd.Next(1, 7));

            rolls.Sort();
            rolls.RemoveAt(0);
            //rolls and removes lowest

            return rolls.Sum();
        }
        #endregion

        private async void ClassFeaturelbx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClassFeaturelbx.SelectedItem == null) return;

            PopupNameTxt.Text = (ClassFeaturelbx.SelectedItem as Feature).name;

            PopupDetailTxt.Text = $"Feature Index: {(ClassFeaturelbx.SelectedItem as Feature).index}\n" +
                $"URL: {(ClassFeaturelbx.SelectedItem as Feature).url}";
            await GetFeatureDesc((ClassFeaturelbx.SelectedItem as Feature).index); // we get the feature description using the index of the selected feature, this way we can display the description in the popup

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

        #region Health Stuff
        public int CalculateHealth(int hitDie, int level, int constitution)
        {
            int conModifier = (constitution - 10) / 2;
            int levelOneHp = hitDie + conModifier;
            int additionalHp = (int)Math.Ceiling(hitDie / 2.0 + 0.5) + conModifier; // average rounded up

            return levelOneHp + (additionalHp * (level - 1));
        }

        private void HealthRollBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ConTbx.Text))
            {
                MessageBox.Show("Please enter a constitution score to calculate health.");
                return;
            }

            int hitDie = int.Parse(HitDieTxbl.Text.Split('d')[1]);
            int level = LvlCbx.SelectedIndex + 1;
            int con = int.Parse(ConTbx.Text);

            HpTbx.Text = CalculateHealth(hitDie, level, con).ToString(); // we calculate the health using the hit die, level and constitution score, and display it in the HP textbox
        }
        #endregion


        public void LoadSkillsForClass(string className)
        {
            // hide all first
            SkillAcrobatics.Visibility = Visibility.Collapsed;
            SkillAnimalHandling.Visibility = Visibility.Collapsed;
            SkillAthletics.Visibility = Visibility.Collapsed;
            SkillArcana.Visibility = Visibility.Collapsed;
            SkillDeception.Visibility = Visibility.Collapsed;
            SkillHistory.Visibility = Visibility.Collapsed;
            SkillInsight.Visibility = Visibility.Collapsed;
            SkillIntimidation.Visibility = Visibility.Collapsed;
            SkillInvestigation.Visibility = Visibility.Collapsed;
            SkillMedicine.Visibility = Visibility.Collapsed;
            SkillNature.Visibility = Visibility.Collapsed;
            SkillPerception.Visibility = Visibility.Collapsed;
            SkillPerformance.Visibility = Visibility.Collapsed;
            SkillPersuasion.Visibility = Visibility.Collapsed;
            SkillReligion.Visibility = Visibility.Collapsed;
            SkillSleightOfHand.Visibility = Visibility.Collapsed;
            SkillStealth.Visibility = Visibility.Collapsed;
            SkillSurvival.Visibility = Visibility.Collapsed;

            switch (className.ToLower())
            {
                case "barbarian": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillAnimalHandling.Visibility = Visibility.Visible;
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillNature.Visibility = Visibility.Visible;
                    SkillPerception.Visibility = Visibility.Visible;
                    SkillSurvival.Visibility = Visibility.Visible;
                    break;

                case "bard": // choose 3
                    SkillsHeaderTxt.Text = "Skills (Choose Three)";
                    SkillAcrobatics.Visibility = Visibility.Visible;
                    SkillAnimalHandling.Visibility = Visibility.Visible;
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillDeception.Visibility = Visibility.Visible;
                    SkillHistory.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillInvestigation.Visibility = Visibility.Visible;
                    SkillMedicine.Visibility = Visibility.Visible;
                    SkillNature.Visibility = Visibility.Visible;
                    SkillPerception.Visibility = Visibility.Visible;
                    SkillPerformance.Visibility = Visibility.Visible;
                    SkillPersuasion.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    SkillSleightOfHand.Visibility = Visibility.Visible;
                    SkillStealth.Visibility = Visibility.Visible;
                    SkillSurvival.Visibility = Visibility.Visible;
                    break;

                case "cleric": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillHistory.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillMedicine.Visibility = Visibility.Visible;
                    SkillPersuasion.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    break;

                case "druid": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillArcana.Visibility = Visibility.Visible;
                    SkillAnimalHandling.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillMedicine.Visibility = Visibility.Visible;
                    SkillNature.Visibility = Visibility.Visible;
                    SkillPerception.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    SkillSurvival.Visibility = Visibility.Visible;
                    break;

                case "fighter": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillAcrobatics.Visibility = Visibility.Visible;
                    SkillAnimalHandling.Visibility = Visibility.Visible;
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillHistory.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillPerception.Visibility = Visibility.Visible;
                    SkillSurvival.Visibility = Visibility.Visible;
                    break;

                case "monk": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillAcrobatics.Visibility = Visibility.Visible;
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillHistory.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    SkillStealth.Visibility = Visibility.Visible;
                    break;

                case "paladin": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillMedicine.Visibility = Visibility.Visible;
                    SkillPersuasion.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    break;

                case "ranger": // choose 3
                    SkillsHeaderTxt.Text = "Skills (Choose Three)";
                    SkillAnimalHandling.Visibility = Visibility.Visible;
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillInvestigation.Visibility = Visibility.Visible;
                    SkillNature.Visibility = Visibility.Visible;
                    SkillPerception.Visibility = Visibility.Visible;
                    SkillStealth.Visibility = Visibility.Visible;
                    SkillSurvival.Visibility = Visibility.Visible;
                    break;

                case "rogue": // choose 4
                    SkillsHeaderTxt.Text = "Skills (Choose Four)";
                    SkillAcrobatics.Visibility = Visibility.Visible;
                    SkillAthletics.Visibility = Visibility.Visible;
                    SkillDeception.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillInvestigation.Visibility = Visibility.Visible;
                    SkillPerception.Visibility = Visibility.Visible;
                    SkillPersuasion.Visibility = Visibility.Visible;
                    SkillSleightOfHand.Visibility = Visibility.Visible;
                    SkillStealth.Visibility = Visibility.Visible;
                    break;

                case "sorcerer": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillArcana.Visibility = Visibility.Visible;
                    SkillDeception.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillPersuasion.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    break;

                case "warlock": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillArcana.Visibility = Visibility.Visible;
                    SkillDeception.Visibility = Visibility.Visible;
                    SkillHistory.Visibility = Visibility.Visible;
                    SkillIntimidation.Visibility = Visibility.Visible;
                    SkillInvestigation.Visibility = Visibility.Visible;
                    SkillNature.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    break;

                case "wizard": // choose 2
                    SkillsHeaderTxt.Text = "Skills (Choose Two)";
                    SkillArcana.Visibility = Visibility.Visible;
                    SkillHistory.Visibility = Visibility.Visible;
                    SkillInsight.Visibility = Visibility.Visible;
                    SkillInvestigation.Visibility = Visibility.Visible;
                    SkillMedicine.Visibility = Visibility.Visible;
                    SkillReligion.Visibility = Visibility.Visible;
                    break;
            }
        }

        public int GetSelectedSkillCount()
        {
            var allSkills = new List<CheckBox>
    {
        SkillAcrobatics, SkillAnimalHandling, SkillAthletics, SkillArcana,
        SkillDeception, SkillHistory, SkillInsight, SkillIntimidation,
        SkillInvestigation, SkillMedicine, SkillNature, SkillPerception,
        SkillPerformance, SkillPersuasion, SkillReligion, SkillSleightOfHand,
        SkillStealth, SkillSurvival
    };

            return allSkills.Count(c => c.IsChecked == true);
        }

        public int GetAllowedSkillCount(string className)
        {
            switch (className.ToLower())
            {
                case "bard":
                    return 3;
                case "ranger":
                    return 3;
                case "rogue":
                    return 4;
                default:
                    return 2;
            }
        }

        public bool IsSpellcasterMethod(string className)
        {
            switch (className.ToLower())
            {
                case "wizard":
                case "sorcerer":
                case "warlock":
                case "cleric":
                case "druid":
                case "bard":
                case "paladin":
                    return true;
                default:
                    return false;
            }
        }

    }

}
