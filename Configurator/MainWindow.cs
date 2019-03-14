using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected override async void OnShown()
    {
        base.OnShown();

        var config = await CodeMaid.Common.Configurator.LoadConfiguration();

        this.privating.Active = config.IsPrivatingEnabled;
        this.ordering.Active = config.IsOrderingEnabled;
    }

    protected async void OrderingChanged(object sender, EventArgs e)
    {
        if (sender is CheckButton checkbox)
        {
            var config = await CodeMaid.Common.Configurator.LoadConfiguration();
            config.IsOrderingEnabled = checkbox.Active;

            await CodeMaid.Common.Configurator.SaveConfiguration(config);
        }

    }

    protected async void PrivatingChanged(object sender, EventArgs e)
    {
        if (sender is CheckButton checkbox)
        {
            var config = await CodeMaid.Common.Configurator.LoadConfiguration();
            config.IsPrivatingEnabled = checkbox.Active;

            await CodeMaid.Common.Configurator.SaveConfiguration(config);
        }

    }
}
