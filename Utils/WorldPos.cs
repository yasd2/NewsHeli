namespace NewsHeli.Utils;

public class WorldPos
{
    public static Vector3 GetRoadPosWithHeading(Vector3 inputPosition, out Vector3 outputPosition, out float heading)
    {
        outputPosition = Vector3.Zero;

        if (NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(inputPosition, out Vector3 vec1, out float headingvar, 1, 3f, 0f))
        {
            heading = headingvar;
            outputPosition = vec1;
            return outputPosition;
        }

        Logger.Log("Failed to get closest vehicle node, using input position.");

        heading = 0;
        return inputPosition;
    }
}
