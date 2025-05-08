using Rage;
using System.Drawing;

namespace NewsHeli.Utils;

internal class NewsVanManager
{
    internal Vehicle Van;
    internal Blip B_Van;
    internal Ped Driver, Passenger;

    internal Ped Suspect;

    internal GameFiber GF_Loop5s;
    internal GameFiber GF_Loop;

    internal void Create()
    {
        try
        {
            Ped suspect = Functions.GetPursuitPeds(MainFiber.Pursuit).FirstOrDefault();
            Vector3 pos = suspect.Exists() ? World.GetNextPositionOnStreet(suspect.Position.Around2D(70f)) : World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around2D(70f));

            Vector3 position = new Vector3(pos.X, pos.Y, World.GetGroundZ(pos, false, false) ?? pos.Z);

            Van = VanData.SpawnRandom(position, 0);
            
            if (!Van.Exists())
            {
                Logger.Log("Van doesn't exist, abort");
                return;
            }
            Van.TopSpeed = MathHelper.ConvertKilometersPerHourToMetersPerSecond(120f);


            Driver = DriverData.SpawnRandom(Van.Position.Around2D(10f), 0);

            if (!Driver.Exists())
            {
                Logger.Log("Driver doesn't exist, abort");
                return;
            }


            Passenger = PassengerData.SpawnRandom(Van.Position.Around2D(10f), 0);

            if (!Passenger.Exists())
            {
                Logger.Log("Passenger doesn't exist, abort");
                return;
            }


            Driver.WarpIntoVehicle(Van, -1);
            Passenger.WarpIntoVehicle(Van, 0);

            NativeFunction.Natives.SET_​DRIVER_​ABILITY(Driver, 1.0f);
            NativeFunction.Natives.SET_​DRIVER_​AGGRESSIVENESS(Driver, 0.2f);


            B_Van = new Blip(Van)
            {
                Color = Color.Green,
                Scale = 0.4f,
                Name = "News-Van",
            };


            Loop();
        }
        catch (Exception ex) 
        {
            Logger.Log($"FATAL ERROR NewsVanManager: {ex}");
        }
    }


    internal void Loop()
    {
        /*GF_Loop = GameFiber.ExecuteNewWhile(() =>
        {
            
        },
        "",
        () => Van.Exists() && Driver.Exists() && Suspect.Exists());*/


        GF_Loop5s = GameFiber.ExecuteNewWhile(() =>
        {
            var peds = Functions.GetPursuitPeds(MainFiber.Pursuit);
            if (peds == null)
            {
                Logger.Log("pursuit peds are null");
                return;
            }
            else if (peds.Length <= 0)
            {
                Logger.Log("pursuit peds are empty");
                return;
            }


            Ped target = new Ped();

            foreach (Ped ped in peds)
            {
                if (ped.Exists() && ped.IsInAnyVehicle(false) && ped.SeatIndex == -1)
                {
                    Logger.Log("New pursuit target ped");
                    target = ped;
                    break;
                }
            }

            // native, um suspect zu folgen!!!!
            if (target.Exists() && Driver)
            {
                Logger.Log("Van is chasing suspect");
                //NativeFunction.Natives.TASK_​VEHICLE_​CHASE(Driver, target);

                /*NativeFunction.Natives.TASK_​VEHICLE_​MISSION_​PED_​TARGET(Driver, Van, target, 
                    7, // follow
                    MathHelper.ConvertKilometersPerHourToMetersPerSecond(120f),
                    (uint)VehicleDrivingFlags.Emergency,
                    40f, 35f, false);*/

                NativeFunction.Natives.TASK_​VEHICLE_​FOLLOW(Driver, Van, target,
                    MathHelper.ConvertKilometersPerHourToMetersPerSecond(120f),
                    (uint)VehicleDrivingFlags.Emergency,
                    45f);

                //NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_IDEAL_PURSUIT_DISTANCE(Driver, 40f);
                //NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 32, true);

            }
            else
            {
                Logger.Log("target doesn't exist");
            }


            GameFiber.Wait(5000);
        },
        "NewsHeli_5sLoop",
        () => true);
    }


    internal void SafeAbort()
    {
        Logger.Log("Aborting NewsVanManager safely.");
        GF_Loop.SafeAbort();
        GF_Loop5s.SafeAbort();
        B_Van.DeleteIfExists();
        Van.DismissIfExists();
        Driver.DismissIfExists();
        Passenger.DismissIfExists();
    }


}
