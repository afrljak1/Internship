using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static MovieApp.MainWindow;
using Newtonsoft.Json;
using System.Text;

namespace MovieApp
{
    public partial class MovieDetails : Window
    {
        private const string baseUrl = "https://localhost:7064/api/Movie/";

        public delegate void MovieDeletedEventHandler(MovieInfo deletedMovie);
        public event MovieDeletedEventHandler MovieDeleted;

        private MovieInfo originalMovie;

        public MovieInfo UpdatedMovie { get; private set; }



        private MovieInfo selectedMovie;

    public MovieDetails(MovieInfo movie)
    {
        InitializeComponent();
        selectedMovie = movie;
        DataContext = selectedMovie;
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
                    client.BaseAddress = new Uri(baseUrl);
                    string deletelEndpoint = $"{baseUrl}{title}";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.DeleteAsync(deletelEndpoint);

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
                MovieInfo movieInfo = DataContext as MovieInfo;

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{movieInfo.Title}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await DeleteMovieAsync(movieInfo.Title);

                    if (success)
                    {
                        MessageBox.Show("Movie deleted successfully!");
                        MovieDeleted?.Invoke(movieInfo);
                        Close();
                        
                      
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

      

/*        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Učitajte informacije iz korisničkog unosa u originalni film
                originalMovie.Title = TitleTextBox.Text;

                // Učitajte ostale informacije ...

                await ApiHelper.UpdateMovieAsync(originalMovie);

                MessageBox.Show("Movie updated successfully!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }*/

        private async Task UpdateMovieAsync(MovieInfo movie)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{baseUrl}{movie.MovieId}";
                    string json = JsonConvert.SerializeObject(movie);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(url, content);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating movie: {ex.Message}");
            }
        }

         private void EditButton_Click(object sender, RoutedEventArgs e)
         {
             EditMovieWindow editMovieWindow = new EditMovieWindow(selectedMovie);
             editMovieWindow.ShowDialog();

             if (editMovieWindow.UpdatedMovie != null)
             {
                 selectedMovie = editMovieWindow.UpdatedMovie;
                 DataContext = selectedMovie;
             }
         }

    }
}



