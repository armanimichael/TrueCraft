using Gtk;

namespace TrueCraft.Launcher.Views;

public class MainMenuView : VBox
{
    private LauncherWindow _launcher { get; }

    private Label _welcomeText;
    private Button _singleplayerButton;
    private Button _multiplayerButton;
    private Button _optionsButton;
    private Button _quitButton;

    public MainMenuView(LauncherWindow window)
    {
        _launcher = window;
        SetSizeRequest(250, -1);

        _welcomeText = new Label("Welcome, " + _launcher.User.Username)
                       {
                           Justify = Justification.Center
                       };

        _singleplayerButton = new Button("Singleplayer");
        _multiplayerButton = new Button("Multiplayer");
        _optionsButton = new Button("Options");
        _quitButton = new Button("Quit Game");

        _singleplayerButton.Clicked += (sender, e) => _launcher.ShowSinglePlayerView();
        _multiplayerButton.Clicked += (sender, e) => _launcher.ShowMultiplayerView();
        _optionsButton.Clicked += (sender, e) => _launcher.ShowOptionView();
        _quitButton.Clicked += (sender, e) => Application.Quit();

        PackStart(_welcomeText, true, false, 0);
        PackStart(_singleplayerButton, true, false, 0);
        PackStart(_multiplayerButton, true, false, 0);
        PackStart(_optionsButton, true, false, 0);
        PackEnd(_quitButton, true, false, 0);
    }
}