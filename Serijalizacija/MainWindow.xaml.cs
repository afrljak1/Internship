using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace Serijalizacija
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;

        public MainWindow()
        {
            InitializeComponent();
            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "predmeti.json"); //pozicioniranje json file-a na desktop
        }

        private void SerializeButton(object sender, RoutedEventArgs e)
        {
            var data = new MyClass();
            data.Predmeti = new List<string>
            {
                "Tehnike programiranja",
                "Algoritmi i strukture podataka",
                "Backend web tehnologije",
                "Razvoj softvera",
                "Uvod u programiranje",
                "Osnove informacionih sistema",
                "Napredni razvoj softvera"
            };
            data.Datum = DateTimeOffset.Parse("2023-07-14");
            SerializeToJson(data);
        }

        private void DeserializeButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = DeserializeFromJson<MyClass>();
                MessageBox.Show("Deserijalizacija uspešna!");

                // Ispisivanje deserijalizovanih podataka
                string predmeti = string.Join(", ", data.Predmeti);
                string poruka = $"Deserijalizovani predmeti:\n {predmeti}\nDatum: {data.Datum}";
                TextBoxOutput.Text = poruka;
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Greška prilikom deserijalizacije: {ex.Message}");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Greška prilikom čitanja fajla: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nepredviđena greška: {ex.Message}");
            }
        }

        private void SerializeToJson<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(fileName, json);
            MessageBox.Show("Serijalizacija uspešna!");
        }

        private T DeserializeFromJson<T>()
        {
            var json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public class MyClass
    {
        public List<string> Predmeti { get; set; }
        public DateTimeOffset Datum { get; set; }
    }
}


