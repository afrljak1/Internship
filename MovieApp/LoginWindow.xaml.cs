using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MovieApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
  
            private const string validUsername = "user";
            private const string validPassword = "password";

            public LoginWindow()
            {
                InitializeComponent();
            }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            if (await AuthenticateAsync(username, password))
            {
                resultText.Visibility = Visibility.Collapsed;
                MessageBox.Show("Login successful!");

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                resultText.Visibility = Visibility.Visible;
                resultText.Text = "Invalid username or password.";
            }
        }

        private async Task<bool> AuthenticateAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://localhost:7064/api/Logins";

                // Pošaljite GET zahtjev za prijavu na API
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                { 
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}

