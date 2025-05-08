namespace NewsHeli.Utils;

public class RNUIMenu
{
    public RNUIMenu(string title = "Menu", string subtitle = "", string subSubTitle = "",
        List<(string title, string description, Action action)> items = null,
        Keys openingKey = Keys.J)
    {
        Title = title;
        Subtitle = subtitle;
        SubSubTitle = subSubTitle;
        Items = items ?? new List<(string, string, Action)>();
        OpeningKey = openingKey;

        SetupMenu();
    }

    public RNUIMenu() { }

    private void SetupMenu()
    {
        if (IsSetup) return;

        Pool = new MenuPool();
        Menu = new UIMenu(this.Title, this.Subtitle);

        var subTitle = new UIMenuItem(this.SubSubTitle) { Skipped = true };
        Menu.AddItem(subTitle);

        foreach (var (title, subtitle, action) in Items)
        {
            var item = new UIMenuItem(title, subtitle);
            item.Activated += (menu, _) =>
            {
                item.Enabled = false;
                action?.Invoke();
                item.Enabled = true;
                menu.Visible = false;
            };
            Menu.AddItem(item);
        }

        Menu.SetBannerType(Color.Black);
        Menu.TitleStyle = Menu.TitleStyle with
        {
            Font = TextFont.Monospace,
            Color = Color.LightSeaGreen,
            DropShadow = true,
        };
        subTitle.ForeColor = Color.LightSeaGreen;
        subTitle.BackColor = Color.Black;
        Menu.DescriptionSeparatorColor = Color.LightSeaGreen;

        Menu.MouseControlsEnabled = false;
        Menu.AllowCameraMovement = true;

        Pool.Add(Menu);
        this.IsSetup = true;
    }

    public void ProcessMenu()
    {
        Pool.ProcessMenus();

        if (Game.IsKeyDown(OpeningKey))
        {
            if (Menu.Visible)
            {
                Menu.Visible = false;
            }
            else if (!UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
            {
                Menu.Visible = true;
            }
        }
    }


    public MenuPool Pool { get; set; }
    public UIMenu Menu { get; set; }
    public bool IsSetup { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string SubSubTitle { get; set; }
    public List<(string title, string description, Action action)> Items { get; set; }
    public Keys OpeningKey { get; set; }
}
