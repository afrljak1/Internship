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

        private void doubleclick(object sender, MouseButtonEventArgs e)
        {
            if (resultListBox.SelectedItem is MovieInfo selectedMovie)
            {
                detailwindow(selectedMovie.ImdbID);
            }
        }

        private async void detailwindow(string imdbId)
        {
            try
            {
                string detailEndpoint = $"http://www.omdbapi.com/?apikey={apiKey}&i={imdbId}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(detailEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(jsonResult);

                        MovieInfo selectedMovie = new MovieInfo
                        {
                            Title = data.Title,
                            Year = data.Year,
                            Type = data.Type,
                            ImdbID = data.imdbID,
                            Rated = data.Rated,
                            Released = data.Released,
                            Runtime = data.Runtime,
                            Genre = data.Genre,
                            Actors = data.Actors,
                            Language = data.Language,
                            Poster = data.Poster,
                            Director = data.Director,
                            Writer = data.Writer,
                            Awards = data.Awards,
                            Country = data.Country,
                            Plot = data.Plot,
                            ImdbRating = data.ImdbRating,
                            Prodaction = data.Production
                        };

                        MovieDetails detailWindow = new MovieDetails(selectedMovie);
                        detailWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Error while fetching movie details. Please try again later.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: Not found!");
            }
        }


        public class MovieInfo
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string Type { get; set; }
            public string ImdbID { get; set; }
            public string Rated { get; set; }
            public string Released { get; set; }
            public string Runtime { get; set; }
            public string Genre { get; set; }
            public string Poster { get; set; }
            public string Actors { get; set; }
            public string Language { get; set; }
            public string Director { get; set; }
            public string Writer { get; set; }
            public string Awards { get; set; }
            public string Country { get; set; }
            public string Plot { get; set; }
            public string ImdbRating { get; set; }
            public string Prodaction { get; set; }

            public override string ToString()
            {
                return $"{Title} - ({Year}) - {Type} - {ImdbID}";
            }
      
        }


    }
}
