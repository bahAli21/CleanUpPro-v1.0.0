using System.Runtime.InteropServices;
using TutoPcCleaner.Helpers;
using Plugin.LocalNotification;

namespace TutoPcCleaner
{
    public partial class MainPage : ContentPage
    {
        Sysinfos Sysinfos = new Sysinfos();

        // Checkboxes
        bool chkbFichiersTempChecked = true;
        bool chkbCorbeilleChecked = true;
        bool chkbWinUpdateChecked = true;
        bool chkbErrorsChecked = true;
        bool chkbLogsChecked = true;

        long totalCleanedSize = 0;

        int version = 1;

        public MainPage()
        {
            InitializeComponent();

            CheckVersion();

            ShowSystemInfos();

            InitChkbStates();
        }

        /// <summary>
        /// Récupérer la dernière version du logiciel
        /// </summary>
        public async void CheckVersion()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://www.anthony-cardinale.fr/_public/_dev/v1";
                    string s = await client.GetStringAsync(url);
                    int lastVersion = int.Parse(s);

                    bool paramShowNotif = Preferences.Get("paramSearchMaj", true);

                    if (lastVersion > version && paramShowNotif)
                    {
                        ShowNotif();
                    }
                }
            } catch (Exception ex) { }
        }

        /// <summary>
        /// Afficher une notification de MAJ
        /// </summary>
        public void ShowNotif()
        {
            var notif = new NotificationRequest
            {
                NotificationId = 1,
                Title = "Mise à jour disponible !",
                Subtitle = "Obtenez la dernière version de PC Cleaner",
                Description = "Téléchargez dès à présent la dernière mise à jour.",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1),
                }
            };
            LocalNotificationCenter.Current.Show(notif);
        }

        public void ShowSystemInfos()
        {
            // OS
            osVersion.Text = Sysinfos.GetWinVer();

            // CPU
            hardware.Text = Sysinfos.GetHardwareInfos();
        }

        public void InitChkbStates()
        {
            // Fichiers temp
            chkbFichiersTempChecked = Preferences.Get("chkbFichiersTempChecked", true);
            chkbFichiersTemp.IsChecked = chkbFichiersTempChecked;
            // Corbeille
            chkbCorbeilleChecked = Preferences.Get("chkbCorbeilleChecked", true);
            chkbCorbeille.IsChecked = chkbCorbeilleChecked;
            // Win Update
            chkbWinUpdateChecked = Preferences.Get("chkbWinUpdateChecked", true);
            chkbWinUpdate.IsChecked = chkbWinUpdateChecked;
            // Erreurs
            chkbErrorsChecked = Preferences.Get("chkbErrorsChecked", true);
            chkbErrors.IsChecked = chkbErrorsChecked;
            // Logs
            chkbLogsChecked = Preferences.Get("chkbLogsChecked", true);
            chkbLogs.IsChecked = chkbLogsChecked;
        }

        private async void InfoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://bahali21.github.io/BAHMamadou");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occurred. No browser may be installed on the device.
            }
        }

        /// <summary>
        /// Se déclenche lors du clic sur le bouton "Nettoyer"
        /// </summary>
        private void ButtonClean_Clicked(object sender, EventArgs e)
        {
            ResetValues();

            // Lancer le nettoyage
            infos.IsVisible = false;

            // Fichiers temp
            if (chkbFichiersTempChecked) ClearWindowsTempFolder();
            // Vider corbeille
            if (chkbCorbeilleChecked) EmptyRecycleBin();
            // MAJ
            if (chkbWinUpdateChecked) ClearWinUpdate();
            // Erreurs
            if (chkbErrorsChecked) ClearWinWER();
            // Logs
            if (chkbLogsChecked) ClearWinLogs();

            progression.Progress = 1;
            tableRecap.IsVisible = true;

            // Calcul de l'espage gagné suite au nettoyage
            long totalCleanedSizeInMb = totalCleanedSize / 1000000;
            if (totalCleanedSizeInMb < 0) totalCleanedSizeInMb = 0;

            if (totalCleanedSizeInMb < 50)
            {
                totalSize.Text = "< 10 Mb supprimés.";
            }
            else
            {
                totalSize.Text = "~" + totalCleanedSizeInMb + " Mb supprimés !";
            }
        }

        // Les fonctions de nettoyage

        /// <summary>
        /// Vider la corbeille
        /// </summary>
        public void EmptyRecycleBin()
        {
            // Flag pour annuler la confirmation
            const int SHERB_NOCONFIRMATION = 0x00000001;

            try
            {
                SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION);

                detailCorbeille.Detail = "Les fichiers de la corbeille ont été supprimés.";
            }
            catch (Exception e) { }
        }

        [DllImport("shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);


        /// <summary>
        /// Vider le dossier Temp de Windows
        /// </summary>
        public void ClearWindowsTempFolder()
        {
            string path = @"C:\Windows\Temp";

            if (Directory.Exists(path))
            {
                detailFichiersTemp.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        /// <summary>
        /// Suppression MAJ
        /// </summary>
        public void ClearWinUpdate()
        {
            string path = @"C:\Windows\SoftwareDistribution\Download";

            if (Directory.Exists(path))
            {
                detailWinUpdate.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        /// <summary>
        /// Rapports d'erreur
        /// </summary>
        public void ClearWinWER() 
        {
            string path = @"C:\ProgramData\Microsoft\Windows\WER";

            if (Directory.Exists(path))
            {
                detailErrors.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }

        /// <summary>
        ///  Logs
        /// </summary>
        public void ClearWinLogs()
        {
            string path = @"C:\Windows\System32\winevt\Logs";

            if (Directory.Exists(path))
            {
                detailLogs.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";

                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize = totalCleanedSize + size;

                ProcessDirectory(path);
            }
        }


        // Utils
        /// <summary>
        /// Calcul du nombre de fichiers dans un dossier
        /// </summary>
        /// <param name="path">Le dossier</param>
        /// <returns>Le nombre de fichiers</returns>
        public int GetFilesCountInFolder(string path)
        {
            int count = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Count();
            return count;
        }

        /// <summary>
        /// Lister les fichiers d'un dossier de façon récursive
        /// </summary>
        /// <param name="targetDirectory">Le dossier</param>
        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        /// <summary>
        /// Supprimer les fichiers
        /// </summary>
        /// <param name="path">Le fichier</param>
        public void ProcessFile(string path)
        {
            try
            {
                if (path.Contains("\\Temp")) 
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\SoftwareDistribution"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\winevt\\Logs"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\Windows\\WER"))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                // On retire du calcul le poids du fichier non supprimé
                FileInfo fi = new FileInfo(path);
                totalCleanedSize -= fi.Length;

                Console.WriteLine("The process failed: {0}", e.Message);
            }
        }

        /// <summary>
        /// Calculer la taille d'un répertoire
        /// </summary>
        /// <param name="d">Le répertoire</param>
        /// <returns>La taille</returns>
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        /// <summary>
        /// Réinitialiser le tableau récap
        /// </summary>
        public void ResetValues()
        {
            totalCleanedSize = 0;
            progression.Progress = 0;
            tableRecap.IsVisible = false;
            totalSize.Text = "";

            detailCorbeille.Detail = "Ignoré.";
            detailErrors.Detail = "Ignoré.";
            detailFichiersTemp.Detail = "Ignoré.";
            detailLogs.Detail = "Ignoré.";
            detailWinUpdate.Detail = "Ignoré.";
        }


        // Liste des évènements "Checkbox modifiée"
        private void chkbFichiersTemp_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbFichiersTempChecked = e.Value;
            Preferences.Set("chkbFichiersTempChecked", chkbFichiersTempChecked);
        }

        private void chkbCorbeille_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbCorbeilleChecked = e.Value;
            Preferences.Set("chkbCorbeilleChecked", chkbCorbeilleChecked);
        }

        private void chkbLogs_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbLogsChecked = e.Value;
            Preferences.Set("chkbLogsChecked", chkbLogsChecked);
        }

        private void chkbWinUpdate_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbWinUpdateChecked = e.Value;
            Preferences.Set("chkbWinUpdateChecked", chkbWinUpdateChecked);
        }

        private void chkbErrors_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbErrorsChecked = e.Value;
            Preferences.Set("chkbErrorsChecked", chkbErrorsChecked);
        }

        // Events de click sur le menu de gauche (pages)
        private async void ImageButton_Options_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OptionsPage());
        }

        private async void ImageButton_ram_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RamPage());
        }

        private async void ImageButton_tools_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToolsPage());
        }

        private async void ImageButton_maj_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdatePage());
        }

        private async void ImageButton_clean_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }

}
