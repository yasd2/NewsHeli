namespace NewsHeli;

internal static class MainFiber
{
    internal static bool IsPursuitActive;
    internal static LHandle Pursuit;
    internal static uint timeInSeconds;

    internal static GameFiber GF_MainFiber;

    internal static NewsHeliManager NewsHeliManager;
    internal static NewsVanManager NewsVanManager;

    internal static void Start()
    {
        GameFiber.StartNew(() =>
        {
            GameFiber.Wait(7000);
            Logger.Log("NewsHeli MainFiber is now running!");


            GF_MainFiber = GameFiber.ExecuteNewWhile(() =>
            {
                Loop();
            },
            "NewsHeli_MainFiber",
            () => true);
        }, "NewsHeli_DelayStart");
    }



    internal static void Loop()
    {
        if (!IsPursuitActive)
        {
            timeInSeconds = 0;
            Pursuit = null;

            // check if pursuit is running on loop
            while ((Pursuit = Functions.GetActivePursuit()) == null)
            {
                GameFiber.Yield();
                GameFiber.Wait(1000);
                Logger.Log("Pursuit is null");
            }

            
            IsPursuitActive = true;

            
        }

        if (IsPursuitActive)
        {
            // check if pursuit is running on loop
            while (Functions.IsPursuitStillRunning(Pursuit))
            {
                GameFiber.Yield();
                GameFiber.Wait(1000);
                timeInSeconds++;
                Logger.Log($"Pursuit active since {timeInSeconds}s");


                if (timeInSeconds == Config.ArrivalTimeHeli)
                {
                    if (Config.EnableAIDispatch)
                    {
                        Logger.Log($"Playing START audio at {timeInSeconds}s");
                        Functions.PlayScannerAudio("START");
                    }


                    // start heliclass
                    if (Config.EnableHeli)
                    {
                        NewsHeliManager = new NewsHeliManager();
                        NewsHeliManager.Create();
                    }
                }


                if (timeInSeconds == Config.ArrivalTimeVan && Config.EnableVan)
                {
                    // Start Vanclass
                    NewsVanManager = new NewsVanManager();
                    NewsVanManager.Create();
                }
            }


            // delete all
            if (Config.EnableAIDispatch)
            {
                Logger.Log("Playing END audio");
                Functions.PlayScannerAudio("END");
            }

            NewsHeliManager?.SafeAbort();
            NewsVanManager?.SafeAbort();
            // ...

            IsPursuitActive = false;
        }
    }


    internal static void SafeAbort()
    {
        GF_MainFiber.SafeAbort();

        NewsHeliManager?.SafeAbort();
        NewsVanManager?.SafeAbort();
    }
}
