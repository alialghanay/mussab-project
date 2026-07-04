using UnityEditor;

public static class PrototypeBatchBuild
{
    [MenuItem("Tools/Neighborhood/Batch Build World")]
    public static void BuildWorld()
    {
        NeighborhoodWorldBuilder.Build();
    }
}
