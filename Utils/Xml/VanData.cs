namespace NewsHeli.Utils;

internal class VanData
{
    public string ModelName { get; set; }
    public int? Livery { get; set; }

    public override string ToString()
        => Livery.HasValue ? $"{ModelName} (Livery {Livery})" : ModelName;
    

    public static List<VanData> GetAllVans()
    {
        var result = new List<VanData>();

        string path = @"plugins\LSPDFR\NewsHeli.xml";
        if (!File.Exists(path))
            return result;

        var doc = XDocument.Load(path);
        var heliRoot = doc.Root?.Element("Vans");

        if (heliRoot == null)
            return result;

        foreach (var vehicleElement in heliRoot.Elements("Vehicle"))
        {
            string modelName = vehicleElement.Value.Trim();
            int? livery = null;

            var liveryAttr = vehicleElement.Attribute("livery");
            if (liveryAttr != null && int.TryParse(liveryAttr.Value, out int parsedLivery))
                livery = parsedLivery;

            result.Add(new VanData { ModelName = modelName, Livery = livery });
        }

        if (result.Count == 0) Logger.Log("ERROR: VanData is empty!");

        return result;
    }

    public static Vehicle SpawnRandom(Vector3 position, float heading = 0f)
    {
        VanData chosen = CustomizationXml.VanDatas[MathHelper.GetRandomInteger(CustomizationXml.VanDatas.Count)];

        Vehicle van = new Vehicle(chosen.ModelName, position)
        {
            IsPersistent = true,
            Heading = heading,
            IsEngineOn = true,
        };

        if (chosen.Livery.HasValue &&
            NativeFunction.Natives.GET_VEHICLE_LIVERY_COUNT<int>(van) > chosen.Livery.Value)
        {
            NativeFunction.Natives.SET_VEHICLE_LIVERY(van, chosen.Livery.Value);
        }

        return van;
    }
}
