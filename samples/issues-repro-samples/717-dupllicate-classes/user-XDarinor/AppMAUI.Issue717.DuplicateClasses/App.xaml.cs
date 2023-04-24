namespace AppMAUI.Issue717.DuplicateClasses;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
