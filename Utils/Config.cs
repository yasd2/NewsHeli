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


    // HeliView settings
    public static Boolean EnableHeliView { get; set; } = true;
    public static Boolean EnableOverlay { get; set; } = true;
    public static bool WarpPlayerInHeli { get; set; } = true;
    public static String HeliType { get; set; } = "random";
    public static Keys CamKey { get; set; } = Keys.R;
    public static Keys CamModifier { get; set; } = Keys.LControlKey;
    public static bool CamTwoKeys { get; set; } = true;
    public static Keys SkipKey { get; set; } = Keys.LShiftKey;
    public static bool SkipEnabled { get; set; } = true;


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


            // HeliView settings
            EnableHeliView = ini.ReadBoolean("Settings", "EnableHeliView", EnableHeliView);

            EnableOverlay = ini.ReadBoolean("Settings", "EnableOverlay", EnableOverlay);

            WarpPlayerInHeli = ini.ReadBoolean("Settings", "WarpPlayerInHeli", WarpPlayerInHeli);

            HeliType = ini.ReadString("Settings", "HeliType", HeliType);

            CamKey = (Keys)ini.ReadEnum("Settings", "CamKey", CamKey);

            CamModifier = (Keys)ini.ReadEnum("Settings", "CamModifier", CamModifier);

            if (CamModifier == Keys.None)
                CamTwoKeys = false;

            SkipKey = (Keys)ini.ReadEnum("Settings", "SkipKey", SkipKey);

            if (SkipKey == Keys.None)
                SkipEnabled = false;


            Config.ToLog();
        }
        catch (Exception e)
        {
            Logger.Log("#############################################");
            Logger.Log("Error reading NewsHeli.ini file: " + e);
        }
    }


    public static void ToLog()
    {
        Logger.Log("Current NewsHeli.ini settings:");
        Logger.Log($"EnableAIDispatch = {EnableAIDispatch}");
        Logger.Log($"ArrivalTimeHeli = {ArrivalTimeHeli}");
        Logger.Log($"ArrivalTimeVan = {ArrivalTimeVan}");
        Logger.Log($"EnableHeli = {EnableHeli}");
        Logger.Log($"EnableVan = {EnableVan}");
        Logger.Log($"ColorHeli = {ColorHeli}");
        Logger.Log($"ColorVan = {ColorVan}");
        Logger.Log($"SpriteHeli = {(int)SpriteHeli}");
        Logger.Log($"SpriteVan = {(int)SpriteVan}");

        // HeliView settings
        Logger.Log($"EnableHeliView = {EnableHeliView}");
        Logger.Log($"EnableOverlay = {EnableOverlay}");
        Logger.Log($"WarpPlayerInHeli = {WarpPlayerInHeli}");
        Logger.Log($"HeliType = {HeliType}");
        Logger.Log($"CamKey = {CamKey}");
        Logger.Log($"CamModifier = {CamModifier}");
        Logger.Log($"SkipKey = {SkipKey}");
    }
}
