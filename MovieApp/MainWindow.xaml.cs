using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

    public class MovieInfo
    {
        public int MovieId { get; set; }
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
            return $"{Title}";
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string apiKey = "95c80fd1";
        private int currentPage = 1;
        private int totalResults = 0;
        private int totalPages = 0;
        private ObservableCollection<MovieInfo> movieList = new ObservableCollection<MovieInfo>();
        private const string apiBaseUrl = "https://localhost:7064/api/Movie/";

        public MainWindow()
        {
            InitializeComponent();
        }



        private void addButton_Click(object sender, RoutedEventArgs e)
        {
             AddMovieWindow addMovieWindow = new AddMovieWindow();
             addMovieWindow.ShowDialog();
            SearchAndDisplayMoviesAsync(searchBox.Text.Trim());
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

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{apiBaseUrl}search?name={pretraga}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        List<MovieInfo> movies = JsonConvert.DeserializeObject<List<MovieInfo>>(jsonResult);

                        movieList.Clear(); // Clear the existing collection
                        foreach (var movie in movies)
                        {
                            movieList.Add(movie); // Add movies to the ObservableCollection
                        }

                        ResultListBox.ItemsSource = movieList; // Set the ObservableCollection as the ListBox's ItemsSource
                    }
                    else
                    {
                        MessageBox.Show("Error while fetching data.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"HTTP request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            currentPage = 1;
            ResultListBox.Items.Refresh();
        }

        private async Task SearchAndDisplayMoviesAsync(string searchTerm)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{apiBaseUrl}search?name={searchTerm}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        List<MovieInfo> movies = JsonConvert.DeserializeObject<List<MovieInfo>>(jsonResult);

                        foreach (var movieToRemove in movieList.Except(movies).ToList())
                        {
                            movieList.Remove(movieToRemove);
                        }

                        ResultListBox.ItemsSource = movies;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"HTTP request error: {ex.Message}");
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

       
        private async void doubleclick(object sender, MouseButtonEventArgs e)
        {
            if (ResultListBox.SelectedItem is MovieInfo selectedMovie)
            {
                await detailwindow(selectedMovie.MovieId);
                ResultListBox.Items.Refresh();
                await SearchAndDisplayMoviesAsync(searchBox.Text);


            }
        }

        private async Task detailwindow(int movieId)
        {
            try
            {
                string detailEndpoint = $"{apiBaseUrl}{movieId}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(detailEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        MovieInfo selectedMovie = JsonConvert.DeserializeObject<MovieInfo>(jsonResult);

                        MovieDetails detailWindow = new MovieDetails(selectedMovie);
                        detailWindow.MovieDeleted += MovieDeletedHandler;
                        detailWindow.ShowDialog();
                        
                    }
                    else
                    {
                        MessageBox.Show("Error while fetching movie details. Please try again later.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private async void EditMovie_Click(object sender, RoutedEventArgs e)
        {
            if (ResultListBox.SelectedItem is MovieInfo selectedMovie)
            {
                EditMovieDetails(selectedMovie);
            }
        }

        private void EditMovieDetails(MovieInfo movie)
        {
            EditMovieWindow editMovieWindow = new EditMovieWindow(movie);
            editMovieWindow.ShowDialog();

            if (editMovieWindow.UpdatedMovie != null)
            {
                int index = movieList.IndexOf(movie);
                if (index != -1)
                {
                    movieList[index] = editMovieWindow.UpdatedMovie;
                }

                ResultListBox.Items.Refresh();
            }
        }


        private void OpenHomePage()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

         private async Task<bool> DeleteMovieAsync(string title)
         {
             try
             {
                 using (HttpClient client = new HttpClient())
                 {
                     client.BaseAddress = new Uri(apiBaseUrl);
                     string deletelEndpoint = $"{apiBaseUrl}{title}";
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                     HttpResponseMessage response = await client.DeleteAsync(deletelEndpoint);
                     ResultListBox.Items.Refresh();
                     return response.IsSuccessStatusCode;

                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show($"An error occurred: {ex.Message}");
                 return false;
             }
         }


        private async void DeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MovieInfo movieInfo = ResultListBox.SelectedItem as MovieInfo;

                if (movieInfo == null)
                {
                    MessageBox.Show("Please select a movie to delete.");
                    return;
                }

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{movieInfo.Title}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await DeleteMovieAsync(movieInfo.Title);

                    if (success)
                    {
                        // Remove the movie from the ObservableCollection
                        movieList.Remove(movieInfo);

                        // Refresh the ListBox to reflect the changes
                        ResultListBox.Items.Refresh();

                        MessageBox.Show("Movie deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error deleting movie.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }



        private void MovieDeletedHandler(MovieInfo deletedMovie)
        {
            movieList.Remove(deletedMovie);
            ResultListBox.Items.Refresh();
        }


    }
}