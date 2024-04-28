using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using JohnStairs.RCC.Character;

[System.Serializable]
public struct AudioClipData{
    public AudioType audioType;
    public string clipName;
    public AudioClip audio;
    public float volume;
}

public enum AudioType{
    SFX,
    BGM
}

public class AudioSourceController : MonoBehaviour
{
    [SerializeField] AudioClipData[] audioClips;
    public static AudioSourceController instance;
    public int agentNumber = 5;
    public List<AudioSource> audioAgents = new List<AudioSource>();
    public string currentBGMPlaylist;
    Dictionary<string,AudioClipData> _audioDict = new Dictionary<string, AudioClipData>();
    Dictionary<string,List<AudioClipData>> _bgmPlaylist = new Dictionary<string, List<AudioClipData>>();
    List<AudioSource> busyAgents;
    public AudioSource bgmAudioSource;
    Coroutine bgmFadeIn;
    Coroutine bgmFadeOut;
    float bgmVolume;
    float sfxVolume;
    void Awake(){
        instance = this;
        bgmVolume = 1;
        sfxVolume = 1;
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        }else{
            PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        }else{
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }
        PlayerPrefs.Save();
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < agentNumber; i++){
            GameObject agent = new GameObject();
            agent.name = $"AudioAgent{i}";
            agent.transform.SetParent(this.transform);
            audioAgents.Add(agent.AddComponent<AudioSource>());
            audioAgents[i].playOnAwake = false;
            audioAgents[i].spatialBlend = 0.5f;
        }
        foreach(AudioClipData audioClipData in audioClips){
            if(!_audioDict.ContainsKey(audioClipData.clipName)){
                _audioDict.Add(audioClipData.clipName,audioClipData);
                if(audioClipData.audioType == AudioType.BGM){
                    string pattern = @"[a-zA-Z]+";  // Matches one or more characters

                    Match match = Regex.Match(audioClipData.clipName, pattern);

                    if (match.Success)
                    {
                        string result = match.Value;  // 'main'
                        // Debug.Log(result);
                        if(_bgmPlaylist.ContainsKey(result)){
                            _bgmPlaylist[result].Add(audioClipData);
                        }else{
                            _bgmPlaylist.Add(result,new List<AudioClipData>());
                            _bgmPlaylist[result].Add(audioClipData);
                        }
                    }
                }
            }
        }
        busyAgents = new List<AudioSource>();
        if(bgmAudioSource == null)
        {
            FindAnyObjectByType<AudioListener>().TryGetComponent(out bgmAudioSource);
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if(busyAgents.Count > 0){
            List<AudioSource> temp = new List<AudioSource>();
            foreach(AudioSource agent in busyAgents){
                if(!agent.isPlaying){
                    temp.Add(agent);
                }
            }
            foreach(AudioSource agent in temp){
                busyAgents.Remove(agent);
                audioAgents.Add(agent);
            }
        }
        if(!bgmAudioSource.isPlaying && currentBGMPlaylist != "" && currentBGMPlaylist != null && _bgmPlaylist.ContainsKey(currentBGMPlaylist)){
            SetBGM(_bgmPlaylist[currentBGMPlaylist][Random.Range(0,_bgmPlaylist[currentBGMPlaylist].Count)].clipName,Random.Range(5,20));
        }
    }

    AudioSource GetFreeAgent(){
        if(audioAgents.Count > 0){
            AudioSource freeAgent = audioAgents[0];
            audioAgents.RemoveAt(0);
            return freeAgent; 
        }
        return null;
    }

    public void Play(string name, Transform transform){
        if(_audioDict.ContainsKey(name)){
            AudioSource freeAgent = GetFreeAgent();
            if(freeAgent == null) return;
            freeAgent.clip = _audioDict[name].audio;
            freeAgent.volume = GetVolume(name);
            if(transform){
                freeAgent.transform.position = transform.position;
            }
            freeAgent.Play();
            busyAgents.Add(freeAgent);
        }
    }

    public void Play(int index, Transform transform){
        if(audioClips.Length > index){
            AudioSource freeAgent = GetFreeAgent();
            if(freeAgent == null) return;
            freeAgent.clip = audioClips[index].audio;
            freeAgent.volume = GetVolume(index);
            if(transform){
                freeAgent.transform.position = transform.position;
            }
            freeAgent.Play();
            busyAgents.Add(freeAgent);
        }
    }

    public void Play(string name){
        bgmAudioSource.PlayOneShot(_audioDict[name].audio,GetVolume(name));
    }


    public void SetBGM(string name,float delay){
        if(_audioDict.ContainsKey(name) && bgmAudioSource.clip != _audioDict[name].audio){
            if(bgmFadeOut != null){
                StopCoroutine(bgmFadeOut);
                bgmFadeOut = null;
            }
            if(bgmFadeIn != null){
                StopCoroutine(bgmFadeIn);
                bgmFadeIn = null;
            }

            if(bgmAudioSource.isPlaying){
                StartCoroutine(ChangeBGM(name));
            }else{
                float fadeInTime = 4;
                bgmAudioSource.volume = 0;
                bgmAudioSource.clip = _audioDict[name].audio;
                bgmAudioSource.PlayDelayed(delay);
                bgmFadeIn = StartCoroutine(FadeIn(bgmAudioSource,GetVolume(name),delay,fadeInTime));
                bgmFadeOut = StartCoroutine(FadeOut(bgmAudioSource,bgmAudioSource.clip.length-4,4));
            }
        }
    }

    public void FadeOutBGM(string name, float fadeTime){
        AudioClip audioClip = null;
        for(int i = 0; i < audioClips.Length; i ++){
            if(audioClips[i].clipName == name){
                audioClip = audioClips[i].audio;
            }
        }
        if(audioClip && audioClip == bgmAudioSource.clip){
            FadeOutBGM(fadeTime);
        }
    }

    public void SetPlayerPrefs(){
        if(bgmAudioSource.volume != 0){
            bgmAudioSource.volume /= bgmVolume; 
        }else{
            bgmAudioSource.volume = 0.15f;
        }
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        bgmAudioSource.volume *= bgmVolume; 
    }

    public void FadeOutBGM(float fadeTime){
        if(bgmAudioSource.isPlaying){
            if(bgmFadeOut != null){
                StopCoroutine(bgmFadeOut);
                bgmFadeOut = null;
            }
            bgmFadeOut = StartCoroutine(FadeOut(bgmAudioSource,0,fadeTime));
        }
    }

    IEnumerator ChangeBGM(string name){
        yield return StartCoroutine(FadeOut(bgmAudioSource,0,2,false));
        bgmAudioSource.clip = _audioDict[name].audio;
        bgmAudioSource.volume = 0;
        bgmAudioSource.Play();
        yield return StartCoroutine(FadeIn(bgmAudioSource,_audioDict[name].volume,0,4));
        yield return null;
    }

    IEnumerator FadeOut(AudioSource audioSource,float delay,float fadeTime,bool stopAudio = true) 
    {
        yield return new WaitForSeconds(delay);
        float timeElapsed = 0;
        float originalVolume = audioSource.volume;

        while (audioSource.volume > 0) 
        {
            audioSource.volume = Mathf.Lerp(originalVolume, 0, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        bgmFadeOut = null;
        if(stopAudio){
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    IEnumerator FadeIn(AudioSource audioSource,float targetVolume,float delay,float fadeTime) 
    {
        yield return new WaitForSeconds(delay);
        float timeElapsed = 0;

        while (audioSource.volume < targetVolume) 
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        bgmFadeIn = null;
    }

    float GetVolume(string name){
        return _audioDict[name].volume * (_audioDict[name].audioType == AudioType.SFX?sfxVolume:bgmVolume);
    }

    float GetVolume(int index){
        return audioClips[index].volume * (audioClips[index].audioType == AudioType.SFX?sfxVolume:bgmVolume);
    }
}
