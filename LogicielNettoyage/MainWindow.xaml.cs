using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicielNettoyage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string version = "1.0.0"; //La premiere version du logiciel
        public DirectoryInfo winTemp; //Le dossier temp de Windows
        public DirectoryInfo appTemp; //Le dossier temp des application

        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp"); //Le chemin du dossier temp
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath()); //Le chemin du temp des Apps en fonction de l'utilisateur 
            CheckActu(); 
        }

        /// <summary>
        /// cette methode Vérifie les actus depuis notre serveur web WampServer
        /// </summary>
        public void CheckActu()
        {
            string url = "http://localhost/LogicielNettoyage/actu.txt";
            using (WebClient client = new WebClient())
            {
                string actu = client.DownloadString(url);
                if (actu != string.Empty)
                {
                    actuTxt.Content = actu;
                    actuTxt.Visibility = Visibility.Visible;
                    bandeau.Visibility = Visibility.Visible;
                }
                else
                {
                    actuTxt.Visibility = Visibility.Hidden;
                    bandeau.Visibility = Visibility.Hidden;
                }

            }
        }

        /// <summary>
        /// cette methode Vérifie si une MAJ est dispo
        /// </summary>
        public void CheckVersion()
        {
            string url = "http://localhost/LogicielNettoyage/version.txt";
            using (WebClient client = new WebClient())
            {
                string v = client.DownloadString(url);
                if (version != v)
                {
                    MessageBox.Show("Une mise à jour est dispo !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Votre logiciel est à jour !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Calcul de la taille d'un dossier
        /// </summary>
        /// <param name="dir">Dossier à traiter</param>
        /// <returns>Retourne la taille totale à nettoyer</returns>
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        // Cette procedure prend un dossier et se charge de le vider
        public void ClearTempData(DirectoryInfo di)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        private void Button_Histo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Créer page historique", "Historique", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_MAJ_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();
            //MessageBox.Show("Votre logiciel est a jour v3.0.8", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Ouverture d'un site web
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("http://bahali21.github.io/AzerType-Project-JS")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

        }

        private void Button_Analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        /// <summary>
        /// Analyse des dossiers a traiter
        /// </summary>
        public void AnalyseFolders()
        {
            Console.WriteLine("Début de l'analyse...");
            long totalSize = 0;

            try
            {
                totalSize += DirSize(winTemp) / 1000000;
                totalSize += DirSize(appTemp) / 1000000;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible d'analyser les dossiers : " + ex.Message);
            }

            espace.Content = totalSize + " Mb";
            titre.Content = "Analyse effectuée !";
            date.Content = DateTime.Today;
        }

        /// <summary>
        /// Nettoyage du PC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Nettoyer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");

            Clipboard.Clear();

            try
            {
                ClearTempData(winTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            try
            {
                ClearTempData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            btnClean.Content = "NETTOYAGE TERMINÉ";
            titre.Content = "Nettoyage effectué !";
            espace.Content = "0 Mb";
        }

    }
}
