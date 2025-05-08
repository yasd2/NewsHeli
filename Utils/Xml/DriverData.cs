namespace NewsHeli.Utils;

internal class DriverData
{
    public string ModelName { get; set; }

    public static List<DriverData> GetAllDrivers()
    {
        var result = new List<DriverData>();

        string path = @"plugins\LSPDFR\NewsHeli.xml";
        if (!File.Exists(path))
            return result;

        var doc = XDocument.Load(path);

        var occupantsRoot = doc.Root?.Element("Occupants");

        var drivers = occupantsRoot.Element("Drivers");


        foreach (var pedElement in drivers.Elements("Ped"))
        {
            string modelName = pedElement.Value.Trim();
            result.Add(new DriverData { ModelName = modelName });
        }

        if (result.Count == 0) Logger.Log("ERROR: DriverData is empty!");

        return result;
    }

    public static Ped SpawnRandom(Vector3 position, float heading = 0f)
    {
        var chosen = CustomizationXml.DriverDatas[MathHelper.GetRandomInteger(CustomizationXml.DriverDatas.Count)];
        return new Ped(chosen.ModelName, position, heading)
        {
            IsPersistent = true,
            IsExplosionProof = true,
            BlockPermanentEvents = true
        };
    }
}
