using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{   
    #region Singleton
    public static DayCycleManager instance;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
    }
    #endregion

    [System.Serializable]
    public struct SkyColor{
        public Color skyColor;
        public Color lightColor;
        public Material skyBoxMaterial;
        [Range(0,360)]
        public float changeTime;
        [Range(-1,5)]
        public float atmosphereThickness;
        [Range(-1,1)]
        public float sunSize;
        public float sunSizeChangeTime;
        public bool doExposureFadeIn;
        public bool useSkybox;
        [Header("Other Setting")]
        public bool setFogColor;
        public Color fogColor;
    }
    public float lightIntensity;
    public float time;
    public float currentTime;
    [Range(0,10)] 
    public int speed;
    public SkyColor[] skyColors;
    [SerializeField] string clockString;
    [SerializeField] Transform lightSource;
    new Light light;

    int pivot;
    int previousPivot;

    // Start is called before the first frame update
    void Start()
    {
        light = lightSource.GetComponent<Light>();
        pivot = -1;
        previousPivot = -2;
        ClockTimeToTime();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(speed == 0){
            return;
        }
        time += Time.fixedDeltaTime*speed/2;

        currentTime = time%360;
        UpdateLightIntensity();
        UpdateLightAngle();
        UpdateClockTime();
        UpdateSky();
    }

    void UpdateSky(){
        if(skyColors.Length > 0){
            for(int i = 0; i < skyColors.Length; i++){
                if(skyColors[i].changeTime<=currentTime){
                    pivot = i;
                }else{
                    break;
                }
            }
            if(pivot != previousPivot){
                if(skyColors[pivot].sunSizeChangeTime > 0){
                    skyColors[pivot].skyBoxMaterial.DOFloat(skyColors[pivot].sunSize,"_SunSize",skyColors[pivot].sunSizeChangeTime*2/speed);
                }else if(skyColors[pivot].sunSizeChangeTime == 0){
                    skyColors[pivot].skyBoxMaterial.SetFloat("_SunSize",skyColors[pivot].sunSize);
                }
                if(skyColors[pivot].doExposureFadeIn){
                    RenderSettings.skybox = skyColors[pivot].skyBoxMaterial;
                    RenderSettings.skybox.SetFloat("_Exposure",0);
                    RenderSettings.skybox.SetColor("_SkyTint",skyColors[pivot].skyColor);
                    RenderSettings.skybox.DOFloat(1,"_Exposure",10/speed);
                }else{
                    RenderSettings.skybox.DOColor(skyColors[pivot].skyColor,"_SkyTint",10/speed).OnComplete(()=>{
                        skyColors[pivot].skyBoxMaterial.SetColor("_SkyTint",skyColors[pivot].skyColor);
                        if(skyColors[pivot].useSkybox){
                            RenderSettings.skybox = skyColors[pivot].skyBoxMaterial;
                        }
                    });
                }
                if(skyColors[pivot].setFogColor){
                    DOTween.To(()=>RenderSettings.fogColor,x=>RenderSettings.fogColor=x,skyColors[pivot].fogColor,10);
                }   
                if(skyColors[pivot].atmosphereThickness > 0){
                    RenderSettings.skybox.DOFloat(skyColors[pivot].atmosphereThickness,"_AtmosphereThickness",10/speed);
                }
                light.DOColor(skyColors[pivot].lightColor,5/speed);
                previousPivot = pivot;
            }
        }
    }

    void UpdateClockTime(){
        int hour = (int)currentTime/15;
        int minute = Mathf.FloorToInt(currentTime - hour*15)*4;
        clockString = $"{(hour<10?"0":"")}{hour}:{(minute<10?"0":"")}{minute}";
        UpdateLightIntensity();
        UpdateLightAngle();
        UpdateSky();
    }

    void ClockTimeToTime(){
        if(clockString == ""){
            return;
        }
        bool isHour = true;
        string hourString = "";
        string minuteString = "";
        foreach(char character in clockString){
            if(isHour){
                if(character == ':'){
                    isHour = false;
                    continue;
                }
                hourString += character;
            }else{
                minuteString += character;
            }
        }
        Debug.Log(hourString+" " +minuteString);
        time = short.Parse(hourString)*15+short.Parse(minuteString)/4;
    }

    void UpdateLightIntensity(){
        light.intensity = Mathf.Lerp(0,lightIntensity,(-Mathf.Cos(Mathf.Deg2Rad*currentTime)+1)/2);
    }

    void UpdateLightAngle(){
        Vector3 targetAngle = new Vector3((currentTime+90)%180,-90,0);
        lightSource.eulerAngles = targetAngle;
    }

    public void SetTime(string newTime){
        clockString = newTime;
        ClockTimeToTime();
    }
}
