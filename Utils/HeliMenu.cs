/*
using NewsHeli;
using Rage;
using Rage.Native;
using RAGENativeUI;
using System.Drawing;

internal class HeliMenu
{
    public RNUIMenu Menu { get; set; }
    public Vehicle Heli { get; set; }
    public Ped Pilot { get; set; }
    public Blip B_FlyDestination { get; set; }
    public Ped DestinationPed { get; set; }
    public GameFiber GF_LightOnPed_Loop { get; set; }

    public void Setup()
    {
        Menu = new RNUIMenu("HeliMenu", "Options", "Fly to the moon!",
            [
                new("Spawn Heli", "Spawns the heli with a pilot!",
                    () =>
                    {
                        Heli = new HeliData().SpawnRandom(Game.LocalPlayer.Character.GetOffsetPositionUp(200f));

                        string[] pilots =
                        [
                            "s_m_y_pilot_01",
                            "s_m_m_pilot_02",
                        ];

                        Pilot = new Ped(pilots[MathHelper.GetRandomInteger(pilots.Length)], Vector3.Zero, 0f)
                        {
                            IsPersistent = true,
                            BlockPermanentEvents = true,
                        };

                        Pilot.WarpIntoVehicle(Heli, -1);
                    }
                ),

                new("Enter Heli", "Teleports you to the passenger seat",
                    () =>
                    {
                        if (Heli)
                            Globals.Player.WarpIntoVehicle(Heli, 0);
                    }
                ),

                new("~r~Fly to waypoint!", "Pilot flies to waypoint",
                    () =>
                    {
                        if (!NativeFunction.Natives.IS_WAYPOINT_ACTIVE<bool>())
                        {
                            Game.DisplayNotification("You need to set a waypoint first!");
                            return;
                        }

                        DestinationPed = new MCPed(MCModels.PedModels.UseRandomValidModel(), GetWaypointPosition());

                        if (Pilot.Exists() && DestinationPed.Exists())
                        {
                            NativesHeli.TASK_HELI_CHASE(Pilot, DestinationPed, 0, 0, 20);

                            B_FlyDestination.DeleteIfExists();
                            B_FlyDestination = new Blip(DestinationPed)
                            {
                                Sprite = BlipSprite.Friend,
                                Color = Color.BlueViolet,
                                Scale = 1.2f,
                                IsRouteEnabled = true,
                            };

                            NativesHeli.SET_VEHICLE_SEARCHLIGHT(Heli);
                        }
                    }
                ),

                new("~g~Lights on Ped", "",
                    () =>
                    {
                        GF_LightOnPed_Loop = GameFiber.ExecuteNewWhile(() =>
                            NativeFunction.Natives.SET_MOUNTED_WEAPON_TARGET(Pilot, DestinationPed, 0, 0f, 0f, 0f, 3, 1),
                        () => true);
                    }
                ),

                new("~b~Rappel", "",
                    () =>
                    {
                        if (Heli)
                        {
                            Globals.Player.WarpIntoVehicle(Heli, new int[]{1, 2}.UseRandom());

                            if (NativeFunction.Natives.DOES_VEHICLE_ALLOW_RAPPEL<bool>(Heli))
                            {
                                if (Globals.Player.SeatIndex is 1 or 2)
                                {
                                    NativeFunction.Natives.TASK_​RAPPEL_​FROM_​HELI(Globals.Player, 10f);
                                }
                                else Helper.Log("Player is not in rear seat");
                            }
                            else Helper.Log("Can't rappel from that vehicle");
                        }
                    }
                ),

                new("Land Heli", "",
                    () =>
                    {
                        if (Heli)
                        {
                            NativeFunction.Natives.TASK_HELI_MISSION(Pilot, Heli, 0, 0, DestinationPed.Position, (int)NativesHeli.EMissionFlag.MISSION_LAND, 30f, 30f, DestinationPed.Heading, -1, -1, -1f, (int)NativesHeli.EBehaviorFlags.HF_NONE);
                        }
                    }
                ),

                new("End", "Clears everything related",
                    () =>
                    {
                        GF_LightOnPed_Loop.SafeAbort();
                        Heli.DeleteIfExists();
                        Pilot.DeleteIfExists();
                        B_FlyDestination.DeleteIfExists();
                        DestinationPed.DeleteIfExists();

                        Vector3 playerPos = Globals.Player.Position;
                        Globals.Player.Position = new Vector3(playerPos.X, playerPos.Y, World.GetGroundZ(playerPos, false, false) ?? World.GetGroundZ(playerPos, false, false) ?? 70f);
                    }
                ),
            ]);
    }


    public static Vector3 GetWaypointPosition()
    {
        int waypointBlip = NativeFunction.Natives.GET_FIRST_BLIP_INFO_ID<int>(8); // GET_FIRST_BLIP_INFO_ID with type 8 (Waypoint)
        if (waypointBlip != 0)
        {
            return NativeFunction.Natives.GET_BLIP_INFO_ID_COORD<Vector3>(waypointBlip); // GET_BLIP_INFO_ID_COORD
        }

        Helper.Log("Error in getting waypoint");
        return Vector3.Zero; // Kein Waypoint gesetzt
    }
}*/