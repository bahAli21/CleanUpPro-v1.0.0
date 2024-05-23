using TutoPcCleaner.Helpers;
using System.Windows.Input;
namespace TutoPcCleaner;

public partial class OptionsPage : ContentPage
{
    Sysinfos Sysinfos = new Sysinfos();
    public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

    public OptionsPage()
	{
		InitializeComponent();
        ShowSystemInfos();
        BindingContext = this;
        paramSearchMaj.IsChecked = Preferences.Get("paramSearchMaj", true);
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

    private void paramSearchMaj_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Set("paramSearchMaj", e.Value);
    }
}