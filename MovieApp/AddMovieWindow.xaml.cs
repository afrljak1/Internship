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
using static MovieApp.MainWindow;

namespace MovieApp
{
    /// <summary>
    /// Interaction logic for AddMovieWindow.xaml
    /// </summary>
    public partial class AddMovieWindow : Window
    {
        private MainWindow mainWindow;

        public AddMovieWindow()
        {
            InitializeComponent();
            DataContext = new MovieInfo();

           // mainWindow = main;
        }


        private void ImageUrlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string imageUrl = ImageUrlTextBox.Text;

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                try
                {
                    // Stvorite novi objekt BitmapImage i postavite ga kao izvor slike
                    BitmapImage bitmapImage = new BitmapImage(new Uri(imageUrl));
                    MovieImage.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    // Upravljajte greškom ako URL slike nije ispravan
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
            else
            {
                // Ako je polje za unos URL-a slike prazno, izbrišite sliku
                MovieImage.Source = null;
            }
        }



        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MovieInfo newMovie = new MovieInfo
            {
                Title = TitleTextBox.Text,
                Year = YearTextBox.Text,
                Type = TypeTextBox.Text,
                ImdbID = ImdbIDTextBox.Text,
                Language = LanguageTextBox.Text,
                Released = ReleasedTextBox.Text,
                Country = CountryTextBox.Text,
                Genre = GenreTextBox.Text,
                Awards = AwardsTextBox.Text,
                Poster = ImageUrlTextBox.Text

            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7064/api/Movie";

                    string newMovieData = JsonConvert.SerializeObject(newMovie);
                    HttpContent content = new StringContent(newMovieData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MovieInfo savedMovie = JsonConvert.DeserializeObject<MovieInfo>(responseContent);

                        MessageBox.Show("Movie added successfully.");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Error adding the movie.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void RuntimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
