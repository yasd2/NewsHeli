namespace NewsHeli;

public class Main : Plugin
{
    public override void Initialize()
    {
        Logger.Log("NewsHeli by Yasd loaded");
        Game.DisplayNotification($"~g~NewsHeli by Yasd {Assembly.GetExecutingAssembly().GetName().Version} loaded");

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
    }

    /// <summary>
    /// Clears the plugin at plugin unload
    /// </summary>
    public override void Finally()
    {
        Logger.Log("NewsHeli unloaded");

        // Clears entites and aborts tasks
        MainFiber.SafeAbort();
    }
}