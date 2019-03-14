using MonoDevelop.Components.Commands;

namespace CodeCleaner
{
    public class ConfigureCommandHandler : CommandHandler
    {
        protected override void Run()
        {
            var window = new MainWindow();
            window.SetPosition(Gtk.WindowPosition.CenterAlways);
            window.Show();
        }

        protected override void Update(CommandInfo info)
        {
            info.Enabled = true;
        }
    }
}