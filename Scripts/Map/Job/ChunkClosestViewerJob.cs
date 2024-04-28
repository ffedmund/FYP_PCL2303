using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkClosestViewerJob : IJob
{
    //Input
    [ReadOnly] public NativeArray<float2> viewerPositionArray;
    [ReadOnly] public NativeArray<int2> visibleChunkArray;
    [ReadOnly] public NativeArray<float> sqrDistanceArray;
    [ReadOnly] public NativeArray<bool> isLocalViewerArray;
    [ReadOnly] public int visibleChunkAxisLength;
    [ReadOnly] public int chunkSize;
    [ReadOnly] public int nonLocalViewerVisibleSqrDist;

    //Output
    public NativeHashMap<int2,EndlessTerrain.ClosestViewer> chunkClosestViewerDictionary;


    public void Execute()
    {
        for(int index = 0; index < visibleChunkArray.Length; index++)
        {
            int currentViewerIndex = index / (visibleChunkAxisLength * visibleChunkAxisLength);
            float2 viewerPosition = viewerPositionArray[currentViewerIndex];
            int2 viewedChunkCoordinate = visibleChunkArray[index];
            float sqrDistance = sqrDistanceArray[index];
            bool isLocalViewer = isLocalViewerArray[index];
            
            if (!isLocalViewer && sqrDistance > nonLocalViewerVisibleSqrDist)
            {
                continue;
            }

            EndlessTerrain.ClosestViewer tempViewerData = new EndlessTerrain.ClosestViewer
            {
                closestViewerPosition = viewerPosition,
                sqrDistance = sqrDistance,
                isLocalViewer = isLocalViewer,
                isLocalVisible = chunkClosestViewerDictionary.ContainsKey(viewedChunkCoordinate)?chunkClosestViewerDictionary[viewedChunkCoordinate].isLocalVisible:isLocalViewer
            };

            if (chunkClosestViewerDictionary.ContainsKey(viewedChunkCoordinate) && sqrDistance < chunkClosestViewerDictionary[viewedChunkCoordinate].sqrDistance)
            {
                chunkClosestViewerDictionary[viewedChunkCoordinate] = tempViewerData;
            }
            else if (!chunkClosestViewerDictionary.ContainsKey(viewedChunkCoordinate))
            {
                chunkClosestViewerDictionary.Add(viewedChunkCoordinate, tempViewerData);
            }
        }

    }
}