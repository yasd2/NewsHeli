namespace NewsHeli;

public class Main : Plugin
{
    public override void Initialize()
    {
        Logger.Log($"NewsHeli {Assembly.GetExecutingAssembly().GetName().Version} by Yasd has been loaded");
        PluginLoadedNoticiation();

        // Checks if updates are available
        UpdateChecker.Check();

        // Reads the users Ini settings
        Config.Read();

        // Reads the users Xml settings
        CustomizationXml.Read();

        // Sets up the realationship groups
        RelationshipManager.Setup();

        // Starts the gamefiber for checking if a pursuit is active
        // Here the helicopter and van are spawned, too.
        MainFiber.Start();

        // Start HeliView, a gta camera with a news interface
        //HeliView.Initialize();
        HeliView.Initialize();
    }

    /// <summary>
    /// Clears the plugin at plugin unload
    /// </summary>
    public override void Finally()
    {
        Logger.Log($"NewsHeli {Assembly.GetExecutingAssembly().GetName().Version} has been unloaded");

        // Unloads the HeliView interface
        //HeliView.Finally();
        HeliView.Finally();

        // Clears entites and aborts tasks
        MainFiber.SafeAbort();
    }


    /// <summary>
    /// A custom method which pre- and unloads a notification picture
    /// </summary>
    static void PluginLoadedNoticiation()
    {
        GameFiber.StartNew(() =>
        {
            NativeFunction.Natives.REQUEST_STREAMED_TEXTURE_DICT("DIA_PILOT", false);

            while (!NativeFunction.Natives.HAS_STREAMED_TEXTURE_DICT_LOADED<bool>("DIA_PILOT"))
                GameFiber.Yield();

            Game.DisplayNotification("DIA_PILOT", "DIA_PILOT", "NewsHeli", $"{Assembly.GetExecutingAssembly().GetName().Version} by Yasd", "NewsHeli has been loaded.");
        
            NativeFunction.Natives.SET_STREAMED_TEXTURE_DICT_AS_NO_LONGER_NEEDED("DIA_PILOT");
        });
    }
}