namespace NewsHeli.Utils;

internal class Menu
{
    internal static RNUIMenu RNUIMenu;
    internal static Ped Ped1;
    internal static Ped Ped2;
    internal static Object Camera;
    internal static Object Mic;
    internal static bool IsMediaOnScene;
    internal static GameFiber GF_ProcessMenu;

    internal static void Start()
    {
        if (!File.Exists("RageNativeUI.dll"))
        {
            Game.DisplayNotification("NewsHeli: You don't have RageNativeUI.dll, the optional Menu won't start.");
            Logger.Log("NewsHeli: You don't have RageNativeUI.dll, the optional Menu won't start.");
            return;
        }

        RNUIMenu = new RNUIMenu("NewsHeli", "", "",
        [
            ("Call Media", "Calls media on scene",
                ()=>
                {
                    if (IsMediaOnScene) { Logger.Log("Media already on scene!");
                        Game.DisplayNotification("Media already on scene!"); return; }

                    IsMediaOnScene = true;

                    Ped1 = new Ped("cs_paper", Game.LocalPlayer.Character.Position.Around2D(5f), 0f);
                    Ped1.IsPersistent = true;
                    Ped1.BlockPermanentEvents = true;

                    Ped2 = new Ped("S_M_Y_Grip_01", Game.LocalPlayer.Character.Position.Around2D(5f), 0f);
                    Ped2.IsPersistent = true;
                    Ped2.BlockPermanentEvents = true;

                    NativeFunction.Natives.TASK_​TURN_​PED_​TO_​FACE_​ENTITY(Ped1, Ped2, -1);
                    NativeFunction.Natives.TASK_​TURN_​PED_​TO_​FACE_​ENTITY(Ped2, Ped1, -1);

                    Camera = new Object("prop_v_cam_01", Ped2.Position.Around2D(10f));
                    Camera.AttachTo(Ped2, 28422, new Vector3(0.21f, 0.03f, -0.01f), new Rotator(90f, 176f, 80f));

                    // Animation laden
                    new AnimationDictionary("missfinale_c2mcs_1").LoadAndWait();
                    Ped2.Tasks.PlayAnimation("missfinale_c2mcs_1", "fin_c2_mcs_1_camman", 8.0f, AnimationFlags.Loop | AnimationFlags.SecondaryTask);

                    Mic = new Object("p_ing_microphonel_01", Ped1.Position.Around2D(10f));

                    // An linke Hand anheften (Bone 26611 = IK_L_Hand)
                    Mic.AttachTo(Ped1, 26611, new Vector3(0.05f, -0.02f, 0.04f), new Rotator(338f, 3f, 351f));

                    // Passende Reporter-Animation laden und starten
                    new AnimationDictionary("anim@heists@humane_labs@finale@keycards").LoadAndWait();
                    Ped1.Tasks.PlayAnimation("anim@heists@humane_labs@finale@keycards", "ped_a_enter_loop", 8.0f, AnimationFlags.Loop);  
                }
            ),

            ("Dismiss Media", "Make Media team leave",
                ()=>
                {
                    Camera.DeleteIfExists();
                    Mic.DeleteIfExists();

                    if (Ped1)
                    {
                        Ped1.Tasks.ClearImmediately();
                        Ped1.Dismiss();
                    }

                    if (Ped2)
                    {
                        Ped2.Tasks.ClearImmediately();
                        Ped2.Dismiss();
                    }

                    IsMediaOnScene = false;
                })
        ],
        Keys.D8);


        GF_ProcessMenu = GameFiber.ExecuteNewWhile(()=> RNUIMenu.ProcessMenu(), "NewsHeliMenu", ()=> true);
    }


    internal static void SafeAbort()
    {
        GF_ProcessMenu.SafeAbort();
    }
}
