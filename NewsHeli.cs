namespace NewsHeli;

internal class NewsHeli
{
    internal static Vehicle Heli;
    internal static Ped Pilot;
    internal static Blip B_Heli;
    internal static LHandle Pursuit;
    internal static Ped Suspect;
    internal static GameFiber GF_Main;

    internal static void Start()
        => GF_Main = GameFiber.StartNew(Main, "NewsHeli_MainFiber");
    

    internal static void Main()
    {
        while (true)
        {
            GameFiber.Yield();

            Pursuit = null;
            Suspect = null;

            // 1. Auf neue Verfolgung warten
            while ((Pursuit = Functions.GetActivePursuit()) == null)
            {
                GameFiber.Yield();
                GameFiber.Wait(1000);
            }

            // 2. 30 Sekunden lang jede Sekunde prüfen, ob die Verfolgung noch läuft
            bool stillRunning = true;
            Logger.Log("Pursuit active!");

            for (int i = 0; i < Config.ArrivalTimeHeli; i++)
            {
                GameFiber.Yield();
                if (!Functions.IsPursuitStillRunning(Pursuit))
                {
                    stillRunning = false;
                    break;
                }
                GameFiber.Wait(1000);
            }

            if (!stillRunning)
            {
                Logger.Log("Pursuit aborted!");
                continue; // Verfolgung wurde abgebrochen → wieder auf neue warten
            }

            // 3. Heli spawnen
            Suspect = Functions.GetPursuitPeds(Pursuit).FirstOrDefault();
            SpawnHeli();
            SpawnHeliBlip();

            GameFiber.StartNew(() =>
            {
                while (Pursuit != null && Functions.IsPursuitStillRunning(Pursuit))
                {
                    GameFiber.Yield();
                    Ped ped = GetDrivingSuspect();
                    if (!Suspect) return;
                    if (Suspect.Model != ped.Model)
                    {
                        Suspect = ped;
                        Logger.Log("Heli Target changed!");
                    }
                    NativeFunction.Natives.TASK_​HELI_​CHASE(Pilot, Suspect, 5f, 5f, 60f);
                    GameFiber.Wait(5000);
                }
            });
            

            bool hasSearchlight = NativeFunction.Natives.DOES_VEHICLE_HAVE_SEARCHLIGHT<bool>(Heli);

            if (hasSearchlight) NativeFunction.Natives.SET_VEHICLE_SEARCHLIGHT(Heli, true, true);


            // audio
            /*GameFiber.StartNew(() =>
            {
                while (Functions.GetIsAudioEngineBusy())
                {
                    GameFiber.Yield();
                    GameFiber.Wait(1000);
                }

                
            });*/


            if (Config.EnableAIDispatch)
            {
                Logger.Log("Playing START audio");
                Functions.PlayScannerAudio("START");
            }


            // 4. Optional: follow logic oder Spotlight
            while (Functions.IsPursuitStillRunning(Pursuit))
            {
                GameFiber.Yield();

                if (!Pilot || !Suspect) return;

                
                // Beispiel: könnte später folgen lassen oder Spotlight nutzen
                if (hasSearchlight)
                    NativeFunction.Natives.SET_MOUNTED_WEAPON_TARGET(Pilot, Suspect, 0, 0f, 0f, 0f, 3, 1);

            }

            // 5. Aufräumen
            DeleteAll();
        }
    }

    internal static void SpawnHeli()
    {
        Vector3 position = Suspect.Exists() ? Suspect.GetOffsetPositionUp(200f).Around2D(150f) : Game.LocalPlayer.Character.GetOffsetPositionUp(200f).Around2D(150f);

        Heli = HeliData.SpawnRandom(position, 0f);

        Pilot = PilotData.SpawnRandom(position.Around2D(10f), 0f);

        if (!Pilot) return;

        Pilot.WarpIntoVehicle(Heli, -1);
    }

    internal static void SpawnHeliBlip()
    {
        if (!Heli) return;

        B_Heli = new Blip(Heli)
        {
            Color = Color.Blue,
            Scale = 0.4f,
            Name = "News-Heli",
        };
    }

    internal static void DeleteAll()
    {
        /*GameFiber.StartNew(() =>
        {
            while (Functions.GetIsAudioEngineBusy())
            {
                GameFiber.Yield();
                GameFiber.Wait(1000);
            }

            
        });*/

        if (Config.EnableAIDispatch)
        {
            Logger.Log("Playing END audio");
            Functions.PlayScannerAudio("END");
        }

        Logger.Log("NewsHeli cleanup");
        if (Heli) Heli.Dismiss();
        if (Pilot) Pilot.Dismiss();
        if (B_Heli) B_Heli.Delete();
    }

    internal static Ped GetDrivingSuspect()
    {
        var peds = Functions.GetPursuitPeds(Pursuit);
        foreach (Ped ped in peds)
        {
            if (!ped) break;

            if (ped.IsAlive && !ped.IsPlayer &&
                ped.IsInAnyVehicle(false) && ped.SeatIndex == -1)
            { return ped; }
        }

        foreach (Ped ped in peds)
        {
            if (!ped) break;

            if (ped.IsAlive && !ped.IsPlayer)
            { return ped; }
        }

        return peds.FirstOrDefault();
    }
}
