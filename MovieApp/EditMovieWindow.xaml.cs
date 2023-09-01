using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MovieApp
{
    public partial class EditMovieWindow : Window
    {
        private const string apiBaseUrl = "https://localhost:7064/api/Movie/";
        private MovieInfo originalMovie;

        public MovieInfo UpdatedMovie { get; private set; }
      
           
            public EditMovieWindow(MovieInfo movie)
            {
                InitializeComponent();
                originalMovie = movie;
                DataContext = movie;
            }

      
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                originalMovie.Title = TitleTextBox.Text;
                originalMovie.Year = YearTextBox.Text;
                originalMovie.Type = TypeTextBox.Text;
                originalMovie.ImdbID = ImdbIDTextBox.Text;
                originalMovie.Released = ReleasedTextBox.Text;
                originalMovie.Genre = GenreTextBox.Text;
                originalMovie.Language = LanguageTextBox.Text;
                originalMovie.Plot = PlotTextBox.Text;


                await ApiHelper.UpdateMovieAsync(originalMovie);

                    MessageBox.Show("Movie updated successfully!");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

            private async Task UpdateMovieAsync(MovieInfo movie)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{apiBaseUrl}{movie.MovieId}";
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
    }
}
