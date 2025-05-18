namespace NewsHeli.Utils;

internal class NewsHeliManager
{
    internal Vehicle Heli { get; set; }
    internal Blip B_Heli { get; set; }
    internal Boolean HasSearchLight { get; set; }
    internal Ped Pilot { get; set; }
    internal Ped Suspect { get; set; }
    internal GameFiber GF_Loop5s { get; set; }
    internal GameFiber GF_Loop { get; set; }

    internal void Create()
    {
        try
        {
            Ped suspect = Functions.GetPursuitPeds(MainFiber.Pursuit).FirstOrDefault();
            Vector3 position = suspect.Exists() ? suspect.GetOffsetPositionUp(200f).Around2D(150f) : Game.LocalPlayer.Character.GetOffsetPositionUp(200f).Around2D(150f);

            Heli = HeliData.SpawnRandom(position, 0);

            if (!Heli.Exists())
            {
                Logger.Log("Heli doesn't exist, abort");
                return;
            }

            Pilot = PilotData.SpawnRandom(Heli.Position.Around2D(10f), 0);

            if (!Pilot.Exists())
            {
                Logger.Log("Pilot doesn't exist, abort");
                return;
            }

            Pilot.WarpIntoVehicle(Heli, -1);

            

            B_Heli = new Blip(Heli)
            {
                Color = Color.Blue,
                Scale = 0.4f,
                Name = "News-Heli",
            };

            HasSearchLight = NativeFunction.Natives.DOES_VEHICLE_HAVE_SEARCHLIGHT<bool>(Heli);

            if (HasSearchLight) 
                NativeFunction.Natives.SET_VEHICLE_SEARCHLIGHT(Heli, true, true);


            Loop();
        }
        catch (Exception ex) 
        {
            Logger.Log($"FATAL ERROR NewsHeliManager: {ex}");
        }
    }


    internal void Loop()
    {
        GF_Loop = GameFiber.ExecuteNewWhile(() =>
        {
            NativeFunction.Natives.SET_MOUNTED_WEAPON_TARGET(Pilot, Suspect, 0, 0f, 0f, 0f, 3, 1);
        },
        "",
        () => HasSearchLight && Heli.Exists() && Pilot.Exists() && Suspect.Exists());


        GF_Loop5s = GameFiber.ExecuteNewWhile(() =>
        {
            Suspect = Functions.GetPursuitPeds(MainFiber.Pursuit).FirstOrDefault();

            if (Suspect.Exists() && Pilot.Exists())
            {
                // void TASK_​HELI_​CHASE(Ped pilot, Entity entityToFollow, Vector3 offset
                NativeFunction.Natives.TASK_​HELI_​CHASE(Pilot, Suspect, 20f, 20f, 60f);
            }

            GameFiber.Wait(5000);
        },
        "",
        () => true);
    }


    internal void SafeAbort()
    {
        Logger.Log("Aborting NewsHeliManager safely.");
        GF_Loop.SafeAbort();
        GF_Loop5s.SafeAbort();
        HasSearchLight = false;
        B_Heli.DeleteIfExists();
        Heli.DismissIfExists();
        Pilot.DismissIfExists();
    }


}
