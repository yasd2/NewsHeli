namespace NewsHeli.Utils;

internal class HeliData
{
    public string ModelName { get; set; }
    public int? Livery { get; set; }

    public override string ToString()
    {
        return Livery.HasValue ? $"{ModelName} (Livery {Livery})" : ModelName;
    }

    /// <summary>
    /// This reads the HeliData from the .xml settings file
    /// </summary>
    public static List<HeliData> GetAllHelicopters()
    {
        var result = new List<HeliData>();

        string path = @"plugins\LSPDFR\NewsHeli.xml";
        if (!File.Exists(path))
            return result;

        var doc = XDocument.Load(path);
        var heliRoot = doc.Root?.Element("Helicopters");

        if (heliRoot == null)
            return result;

        foreach (var vehicleElement in heliRoot.Elements("Vehicle"))
        {
            string modelName = vehicleElement.Value.Trim();
            int? livery = null;

            var liveryAttr = vehicleElement.Attribute("livery");
            if (liveryAttr != null && int.TryParse(liveryAttr.Value, out int parsedLivery))
                livery = parsedLivery;

            result.Add(new HeliData { ModelName = modelName, Livery = livery });
        }

        if (result.Count == 0) Logger.Log("ERROR: HeliData is empty!");

        return result;
    }

    public static Vehicle SpawnRandom(Vector3 position, float heading = 0f)
    {
        Logger.Log("Spawning News Helicopter...");

        var chosen = CustomizationXml.HeliDatas.UseRandom();

        var heli = new Vehicle(chosen.ModelName, position)
        {
            IsPersistent = true,
            Heading = heading,
            IsEngineOn = true,
            IsExplosionProof = true,
        };

        if (chosen.Livery.HasValue &&
            NativeFunction.Natives.GET_VEHICLE_LIVERY_COUNT<int>(heli) > chosen.Livery.Value)
        {
            NativeFunction.Natives.SET_VEHICLE_LIVERY(heli, chosen.Livery.Value);
        }

        Logger.Log($"Helicopter Hash is {heli.Model.Hash}, Name is {heli.Model.Name}");

        if (heli.Model.Hash == Game.GetHashKey("CONADA"))
        {
            NativeFunction.Natives.SET_VEHICLE_MOD_KIT(heli, 0); // has to be called before changing SET_​VEHICLE_​MOD

            Logger.Log($"[SPECIAL] Setting hardcoded WeazelNews livery for CONADA");
            NativeFunction.Natives.SET_​VEHICLE_​MOD(heli, 48, 4, true); // Set the livery to Weazel News
        }


        return heli;
    }
}
