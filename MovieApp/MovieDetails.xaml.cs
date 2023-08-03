using System;
using System.Windows;
using System.Windows.Media.Imaging;
using static MovieApp.MainWindow;

namespace MovieApp
{
    public partial class MovieDetails : Window
    {
        public MovieDetails(MovieInfo movieInfo)
        {
            InitializeComponent();
            DataContext = movieInfo;

            if (!string.IsNullOrEmpty(movieInfo.Poster))
            {
                movieImage.Source = new BitmapImage(new Uri(movieInfo.Poster));
            }else
            MessageBox.Show("Couldn't load image");
            return;
        }
    }
}
