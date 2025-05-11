namespace NewsHeli.Utils;

internal class Config
{
    public static Boolean EnableAIDispatch = true;

    public static UInt32 ArrivalTimeHeli = 30;

    public static UInt32 ArrivalTimeVan = 40;

    public static Boolean EnableHeli = true;

    public static Boolean EnableVan = true;


    public static void Read()
    {
        var ini = new InitializationFile("plugins/LSPDFR/MCCallouts.ini");

        if (!ini.Exists())
        {
            Game.LogTrivial("Could not locate 'plugins/LSPDFR/NewsHeli.ini' file!");
            return;
        }

        EnableAIDispatch = ini.ReadBoolean("Settings", "EnableAIDispatch", EnableAIDispatch);

        ArrivalTimeHeli = ini.ReadUInt32("Settings", "ArrivalTimeHeli", ArrivalTimeHeli);

        ArrivalTimeVan = ini.ReadUInt32("Settings", "ArrivalTimeVan", ArrivalTimeVan);

        EnableHeli = ini.ReadBoolean("Settings", "EnableHeli", EnableHeli);

        EnableVan = ini.ReadBoolean("Settings", "EnableVan", EnableVan);
    }
}
