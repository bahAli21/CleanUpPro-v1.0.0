using TutoPcCleaner.Helpers;
using System.Windows.Input;

namespace TutoPcCleaner;

public partial class UpdatePage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();
    public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
    int version = 1;

    public UpdatePage()
	{
		InitializeComponent();
        ShowSystemInfos();
        BindingContext = this;
        CheckVersion();
    }

    public async void CheckVersion()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://www.anthony-cardinale.fr/_public/_dev/v1";
            string s = await client.GetStringAsync(url);
            int lastVersion = int.Parse(s);

            loadingGraph.IsVisible = false;
            loadingText.IsVisible = false;

            if (lastVersion > version)
            {
                // MAJ Dispo
                ShowUpdatePage();
            }
            else
            {
                // Up To Date
                ShowDefaultPage();
            }
        }
    }

    public void ShowUpdatePage()
    {
        updateTitle.IsVisible = true;
        updateSub.IsVisible = true;
        updateVersion.IsVisible = true;
        updateLink.IsVisible = true;
    }

    public void ShowDefaultPage()
    {
        defaultTitle.IsVisible = true;
        defaultSub.IsVisible = true;
        defaultVersion.IsVisible = true;
        defaultLink.IsVisible = true;
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

    private async void ButtonUpdateSoft_Clicked(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("http://anthony-cardinale.fr/tools/pccleaner?from=pccleaner&update=true");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {

        }
    }
}