using Microsoft.VisualBasic;
using TutoPcCleaner.Helpers;

namespace TutoPcCleaner;

public partial class ToolsPage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();

    public ToolsPage()
	{
		InitializeComponent();
        ShowSystemInfos();
    }

    public void ShowSystemInfos()
    {
        // OS
        osVersion.Text = Sysinfos.GetWinVer();

        // CPU
        hardware.Text = Sysinfos.GetHardwareInfos();
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

    private void ButtonCreateRestaurePoint_Clicked(object sender, EventArgs e)
    {
        dynamic restPoint = Interaction.GetObject("winmgmts:\\\\.\\root\\default:Systemrestore");

        if (restPoint != null)
        {
            if (restPoint.CreateRestorePoint("PC Cleaner restore point", 0, 100) == 0)
            {
                restaureTxt.Text = "Point de restauration créé !";
            }
            else
            {
                restaureTxt.Text = "Échec lors de la création du point de restauration !";
            }
        }
    }

    private void ButtonScan_Clicked(object sender, EventArgs e)
    {
        scanFileTxt.Text = "";

        string pathToScan = @"C:\Windows\System32";

        ScanDirectory(pathToScan);
    }

    public void ScanDirectory(string targetDirectory)
    {
        try
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
        } catch (Exception ex) { }

        try
        {
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ScanDirectory(subdirectory);
            }
        catch (Exception ex) { }
    }

    public void ProcessFile(string path)
    {
        scanFileTxt.Text = "Analyse du fichier " + path;
        // TODO : Faire le scan antivirus sur le fichier path
    }

}