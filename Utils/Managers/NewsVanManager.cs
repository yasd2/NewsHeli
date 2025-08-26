namespace NewsHeli.Utils;

internal class NewsVanManager
{
    internal Vehicle Van { get; set; }
    internal Blip B_Van { get; set; }
    internal Ped Driver { get; set; }
    internal Ped Passenger { get; set; }
    internal Ped Suspect { get; set; }
    internal GameFiber GF_Loop5s { get; set; }
    internal GameFiber GF_Loop { get; set; }


    internal void Create()
    {
        try
        {
            Ped suspect = Functions.GetPursuitPeds(MainFiber.Pursuit).FirstOrDefault();

            Vector3 outputPosition;
            float heading;

            if (Suspect.Exists())
                WorldPos.GetRoadPosWithHeading(World.GetNextPositionOnStreet(suspect.Position.Around2D(70f)), out outputPosition, out heading);
            else
                WorldPos.GetRoadPosWithHeading(World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around2D(70f)), out outputPosition, out heading);



            Van = VanData.SpawnRandom(outputPosition, heading);

            if (!Van.Exists())
            {
                Logger.Log("Van doesn't exist, abort");
                return;
            }
            NativeFunction.Natives.SET_VEHICLE_ON_GROUND_PROPERLY<bool>(Van, 5.0f);

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
            NativeFunction.Natives.SET_​DRIVER_​AGGRESSIVENESS(Driver, 0.0f);


            B_Van = new Blip(Van)
            {
                Sprite = Config.SpriteVan,
                Color = Config.ColorVan,
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

        Ped lastPed = new Ped();

        GF_Loop5s = GameFiber.ExecuteNewWhile(() =>
        {
            var peds = Functions.GetPursuitPeds(MainFiber.Pursuit);
            if (peds == null)
            {
                Logger.Log("pursuit peds are null");
                GameFiber.Wait(5000);
                return;
            }
            else if (peds.Length <= 0)
            {
                Logger.Log("pursuit peds are empty");
                GameFiber.Wait(5000);
                return;
            }



            foreach (Ped ped in peds)
            {
                if (ped.Exists() && ped.IsInAnyVehicle(false) && ped.SeatIndex == -1)
                {
                    if (lastPed != ped)
                    {
                        Logger.Log("New pursuit target ped");
                        lastPed = ped;
                        break;
                    }
                }
            }

            // Native settings to follow suspect
            if (lastPed.Exists() && Driver.Exists() && lastPed.CurrentVehicle.Exists())
            {
                Logger.Log("Van is chasing suspect");

                NativeFunction.Natives.TASK_​VEHICLE_​FOLLOW(Driver, Van, lastPed.CurrentVehicle,
                    MathHelper.ConvertKilometersPerHourToMetersPerSecond(120f),
                    /*VehicleDrivingFlag*/ 828, /*828 = VehicleDrivingFlag.Emergency, but without the Reckless flag - https://vespura.com/fivem/drivingstyle/ */
                    /*DistanceToKeep*/ 40);


                // Alternatives:

                /* TASK_VEHICLE_ESCORT
                   TASK_VEHICLE_FOLLOW
                   TASK_VEHICLE_CHASE 
                   TASK_VEHICLE_MISSION */

                /*NativeFunction.Natives.TASK_VEHICLE_ESCORT(Driver, Van, lastPed.CurrentVehicle,
                    -1,
                    MathHelper.ConvertKilometersPerHourToMetersPerSecond(120f),
                    828,
                    20f,
                    20,
                    40f); looks like this native does the same/*

                // Does NOT work that way, van always pits and rams suspect, even with setting flags, probably I'm setting them wrong
                NativeFunction.Natives.TASK_​VEHICLE_​CHASE(Driver, lastPed);
                NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 32, true);
                NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_IDEAL_PURSUIT_DISTANCE(Driver, 40f);*/
            }
            else
            {
                if (!lastPed) { Logger.Log("lastPed doesn't exist"); GameFiber.Wait(5000); return; }
                if (!Driver) { Logger.Log("Van Driver doesn't exist"); GameFiber.Wait(5000); return; }
                if (!lastPed.CurrentVehicle) { Logger.Log("lastPeds CurrentVehicle doesn't exist"); GameFiber.Wait(5000); return; }
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
