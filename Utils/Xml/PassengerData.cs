namespace NewsHeli.Utils;

internal class PassengerData
{
    public string ModelName { get; set; }

    public static List<PassengerData> GetAllPassengers()
    {
        var result = new List<PassengerData>();

        string path = @"plugins\LSPDFR\NewsHeli.xml";
        if (!File.Exists(path))
            return result;

        var doc = XDocument.Load(path);

        var occupantsRoot = doc.Root?.Element("Occupants");

        var drivers = occupantsRoot.Element("Passengers");


        foreach (var pedElement in drivers.Elements("Ped"))
        {
            string modelName = pedElement.Value.Trim();
            result.Add(new PassengerData { ModelName = modelName });
        }

        if (result.Count == 0) Logger.Log("ERROR: PassengerData is empty!");

        return result;
    }

    public static Ped SpawnRandom(Vector3 position, float heading = 0f)
    {
        var chosen = CustomizationXml.PassengerDatas[MathHelper.GetRandomInteger(CustomizationXml.PassengerDatas.Count)];
        return new Ped(chosen.ModelName, position, heading)
        {
            IsPersistent = true,
            IsExplosionProof = true,
            BlockPermanentEvents = true
        };
    }
}
