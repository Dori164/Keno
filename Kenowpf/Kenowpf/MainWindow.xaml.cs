using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace KenoGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RacsFeltolt();
        }

        private void RacsFeltolt()
        {
            int szam = 1;
            for (int sor = 0; sor < 8; sor++)
            {
                for (int oszlop = 0; oszlop < 10; oszlop++)
                {
                    Label lbl = new Label
                    {
                        Content = szam.ToString(),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Background = Brushes.LightGreen,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    Grid.SetRow(lbl, sor);
                    Grid.SetColumn(lbl, oszlop);
                    gridSzelveny.Children.Add(lbl);
                    szam++;
                }
            }
        }

        private void BtnBetolt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Szöveges fájlok (*.txt)|*.txt|Minden fájl (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] sorok = File.ReadAllLines(openFileDialog.FileName);
                    lstSzelvenyek.ItemsSource = sorok;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a fájl betöltésekor: {ex.Message}");
                }
            }
            else
            {
                string alapFajl = "szelvenyek.txt";
                if (File.Exists(alapFajl))
                {
                    string[] sorok = File.ReadAllLines(alapFajl);
                    lstSzelvenyek.ItemsSource = sorok;
                }
                else
                {
                    MessageBox.Show("Nem található a szelvenyek.txt fájl!");
                }
            }
        }

        private void LstSzelvenyek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSzelvenyek.SelectedItem != null)
            {
                string szelveny = lstSzelvenyek.SelectedItem.ToString();
                string[] reszek = szelveny.Split(new[] { ';', ':', ',' });
                List<int> kivalasztottSzamok = new List<int>();

                for (int i = 1; i < reszek.Length; i++)
                {
                    if (int.TryParse(reszek[i], out int szam))
                    {
                        kivalasztottSzamok.Add(szam);
                    }
                }

                foreach (Label lbl in gridSzelveny.Children)
                {
                    if (int.TryParse(lbl.Content.ToString(), out int szam) && kivalasztottSzamok.Contains(szam))
                    {
                        lbl.Background = Brushes.Yellow;
                    }
                    else
                    {
                        lbl.Background = Brushes.LightGreen;
                    }
                }
            }
        }

        private void BtnSorsol_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            List<int> sorsoltSzamok = new List<int>();

            while (sorsoltSzamok.Count < 20)
            {
                int szam = rnd.Next(1, 81);
                if (!sorsoltSzamok.Contains(szam))
                {
                    sorsoltSzamok.Add(szam);
                }
            }

            sorsoltSzamok.Sort();

            foreach (Label lbl in gridSzelveny.Children)
            {
                if (int.TryParse(lbl.Content.ToString(), out int szam) && sorsoltSzamok.Contains(szam))
                {
                    lbl.Background = Brushes.LightCoral;
                }
            }

            if (lstSzelvenyek.SelectedItem != null)
            {
                string szelveny = lstSzelvenyek.SelectedItem.ToString();
                string[] reszek = szelveny.Split(new[] { ';', ':', ',' });
                List<int> kivalasztottSzamok = new List<int>();

                for (int i = 1; i < reszek.Length; i++)
                {
                    if (int.TryParse(reszek[i], out int szam))
                    {
                        kivalasztottSzamok.Add(szam);
                    }
                }

                int talalatok = 0;
                foreach (int szam in kivalasztottSzamok)
                {
                    if (sorsoltSzamok.Contains(szam))
                    {
                        talalatok++;
                    }
                }

                lblSzorzo.Content = $"Szorzó: {talalatok}";
                lblNyeremeny.Content = $"Nyeremény: {Szorzo(kivalasztottSzamok.Count, talalatok)}";
            }
        }

        private int Szorzo(int jatekTipus, int talalatokSzama)
        {
            Dictionary<string, int> nyeroParok = new Dictionary<string, int>
            {
                {"10-10",1000000}, {"10-9",8000}, {"10-8",350}, {"10-7",30}, {"10-6",3}, {"10-5",1}, {"10-0",2},
                {"9-9",100000}, {"9-8",1200}, {"9-7",100}, {"9-6",12}, {"9-5",3}, {"9-0",1},
                {"8-8",20000}, {"8-7",350}, {"8-6",25}, {"8-5",5}, {"8-0",1},
                {"7-7",5000}, {"7-6",60}, {"7-5",6}, {"7-4",1}, {"7-0",1},
                {"6-6",500}, {"6-5",20}, {"6-4",3}, {"6-0",1},
                {"5-5",200}, {"5-4",10}, {"5-3",2},
                {"4-4",100}, {"4-3",2},
                {"3-3",15}, {"3-2",1},
                {"2-2",6},
                {"1-1",2}
            };

            string kulcs = $"{jatekTipus}-{talalatokSzama}";
            return nyeroParok.TryGetValue(kulcs, out int ertek) ? ertek : 0;
        }
    }
}
