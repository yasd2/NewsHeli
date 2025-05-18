namespace NewsHeli;

public class Main : Plugin
{
    public override void Initialize()
    {
        Logger.Log("NewsHeli by Yasd loaded");
        Game.DisplayNotification($"~g~NewsHeli by Yasd {Assembly.GetExecutingAssembly().GetName().Version} loaded");

        UpdateChecker.Check();

        Config.Read();

        CustomizationXml.Read();

        MainFiber.Start();

        //Game.AddConsoleCommands();
    }

    public override void Finally()
    {
        Logger.Log("NewsHeli unloaded");

        MainFiber.SafeAbort();
    }
}