using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rage.Native.NativeFunction;

namespace NewsHeli.Utils;

/// <summary>
/// Open source, all natives can be found here: https://nativedb.dotindustries.dev/gta5/natives
/// </summary>
internal static class VehicleChase
{
    public static void TASK_​VEHICLE_​CHASE(Ped driver, Entity targetEnt)
        => Natives.TASK_​VEHICLE_​CHASE(driver, targetEnt);

    public static void SET_​TASK_​VEHICLE_​CHASE_​BEHAVIOR_​FLAG(Ped ped, int flag, bool set)
        => Natives.SET_​TASK_​VEHICLE_​CHASE_​BEHAVIOR_​FLAG(ped, flag, set);

    public static void SET_​TASK_​VEHICLE_​CHASE_​BEHAVIOR_​FLAG(Ped ped, EVehicleChaseFlag flag, bool set)
        => Natives.SET_​TASK_​VEHICLE_​CHASE_​BEHAVIOR_​FLAG(ped, (int)flag, set);

    public static void SetVehicleChaseBehaviorFlags(Ped ped, params EVehicleChaseFlag[] flags)
    {
        foreach (var flag in flags)
        {
            SET_​TASK_​VEHICLE_​CHASE_​BEHAVIOR_​FLAG(ped, flag, true);
        }
    }

    public enum EVehicleChaseFlag
    {
        Ramming_Aggressive = 1,
        Ramming_Medium = 2,
        MediumBoxingWithPit = 8,
        Ramming_Soft = 16,
        Convoy_StayBack = 32,
    }

    public static void SET_​TASK_​VEHICLE_​CHASE_​IDEAL_​PURSUIT_​DISTANCE(Ped ped, float distance)
        => Natives.SET_​TASK_​VEHICLE_​CHASE_​IDEAL_​PURSUIT_​DISTANCE(ped, distance);

    public static void TASK_​VEHICLE_​ESCORT(
        Ped ped,
        Vehicle vehicle,
        Vehicle targetVehicle,
        int mode,
        float speed,
        int drivingStyle,
        float minDistance,
        int minHeightAboveTerrain,
        float noRoadsDistance
)
        => Natives.TASK_​VEHICLE_​ESCORT(ped, vehicle, targetVehicle, mode, speed, drivingStyle, minDistance, minHeightAboveTerrain, noRoadsDistance);

    /// <summary>
    /// Doesnt work, seems to be more complicated
    /// </summary>
    /// <param name="Driver"></param>
    public static void SetupChaseFlags(Ped Driver)
    {
        //NativeFunction.Natives.TASK_​VEHICLE_​CHASE(Driver, lastPed);

        //NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 32, true);
        //NativeFunction.Natives.SET_TASK_VEHICLE_CHASE_IDEAL_PURSUIT_DISTANCE(Driver, 12f);

        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 1, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 2, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 4, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 8, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 16, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 32, false);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 64, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 128, false);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 256, true);
        VehicleChase.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG(Driver, 512, true);
    }
}
