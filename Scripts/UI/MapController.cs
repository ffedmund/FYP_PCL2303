using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerClickHandler
{
    public enum MarkerMode
    {
        Normal,
        Delete,
        Cross,
        Position
    }
    
    readonly float threshold = 0.1f;
    public MarkerMode markerMode;

    [SerializeField] float cursorSpeed = 1;
    [SerializeField] GameObject crossMarkerPrefab;
    [SerializeField] GameObject positionMarkerPrefab;
    [SerializeField] GameObject playerMarkerPrefab;
    [SerializeField] GameObject mapElementPrefab;
    [SerializeField] Button[] markerButtons;
    [SerializeField] Transform viewer;
    RectTransform rectTransform;
    RectTransform playerMarker;
    EndlessTerrain endlessTerrain;
    Dictionary<Vector2,Material> mapDict = new Dictionary<Vector2, Material>();
    Vector2 mapCorrectionVector = new Vector2(1,-1);
    Vector3 viewerInitPosition;
    bool isPointerDown;
    bool isPointerMove;

    public void SetViewer(Transform viewer)
    {
        this.viewer = viewer;
    }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        endlessTerrain = FindAnyObjectByType<EndlessTerrain>();
        playerMarker = Instantiate(playerMarkerPrefab, transform, false).GetComponent<RectTransform>();
    }

    private void Update(){
        if(gameObject.activeSelf && playerMarker && viewer != null){
            Vector3 viewerCoordinate = viewer.position/TerrainGenerationManager.scale/(MapGenerator.mapChunkSize-1);
            playerMarker.anchoredPosition = new Vector2(viewerCoordinate.x,-viewerCoordinate.z) * 360;
            playerMarker.eulerAngles = new Vector3(0,0,-viewer.rotation.eulerAngles.y + 180);
            if((viewerInitPosition - viewer.position).sqrMagnitude >= 360*360)
            {
               OnEnable();
            }
        }
    }

    private void OnEnable() {
        Dictionary<Vector2,Material> tempDict = endlessTerrain.GetTerrainMinimapMaterials();
        Vector3 viewerCoordinate = viewer.position/TerrainGenerationManager.scale/(MapGenerator.mapChunkSize-1);
        rectTransform.anchoredPosition = new Vector2(-viewerCoordinate.x,-viewerCoordinate.z) * 360;
        foreach(Vector2 coordinate in tempDict.Keys){
            if(!mapDict.ContainsKey(coordinate)){
                GameObject mapElement = Instantiate(mapElementPrefab, transform, false);
                RectTransform mapTransform = mapElement.GetComponent<RectTransform>();
                mapTransform.anchoredPosition = coordinate * mapTransform.sizeDelta.x * mapCorrectionVector;
                mapElement.GetComponent<RawImage>().texture = tempDict[coordinate].mainTexture;
            }
        }
        viewerInitPosition = viewer.position;
        if(playerMarker)
        {
            playerMarker.transform.SetAsLastSibling();
        }
        mapDict = tempDict;
    }

    private void OnDisable() {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        isPointerMove = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(isPointerDown && eventData.delta.SqrMagnitude() > threshold*threshold)
        {
            rectTransform.position += new Vector3(eventData.delta.x,eventData.delta.y,0) * cursorSpeed;
            isPointerMove = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isPointerMove)
            return;

        // Convert the mouse position to a position on the UI
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPoint);

        // Instantiate a new sign at the clicked position
        GameObject markerPrefab;
        switch (markerMode){
            case MarkerMode.Cross:
                markerPrefab = crossMarkerPrefab;
                break;
            case MarkerMode.Position:
                markerPrefab = positionMarkerPrefab;
                break;
            case MarkerMode.Delete:
                if(eventData.pointerEnter.tag == "Marker"){
                    Destroy(eventData.pointerEnter);
                }
                return;
            default:
                return;
        }
        GameObject sign = Instantiate(markerPrefab, transform, false);
        sign.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    public void SetMarkerMode(int index){
        this.markerMode = (MarkerMode) index;
        for(int i = 0; i < markerButtons.Length; i++)
        {
            ColorBlock colorBlock = new ColorBlock
            {
                normalColor = new Color(1, 1, 1, i == index ? 0.05f : 0),
                pressedColor = markerButtons[i].colors.pressedColor,
                highlightedColor = markerButtons[i].colors.highlightedColor,
                selectedColor = markerButtons[i].colors.selectedColor,
                disabledColor = markerButtons[i].colors.disabledColor,
                colorMultiplier = 1
            };
            markerButtons[i].colors = colorBlock;
        }
    }

}
