namespace NewsHeli.Utils;

/// <summary>
/// Sets the gta relationship for the media.
/// </summary>
internal class RelationshipManager
{
    public static RelationshipGroup RG_Media = new RelationshipGroup("MEDIA");

    public static void Setup()
    {
        Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, RG_Media, Relationship.Like);
        Game.SetRelationshipBetweenRelationshipGroups(RG_Media, RelationshipGroup.Cop, Relationship.Like);
        Game.SetRelationshipBetweenRelationshipGroups(RG_Media, Game.LocalPlayer.Character.RelationshipGroup, Relationship.Like);
    }
}
