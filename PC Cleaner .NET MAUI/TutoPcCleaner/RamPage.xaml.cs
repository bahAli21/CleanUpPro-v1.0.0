using TutoPcCleaner.Helpers;
using System.Management;
using Microsoft.Win32;
namespace TutoPcCleaner;

public partial class RamPage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();

    public RamPage()
	{
		InitializeComponent();
        ShowSystemInfos();
        GetRamUsage();
    }

    public void ShowSystemInfos()
    {
        // OS
        osVersion.Text = Sysinfos.GetWinVer();

        // CPU
        hardware.Text = Sysinfos.GetHardwareInfos();
    }

    public void GetRamUsage()
    {
        try
        {
            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");
            ulong totalRam = 0;
            ulong frram = 0;

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                frram = Convert.ToUInt64(objram["FreePhysicalMemory"]);
            }

            // Calcul % RAM libre
            int fram2 = Convert.ToInt32(frram);
            int fram3 = Convert.ToInt32(totalRam);
            string fram4 = Convert.ToString(fram2);
            string fram5 = Convert.ToString(fram3);
            double fram6 = Convert.ToDouble(fram4);
            double fram7 = Convert.ToDouble(fram5);
            double percent = fram6 / fram7 * 100;
            int per2 = (int)Math.Round(percent);

            ramUsageTxt.Text = 100 - per2 + "%";

            graph.Progress = 1 - (percent / 100);

            cellTotal.Detail = totalRam + " Mb.";
            cellFree.Detail = frram + " Mb. (" + per2 + "%).";
            cellUsed.Detail = (totalRam - frram) + " Mb. (" + (100 - per2) + "%)";
        }
        catch (Exception ex) { }
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


    private void ButtonCleanRAM_Clicked(object sender, EventArgs e)
    {
        graph.IsIndeterminate = true;
        OptimizeRAM();
    }

    // Fonction clean RAM
    public async void OptimizeRAM()
    {
        try
        {
            GC.Collect(1, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }
        catch (Exception ex) { }

        await Task.Delay(TimeSpan.FromSeconds(2));

        graph.IsIndeterminate = false;
        ramCleaned.IsVisible = true;

        GetRamUsage();
    }
}