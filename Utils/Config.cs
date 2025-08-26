namespace NewsHeli.Utils;

/// <summary>
/// Here are all settings for the ini.
/// </summary>
internal class Config
{
    public static Boolean EnableAIDispatch { get; set; } = true;
    public static UInt32 ArrivalTimeHeli { get; set; } = 30;
    public static UInt32 ArrivalTimeVan { get; set; } = 40;
    public static Boolean EnableHeli { get; set; } = true;
    public static Boolean EnableVan { get; set; } = true;
    public static Color ColorHeli { get; set; } = Color.Blue; 
    public static Color ColorVan { get; set; } = Color.Green;
    public static BlipSprite SpriteHeli { get; set; } = (BlipSprite)1;
    public static BlipSprite SpriteVan { get; set; } = (BlipSprite)1;



    /// <summary>
    /// This reads all the user settings from the Ini file.
    /// </summary>
    public static void Read()
    {
        var ini = new InitializationFile("plugins/LSPDFR/NewsHeli.ini");

        if (!ini.Exists())
        {
            Game.LogTrivial("FATAL ERROR: Could not locate 'plugins/LSPDFR/NewsHeli.ini' file!");
            return;
        }

        Logger.Log("Reading NewsHeli.ini file...");

        try
        {
            EnableAIDispatch = ini.ReadBoolean("Settings", "EnableAIDispatch", EnableAIDispatch);

            ArrivalTimeHeli = ini.ReadUInt32("Settings", "ArrivalTimeHeli", ArrivalTimeHeli);

            ArrivalTimeVan = ini.ReadUInt32("Settings", "ArrivalTimeVan", ArrivalTimeVan);

            EnableHeli = ini.ReadBoolean("Settings", "EnableHeli", EnableHeli);

            EnableVan = ini.ReadBoolean("Settings", "EnableVan", EnableVan);

            string sColorHeli = ini.ReadString("Settings", "ColorHeli", "Blue");
            ColorHeli = Color.FromName(sColorHeli);

            string sColorVan = ini.ReadString("Settings", "ColorVan", "Green");
            ColorVan = Color.FromName(sColorVan);

            SpriteHeli = (BlipSprite)ini.ReadInt32("Settings", "SpriteHeli", (int)SpriteHeli);

            SpriteVan = (BlipSprite)ini.ReadInt32("Settings", "SpriteVan", (int)SpriteVan);

            Logger.Log($"ColorHeli = {ColorHeli}, ColorVan = {ColorVan}");
        }
        catch (Exception e)
        {
            Logger.Log("#############################################");
            Logger.Log("Error reading NewsHeli.ini file: " + e);
        }
    }
}
