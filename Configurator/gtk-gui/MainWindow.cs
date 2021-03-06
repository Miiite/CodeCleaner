
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox1;

	private global::Gtk.CheckButton privating;

	private global::Gtk.CheckButton ordering;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("Code Cleaner Configuration");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		this.Modal = true;
		this.Resizable = false;
		this.AllowGrow = false;
		this.Gravity = ((global::Gdk.Gravity)(5));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox();
		this.vbox1.WidthRequest = 250;
		this.vbox1.HeightRequest = 150;
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.privating = new global::Gtk.CheckButton();
		this.privating.CanFocus = true;
		this.privating.Name = "privating";
		this.privating.Label = global::Mono.Unix.Catalog.GetString("\"Privating\" enabled");
		this.privating.Active = true;
		this.privating.DrawIndicator = true;
		this.privating.UseUnderline = true;
		this.vbox1.Add(this.privating);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.privating]));
		w1.Position = 0;
		w1.Expand = false;
		w1.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.ordering = new global::Gtk.CheckButton();
		this.ordering.CanFocus = true;
		this.ordering.Name = "ordering";
		this.ordering.Label = global::Mono.Unix.Catalog.GetString("\"Ordering\" enabled");
		this.ordering.Active = true;
		this.ordering.DrawIndicator = true;
		this.ordering.UseUnderline = true;
		this.vbox1.Add(this.ordering);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.ordering]));
		w2.Position = 1;
		w2.Expand = false;
		w2.Fill = false;
		this.Add(this.vbox1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 500;
		this.DefaultHeight = 400;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.privating.Toggled += new global::System.EventHandler(this.PrivatingChanged);
		this.ordering.Toggled += new global::System.EventHandler(this.OrderingChanged);
	}
}
