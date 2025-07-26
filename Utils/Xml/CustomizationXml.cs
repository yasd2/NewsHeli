namespace NewsHeli.Utils;

internal class CustomizationXml
{
    public static List<HeliData> HeliDatas = [];

    public static List<PilotData> PilotDatas = [];

    public static List<VanData> VanDatas = [];

    public static List<DriverData> DriverDatas = [];

    public static List<PassengerData> PassengerDatas = [];

    /// <summary>
    /// This reads all the customization data from the .xml settings file.
    /// </summary>
    public static void Read()
    {
        HeliDatas = HeliData.GetAllHelicopters();

        PilotDatas = PilotData.GetAllPilots();

        VanDatas = VanData.GetAllVans();

        DriverDatas = DriverData.GetAllDrivers();

        PassengerDatas = PassengerData.GetAllPassengers();

        Logger.Log("XmlDatas successfully read");
    }

}
