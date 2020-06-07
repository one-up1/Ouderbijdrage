using System;
using System.Windows;
using System.Windows.Controls;

namespace Ouderbijdrage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void bAddChild_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text.Length == 0)
            {
                tbName.Focus();
                return;
            }

            if (dpDateOfBirth.SelectedDate == null)
            {
                dpDateOfBirth.Focus();
                return;
            }

            lbChildren.Items.Add(new Child(tbName.Text, dpDateOfBirth.SelectedDate.Value));
            tbName.Text = "";

            Calc();
        }

        private void bRemoveChild_Click(object sender, RoutedEventArgs e)
        {
            if (lbChildren.SelectedItem != null)
            {
                lbChildren.Items.Remove(lbChildren.SelectedItem);
                Calc();
            }
        }

        private void cbSingleParent_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Calc();
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Calc();
        }

        private void Calc()
        {
            lCost.Content = "";
            if (lbChildren.Items.Count == 0) return;
            if (dpDate.SelectedDate == null) return;

            double cost = 50; //Basisbedrag.

            int count1 = 0, count2 = 0;
            foreach (Child child in lbChildren.Items)
            {
                int age = child.GetAge(dpDate.SelectedDate.Value);
                Console.WriteLine(child + ": " + age);

                if (age < 10)
                {
                    // Voor maximaal 3 kinderen jonger dan 10 jaar.
                    if (count1 < 3)
                    {
                        cost += 25;
                        count1++;
                    }
                }
                else
                {
                    // Voor maximaal 2 kinderen van 10 jaar en ouder.
                    if (count2 < 2)
                    {
                        cost += 37;
                        count2++;
                    }
                }
            }

            // De maximale ouderbijdrage bedraagt € 150.
            if (cost > 150)
            {
                cost = 150;
            }

            // Voor éénoudergezinnen wordt op de berekende bijdrage
            // (nadat de controle op het maximum heeft plaatsgevonden)
            // een reductie toegepast van 25%.
            if (cbSingleParent.IsChecked == true)
            {
                cost *= 0.75;
            }

            lCost.Content = "Ouderbijdrage: € " + Math.Round(cost, 2).ToString("0.00");
        }

        private class Child
        {
            private readonly string name;
            private readonly DateTime dateOfBirth;

            public Child(string name, DateTime dateOfBirth)
            {
                this.name = name;
                this.dateOfBirth = dateOfBirth;
            }

            public override string ToString()
            {
                return name;
            }

            public int GetAge(DateTime date)
            {
                return (int) (date - dateOfBirth).TotalDays / 365;
            }
        }
    }
}
