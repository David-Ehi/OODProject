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
        private string selectedClass;
        public FighterPage(string Class)
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

            var client = new HttpClient();

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
            {


                PlayerData db = new PlayerData();

                var selectedSkills = GetSelectedSkills();
                int level = LvlCbx.SelectedIndex + 1;
                string charactername = CharacterNametbx.Text;
                string playername = playernametbx.Text;

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


                character.Player = player;
                player.Characters.Add(character);

                db.Characters.Add(character);
                db.Players.Add(player);
                db.SaveChanges();


                MessageBox.Show("Character saved successfully");



            }
        }
    }
    
}
