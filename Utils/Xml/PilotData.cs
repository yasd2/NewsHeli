namespace NewsHeli.Utils;

internal class PilotData
{
    public string ModelName { get; set; }

    public static List<PilotData> GetAllPilots()
    {
        var result = new List<PilotData>();

        string path = @"plugins\LSPDFR\NewsHeli.xml";
        if (!File.Exists(path))
            return result;

        var doc = XDocument.Load(path);
        var pilotRoot = doc.Root?.Element("Pilots");

        if (pilotRoot == null)
            return result;

        foreach (var pedElement in pilotRoot.Elements("Ped"))
        {
            string modelName = pedElement.Value.Trim();
            result.Add(new PilotData { ModelName = modelName });
        }

        if (result.Count == 0) Logger.Log("ERROR: PilotData is empty!");

        return result;
    }

    public static Ped SpawnRandom(Vector3 position, float heading = 0f)
    {
        var chosen = CustomizationXml.PilotDatas[MathHelper.GetRandomInteger(CustomizationXml.PilotDatas.Count)];
        return new Ped(chosen.ModelName, position, heading)
        {
            IsPersistent = true,
            IsExplosionProof = true,
            BlockPermanentEvents = true
        };
    }
}
