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
using System.Windows.Shapes;

namespace OODProject
{
    /// <summary>
    /// Interaction logic for CharWindow.xaml
    /// </summary>
    public partial class CharWindow : Window
    {
        private Characters character;
        public CharWindow(Characters charactersFromMain)
        {
            InitializeComponent();
            character = charactersFromMain;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CharNameLabel.Content = character.Name;
            MaxHpLbl.Content = character.HP;
            HpTxBx.Text = character.HP.ToString();
            AcLbl.Content = character.AC;
            LoadAbilityScores();
        }

        private void LoadAbilityScores()
        {
            AbilitiesLbBx.Items.Add("STR: " + character.Strength);
            AbilitiesLbBx.Items.Add("DEX: " + character.Dexterity);
            AbilitiesLbBx.Items.Add("CON: " + character.Constitution);
            AbilitiesLbBx.Items.Add("WIS: " + character.Wisdom);
            AbilitiesLbBx.Items.Add("INT: " + character.Intellegence);
            AbilitiesLbBx.Items.Add("CHA: " + character.Charisma);
        }

        private void D20Btn_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            int roll = rand.Next(1, 21);
            MessageBox.Show($"You rolled a {roll}");
        }
    }
}
