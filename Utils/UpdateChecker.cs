// Thanks Charlie for the UpdateChecker code! :)
using System.Net;
using System.Threading;

namespace NewsHeli.Utils;

/// <summary>
/// This is an update checker, checks on lcpdfr.com if an update is available.
/// </summary>
internal class UpdateChecker
{
    internal static (bool UpdateAvailable, bool LatestVersion, bool FutureVersion, bool Error) UpdateResult;

    public static void Check()
    {
        void RunCheck()
        {
            var Thread = new Thread(CheckLatestVersion);
            Thread.Start();

            GameFiber.WaitUntil(() => !Thread.IsAlive);

            if (UpdateResult.LatestVersion)
            {
                // NewsHeli is on the ~g~latest version.
            }
            else if (UpdateResult.FutureVersion)
            {
                Game.DisplayNotification("DIA_PILOT", "DIA_PILOT", "NewsHeli", "~g~UpdateChecker", 
                    "~r~BETA detected.~w~~n~Thank you for testing NewsHeli!");
            }
            else if (UpdateResult.UpdateAvailable)
            {
                Game.DisplayNotification("CHAR_LESTER_DEATHWISH", "CHAR_LESTER_DEATHWISH", "NewsHeli", "~g~UpdateChecker", 
                    "~r~Old version detected.~w~~n~Update when possible!");
            }
            else
            {
                // NewsHeli loaded~w~ but we ~r~couldn't check~w~ for updates.
            }
        }

        GameFiber.StartNew(RunCheck, "UpdateChecker NewsHeli");
    }

    /// <summary>
    /// Don't use natives here, it's is called in a C# Thread!
    /// </summary>
    public static void CheckLatestVersion()
    {
        var userVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string FileID = "51090";
        using (var webClient = new WebClient())
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                webClient.Headers.Add("user-agent", "NewsHeli");
                var receivedString = webClient.DownloadString($"https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId={FileID}&textOnly=1").Trim();
                if (receivedString == userVersion)
                {
                    UpdateResult.LatestVersion = true;
                    Logger.Log("Latest version of NewsHeli detected.");
                }
                else if (string.Compare(receivedString, userVersion) < 0)
                {
                    UpdateResult.FutureVersion = true;
                    Logger.Log("Beta/ Future version of NewsHeli detected.");
                }
                else
                {
                    UpdateResult.UpdateAvailable = true;
                    Logger.Log($"The user is on an outdated. {receivedString} > {userVersion}");
                }
            }
            catch (WebException)
            {
                UpdateResult.Error = true;
                Logger.Log("We were unable to check for an update.");
            }
        }
    }
}