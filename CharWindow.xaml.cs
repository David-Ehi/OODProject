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
            }
            //Loads the characters information into the labels and textboxes
            CharNameLabel.Content = character.Name;
            MaxHpLbl.Content = character.HP.ToString();
            HpTxBx.Text = character.HP.ToString();
            AcLbl.Content = character.AC;
            ClassTbx.Text = character.Class;

            HpBar.Maximum = character.HP;
            HpBar.Value = character.HP;

            LoadAbilityScores();
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

        private void GetClassFeaturesBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenCharFeatureWindow();
        }

        public void OpenCharFeatureWindow()
        {
            //opend the character window and passes the selected character to it
            Characters MainChar = character as Characters;
            CharFeatures FeatureWindow = new CharFeatures(MainChar);
            FeatureWindow.Owner = this;
            FeatureWindow.Show();
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
    }
}
