using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MovieApp
{
    public static class ApiHelper
    {
        private const string apiBaseUrl = "https://localhost:7064/api/Movie/";

        public static async Task UpdateMovieAsync(MovieInfo movie)
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
