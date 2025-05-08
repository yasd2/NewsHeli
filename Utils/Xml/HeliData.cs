namespace NewsHeli.Utils;

internal class HeliData
{
    public string ModelName { get; set; }
    public int? Livery { get; set; }

    public override string ToString()
    {
        return Livery.HasValue ? $"{ModelName} (Livery {Livery})" : ModelName;
    }

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
        var chosen = CustomizationXml.HeliDatas[MathHelper.GetRandomInteger(CustomizationXml.HeliDatas.Count)];

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

        return heli;
    }
}
