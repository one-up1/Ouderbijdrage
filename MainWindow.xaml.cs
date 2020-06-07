using System;
using System.Text;
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
            if (lCost == null)
            {
                // Omdat ik in MainWindow.xaml een SelectedDate opgeef voor de pijldatum zodat deze
                // standaard de huidige datum heeft, wordt blijkbaar ook bij het opstarten de
                // SelectedDateChanged listener aangeroepen die deze functie aanroept en dan crasht
                // het programma omdat lCost nog null is.
                return;
            }
            lCost.Content = "";
            if (lbChildren.Items.Count == 0)
            {
                Console.WriteLine("No children specified");
                return;
            }
            if (dpDate.SelectedDate == null)
            {
                Console.WriteLine("No date specified");
                return;
            }

            double cost = 50; // Basisbedrag.

            int count1 = 0, count2 = 0, ignored = 0;
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
                    else
                    {
                        ignored++;
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
                    else
                    {
                        ignored++;
                    }
                }
            }

            // Voor éénoudergezinnen wordt op de berekende bijdrage
            // (nadat de controle op het maximum heeft plaatsgevonden)
            // een reductie toegepast van 25%.
            if (cbSingleParent.IsChecked == true)
            {
                // Moet deze reductie worden toegepast voor of nadat het maximum van 150 is
                // toegepast? (anders zou het maximum voor eenoudergezinnen 112.5 zijn)
                cost *= 0.75;
            }

            // De maximale ouderbijdrage bedraagt € 150.
            if (cost > 150)
            {
                cost = 150;
            }

            // Laat resultaat zien met de aantallen en de bijdrage.
            if (count1 != 0)
            {
                lCost.Content += FormatChildCount(count1) +
                    " jonger dan 10 jaar" + Environment.NewLine;
            }
            if (count2 != 0)
            {
                lCost.Content += FormatChildCount(count2) +
                    " van 10 jaar en ouder" + Environment.NewLine;
            }
            if (ignored != 0)
            {
                lCost.Content += FormatChildCount(ignored) +
                    " niet meegeteld" + Environment.NewLine;
            }
            lCost.Content += "Ouderbijdrage: € " + Math.Round(cost, 2).ToString("0.00");
        }

        private static String FormatChildCount(int i)
        {
            String s = i + " kind";
            if (i != 1)
                s += "eren";
            return s;
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
