using Rage;
using Rage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsHeli.Utils;

internal class ConsoleCommands
{
    [ConsoleCommand]
    public static void CreateCameraMan()
    {
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

    [ConsoleCommand]
    public static void DeleteAllStuff()
    {
        foreach (var entity in World.GetAllEntities().Where(x => x.DistanceTo(Player) <= 30f && (x is Ped or is Object)).ToList())
        {
            entity.Delete();
        }
    }

    private static Ped Player => Game.LocalPlayer.Character;
    internal static Ped Ped1, Ped2;
    internal static Object Camera, Mic;
}
