using LucasRitter.Scaleforms;
using LucasRitter.Scaleforms.Generic;

namespace NewsHeli;

// Reference soure code LucasRitter.Scaleforms: https://github.com/LucasRitter/gtav-scaleforms
// HeliView source code: https://github.com/SSStuart/HeliView_LSPDFRPlugin
// This class is a modified version of the original HeliView code

public class HeliView
{
    public static string pluginName = "HeliView";

    static bool ENABLE_OVERLAY => Config.EnableOverlay;
    static bool WARP_PLAYER => Config.WarpPlayerInHeli;
    static string HELI_TYPE => Config.HeliType.ToLower();

    static bool customCameraActive = false;
    static Camera customCamera = new Camera(false);
    static Vehicle heli = null;
    static Ped heliPilot = null;
    static LHandle pursuit;
    static Ped suspect;
    static int suspectIndex = 0;
    static float FOVsuspectLostOffset = 0;
    static Vehicle playerVehicle;
    static bool playerInVehicle = false;
    static bool playerVehicleWasPersistent = false;
    static string currentHeliType = "";
    static Vector3 playerPosition;
    static Scaleform newsScaleform = new Scaleform("breaking_news");
    static HeliCam heliCamScaleform = new HeliCam();
    static uint lastNewsUpdate = 0;

    /// <summary>
    /// Easy short variable for the player Ped.
    /// </summary>
    static Ped Player => Game.LocalPlayer.Character;

