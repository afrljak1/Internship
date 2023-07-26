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

namespace MovieApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string apiKey = "95c80fd1";
        private int currentPage = 1;
        private int totalResults = 0;
        private int totalPages = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string pretraga = searchBox.Text.Trim();

            if (string.IsNullOrEmpty(pretraga))
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }

            if (pretraga.Length > 8)
            {
                MessageBox.Show("Search term can have up to 8 characters.");
                return;
            }

            currentPage = 1; 
            await SearchAndDisplayMoviesAsync(pretraga);
        }

        private async Task SearchAndDisplayMoviesAsync(string searchTerm)
        {
            try
            {
                string endpoint = $"http://www.omdbapi.com/?apikey={apiKey}&s={searchTerm}&page={currentPage}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(jsonResult);

                        totalResults = Convert.ToInt32(data.totalResults);
                        totalPages = (totalResults) / 10; 

                        List<MovieInfo> movies = new List<MovieInfo>();
                        foreach (var movie in data.Search)
                        {
                            MovieInfo movieInfo = new MovieInfo
                            {
                                Title = movie.Title,
                                Year = movie.Year,
                                Type = movie.Type,
                                ImdbID = movie.imdbID,
                                // Poster = movie.Poster
                            };
                            movies.Add(movieInfo);
                        }

                        resultListBox.ItemsSource = movies;

                        paginationLabel.Content = $"Page {currentPage} of {totalPages}";
                    }
                    else
                    {
                        MessageBox.Show("Error while fetching data. Please try again later.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void prevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                await SearchAndDisplayMoviesAsync(searchBox.Text.Trim());
            }
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                await SearchAndDisplayMoviesAsync(searchBox.Text.Trim());
            }
        }

        public class MovieInfo
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Type { get; set; }
            public string ImdbID { get; set; }
            //public string Poster { get; set; }

            public override string ToString()
            {
                return $"{Title} - ({Year}) - {Type} - {ImdbID}";
            }
        }
    }
}
