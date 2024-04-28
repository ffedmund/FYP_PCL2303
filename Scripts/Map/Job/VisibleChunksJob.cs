using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct VisibleChunksJob : IJobParallelFor
{
    //Input
    [ReadOnly]
    public NativeArray<float2> viewerPositionArray;
    [ReadOnly]
    public float2 localViewer;
    [ReadOnly]
    public int visibleChunkAxisLength;
    [ReadOnly]
    public int chunkVisibleInViewDist;
    [ReadOnly]
    public int chunkSize;
    
    //Ouput
    public NativeArray<int2> visibleChunkArray;
    public NativeArray<bool> isLocalViewerArray;
    public NativeArray<float> sqrDistanceArray;


    public void Execute(int index)
    {
        int currentViewerIndex = index / (visibleChunkAxisLength * visibleChunkAxisLength);
        int actualIndex = index - currentViewerIndex * visibleChunkAxisLength * visibleChunkAxisLength;

        float2 viewerPosition = viewerPositionArray[currentViewerIndex];
        int2 viewerCoordinate = new int2(math.round(viewerPosition / chunkSize));
        bool isLocalViewer = viewerPosition.x == localViewer.x && viewerPosition.y == localViewer.y;
        int x = actualIndex % visibleChunkAxisLength - chunkVisibleInViewDist;
        int y = actualIndex / visibleChunkAxisLength - chunkVisibleInViewDist;

        visibleChunkArray[index] = viewerCoordinate + new int2(x,y);
        isLocalViewerArray[index] = isLocalViewer;
        sqrDistanceArray[index] = math.distancesq(visibleChunkArray[index] * chunkSize, viewerPosition);
    }
}