    //Initialization of the plugin.
    public static void Initialize()
    {
        if (!Config.EnableHeliView)
        {
            Logger.HeliView("HeliView is disabled, aborting...");
            return;
        }

        Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;

        Game.LogTrivial(pluginName + " Plugin 0.0.1 by SSStuart, implemented in NewsHeli, has been initialised.");
        Game.LogTrivial("[" + pluginName + "] Enable HeliView: " + (Config.EnableHeliView ? "Yes" : "No"));
        Game.LogTrivial("[" + pluginName + "] Enable Overlay: " + (ENABLE_OVERLAY ? "Yes" : "No"));
        Game.LogTrivial("[" + pluginName + "] Warp player in Heli: " + (WARP_PLAYER ? "Yes" : "No"));
        Game.LogTrivial("[" + pluginName + "] Heli type: " + HELI_TYPE);

        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);
    }

    public static void Finally()
    {
        if (!Config.EnableHeliView) return;
        StopHeliPursuit(false, true);
        Game.LogTrivial(pluginName + " has been cleaned up.");
    }

    private static void OnOnDutyStateChangedHandler(bool OnDuty)
    {
        if (OnDuty)
        {
            GameFiber.StartNew(ProcessMenus);

            void ProcessMenus()
            {
                while (true)
                {
                    GameFiber.Yield();

                    // TOGGLE THE CAMERA
                    if (Functions.GetActivePursuit() != null &&
                        ((
                        Config.CamTwoKeys &&
                        (Game.IsKeyDown         (Config.CamKey) && Game.IsKeyDown           (Config.CamModifier)) ||
                        (Game.IsKeyDownRightNow (Config.CamKey) && Game.IsKeyDown           (Config.CamModifier)) ||
                        (Game.IsKeyDown         (Config.CamKey) && Game.IsKeyDownRightNow   (Config.CamModifier))
                        )
                        ||
                        (
                        !Config.CamTwoKeys && Game.IsKeyDown(Config.CamKey)
                        )
                    ))
                    {
                        pursuit = Functions.GetActivePursuit();
                        int nbSuspects = Functions.GetPursuitPeds(pursuit).Length;
                        suspectIndex = Math.Min(suspectIndex, nbSuspects - 1);
                        suspect = Functions.GetPursuitPeds(pursuit)[suspectIndex];
                        // If the camera is not active, start the heli camera
                        if (!customCameraActive)
                            StartHeliPursuit();
                        else
                        {
                            // If the camera is already active : If the shift key is also pressed, switch to the next suspect in the pursuit, else stop the heli camera
                            if (Config.SkipEnabled &&
                                (Game.IsKeyDownRightNow(Config.SkipKey) || Game.IsKeyDown(Config.SkipKey))
                               )
                                SwitchSuspect();
                            else
                                StopHeliPursuit();
                        }
                    }
                    // DETECT END OF PURSUIT & OTHER EVENTS THAT SHOULD STOP THE CAMERA
                    if (customCameraActive &&
                        (!Functions.IsPursuitStillRunning(pursuit)      // Stop if pursuit ended
                        || (heli.Exists() && heli.EngineHealth < 200)   // or Stop if heli is too damaged
                        || Functions.IsPedArrested(suspect)
                        ))
                    {
                        StopHeliPursuit();
                    }

                    // UPDATE CAMERA FOV and OVERLAY
                    if (customCameraActive)
                    {
                        if (ENABLE_OVERLAY)
                        {
                            if (currentHeliType == "news")
                            {
                                // If News heli, display the news overlay
                                string newsText = Config.NewsText;
                                if (suspect && suspect.CurrentVehicle && suspect.IsInAnyVehicle(false))
                                {
                                    // If the suspect is in a vehicle, try to get the vehicle name and display it (only considering model name with letters)
                                    string vehName = suspect.CurrentVehicle.Model.Name;
                                    newsText = newsText.Replace("{VEHICLE}", vehName[0].ToString().ToUpper() + vehName.Substring(1).ToLower());
                                    
                                    newsText = newsText.Replace("{MPH}", Math.Round(MathHelper.ConvertMetersPerSecondToMilesPerHour(suspect.CurrentVehicle.Speed)).ToString());
                                    newsText = newsText.Replace("{KPH}", Math.Round(MathHelper.ConvertMetersPerSecondToKilometersPerHour(suspect.CurrentVehicle.Speed)).ToString());
                                }
                                else
                                {
                                    newsText = newsText.Replace("{VEHICLE}", "Fast car");
                                    newsText = newsText.Replace("{MPH}", "0");
                                    newsText = newsText.Replace("{KPH}", "0");
                                }
                                

                                if (suspect.Exists())
                                {
                                    newsText = newsText.Replace("{FORENAME}", Functions.GetPersonaForPed(suspect).Forename);
                                    newsText = newsText.Replace("{SURNAME}", Functions.GetPersonaForPed(suspect).Surname);
                                    newsText = newsText.Replace("{FULLNAME}", Functions.GetPersonaForPed(suspect).FullName);
                                }
                                else
                                {
                                    newsText = newsText.Replace("{FORENAME}", "Max");
                                    newsText = newsText.Replace("{SURNAME}", "Mustermann");
                                    newsText = newsText.Replace("{FULLNAME}", "Max Mustermann");
                                }
                                
                                // Update the overlay texts with the current area name every 10 seconds
                                if (lastNewsUpdate < Game.GameTime - 1000 * 10)
                                {
                                    newsScaleform.CallFunction("SET_TEXT", newsText, Functions.GetZoneAtPosition(suspect.Position).RealAreaName);
                                    lastNewsUpdate = Game.GameTime;
                                }
                                newsScaleform.Draw();
                            }
                            else if (currentHeliType == "cop")
                            {
                                // If Cop heli, display the Heli overlay
                                // Update the overlay camera parameters and draw it
                                heliCamScaleform.Heading = customCamera.Rotation.Yaw;
                                heliCamScaleform.Altitude = heli.Position.Z;
                                heliCamScaleform.Draw();
                            }
                        }
                        // Zoom in / out depending on the suspect visibility
                        if (suspect.IsRendered)
                            FOVsuspectLostOffset = Math.Max(0, FOVsuspectLostOffset - 0.05f);
                        else
                            FOVsuspectLostOffset = Math.Min(100, FOVsuspectLostOffset + 0.1f);
                        // Update the camera zoom (FOV) depending on the distance to the suspect with some oscillation, and suspect visibility
                        customCamera.FOV = Math.Max(4, (1 / heli.DistanceTo(suspect) * 2000) + (float)(Math.Sin(Game.GameTime / 10000.0) * 5 - 5) + FOVsuspectLostOffset);
                    }
                }
            }
        }
    }

    private static void StartHeliPursuit(bool switching = false)
    {
        // Select heli type (cop / news) according to settings
        currentHeliType = HELI_TYPE == "cop" ? "cop" : (HELI_TYPE == "news" ? "news" : (new Random().Next(2) == 1 ? "cop" : "news"));
        Game.LogTrivial("[" + pluginName + "] StartHeliPursuit with Heli type '" + currentHeliType + "'");

        // Spawn the heli and pilot
        heli = new Vehicle((currentHeliType == "cop" ? "polmav" : "maverick"), suspect.GetOffsetPositionUp(100f), suspect.Heading)
        {
            IsPersistent = true,
            IsDriveable = false,
            IsCollisionEnabled = false
        };
        heliPilot = new Ped("s_m_m_pilot_02", heli.GetOffsetPositionUp(-2f), 0f);
        heliPilot.WarpIntoVehicle(heli, -1);
        heli.Velocity = new Vector3(0, 0, 10f);
        // Make the heli chase the suspect
        heli.Driver.Tasks.ChaseWithHelicopter(suspect, new Vector3(((float)Math.Sin(Game.GameTime / 1000) * 100f), ((float)Math.Sin(Game.GameTime / 1000) * -10f - 20f), 70f));
        // Add the pilot to the pursuit to avoid losing the suspect (if the player is not warped in the heli)
        if (!WARP_PLAYER)
            Functions.AddCopToPursuit(Functions.GetActivePursuit(), heliPilot);
        if (!switching)
        {
            // Save player (and player vehicle) position, make player invincible and remove control
            Game.LocalPlayer.HasControl = false;
            playerPosition = Player.Position;
            Player.IsInvincible = true;
            if (Player.CurrentVehicle != null)
            {
                playerVehicle = Player.CurrentVehicle;
                playerVehicle.IsPositionFrozen = true;
                playerVehicleWasPersistent = playerVehicle.IsPersistent;
                playerVehicle.IsPersistent = true;
                playerInVehicle = true;
            }
            else if (Player.LastVehicle.Exists())
            {
                playerVehicle = Player.LastVehicle;
                playerVehicle.IsPositionFrozen = true;
                playerVehicleWasPersistent = playerVehicle.IsPersistent;
                playerVehicle.IsPersistent = true;
                playerInVehicle = false;
            }
            else
            {
                Player.IsPositionFrozen = true;
                playerVehicle = null;
                playerInVehicle = false;
            }
            // Fade screen out and Hide radar (if overlay enabled)
            Game.FadeScreenOut(500);
            GameFiber.Wait(500);
            if (ENABLE_OVERLAY)
                NativeFunction.Natives.DISPLAY_RADAR(false);
            customCameraActive = true;
        }
        // Warp player in heli (if setting enabled)
        if (WARP_PLAYER)
            Player.WarpIntoVehicle(heli, 0);

        // Setup the custom camera
        customCamera.AttachToEntity(heli, new Vector3(0, 1, -2), true);
        customCamera.PointAtEntity(suspect, new Vector3(), true);
        customCamera.FOV = Math.Min(4, 1 / heli.DistanceTo(suspect) * 1050);
        customCamera.Shake("HAND_SHAKE", 1f);
        customCamera.Active = true;
        heli.IsCollisionEnabled = true; // Re-enable heli collision
        // If player not warped in heli, make the map load arround the suspect
        if (!WARP_PLAYER)
            NativeFunction.Natives.SET_FOCUS_ENTITY(suspect);

        // Fade screen in and display controls help
        GameFiber.Wait(2000);
        Game.FadeScreenIn(500);

        Game.DisplayHelp($"~b~{Config.CamModifier} + {Config.CamKey}~w~ : Exit HeliView\n" +
            $"~b~{Config.CamModifier}+{Config.SkipKey} + {Config.CamKey}~w~ : Toggle suspect");
    }

    private static void SwitchSuspect()
    {
        // Select next suspect in pursuit
        int oldSuspectIndex = suspectIndex;
        int nbSuspects = Functions.GetPursuitPeds(pursuit).Length;
        suspectIndex = (suspectIndex + 1) % nbSuspects;
        Game.DisplaySubtitle("Switching to suspect " + (suspectIndex + 1) + "/" + (nbSuspects));
        if (suspectIndex != oldSuspectIndex && !Functions.GetPursuitPeds(pursuit)[suspectIndex].IsDead)
        {
            // Restart the heli pursuit on the new suspect
            StopHeliPursuit(true);
            StartHeliPursuit(true);
        }
    }

    private static void StopHeliPursuit(bool switching = false, bool immediately = false)
    {
        Game.LogTrivial("[" + pluginName + "] StopHeliPursuit");

        if (!immediately)
        {
            // Fade screen out, and show radar again
            Game.FadeScreenOut(500);
            GameFiber.Wait(500);
            if (ENABLE_OVERLAY)
                NativeFunction.Natives.DISPLAY_RADAR(true);
        }
        if (!switching)
        {
            // If the player had an active vehicle
            if (playerVehicle != null && playerVehicle.Exists())
            {
                // Restore player vehicle attributes, and warp player back in it (if the player was in the vehicle), or just reposition the player on foot
                playerVehicle.IsPositionFrozen = false;
                playerVehicle.IsPersistent = playerVehicleWasPersistent;
                if (WARP_PLAYER && playerInVehicle)
                    Player.WarpIntoVehicle(playerVehicle, -1);
                else if (WARP_PLAYER && !playerInVehicle)
                    Player.Position = playerPosition;
            }
            // If the player had no vehicle, just reposition the player on foot
            else
                Player.Position = playerPosition;
        }
        // Restore player control and attributes
        Player.IsInvincible = false;
        Player.IsPositionFrozen = false;
        // Detach and disable the custom camera
        if (customCamera.Exists())
        {
            customCamera.Active = false;
            customCamera.Detach();
        }
        // Remove the heli and pilot
        if (heli.Exists())
        {
            if (heli.HasDriver)
            {
                Functions.RemovePedFromPursuit(heli.Driver);
                heli.Driver.Delete();
            }
            heli.Delete();
        }
        if (!switching)
        {
            // If the player was not warped in the heli, make the map load arround the player again
            if (!WARP_PLAYER)
                NativeFunction.Natives.SET_FOCUS_ENTITY(Player);
            if (!immediately)
            {
                // Fade screen in
                GameFiber.Wait(500);
                Game.FadeScreenIn(500);
            }
            // Bring back player control
            Game.LocalPlayer.HasControl = true;
            customCameraActive = false;
        }
    }

    public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
    {
        foreach (Assembly assembly in Functions.GetAllUserPlugins())
        {
            if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
            {
                return assembly;
            }
        }
        return null;
    }
}