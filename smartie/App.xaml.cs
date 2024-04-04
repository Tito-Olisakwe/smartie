using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace smartie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShowSplashScreen();
        
        // Initialize the DatabaseManager
        DatabaseManager dbManager = new DatabaseManager();
        
        // Test the database connection
        dbManager.TestDatabaseConnection();

        // Rest of your startup code...
    }
        private void ShowSplashScreen()
        {
            var splashScreen = new SplashScreen();
            splashScreen.Show();
        }
    }
}
