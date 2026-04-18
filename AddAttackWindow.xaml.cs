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
    /// Interaction logic for AddAttackWindow.xaml
    /// </summary>
    public partial class AddAttackWindow : Window
    {
        private Characters character;
        public AddAttackWindow(Characters charactersFromMain)
        {
            InitializeComponent();
            character = charactersFromMain;
        }

        private void SaveAttackBtn_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(AttackNameTbx.Text) || NumDiceCbx.SelectedItem == null || DiceTypeCbx.SelectedItem == null)
            {
                MessageBox.Show(this, "Please fill in all fields.");
                return;
            }
            if (!int.TryParse(BonusDamageTbx.Text, out _) || !int.TryParse(ToHitBonusTbx.Text, out _)) // tellisence magic right here, we just want to check if they are valid integers, we don't care about the actual values at this point
            {
                MessageBox.Show(this, "Please enter valid numbers for Bonus Damage and To Hit Bonus.");
                return;
            }
            PlayerData db = new PlayerData();
            using (db)
            {
                Attack newAttack = new Attack
                {
                    Name = AttackNameTbx.Text,
                    NumDice = int.Parse(((ComboBoxItem)NumDiceCbx.SelectedItem).Content.ToString()),
                    DiceType = int.Parse(((ComboBoxItem)DiceTypeCbx.SelectedItem).Content.ToString().Replace("d", "")),
                    BonusDamage = int.Parse(BonusDamageTbx.Text),
                    ToHitBonus = int.Parse(ToHitBonusTbx.Text),
                    CharacterId = character.CharacterId
                };
                db.Attacks.Add(newAttack);
                db.SaveChanges();
                MessageBox.Show(this, "Attack saved successfully.");
                this.Close();
            }
        }
    }
}
