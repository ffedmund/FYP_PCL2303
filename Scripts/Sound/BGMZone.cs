using UnityEngine;

public class BGMZone : MonoBehaviour {
    public string bgmPlaylistName;
    public int playerListLength = 2;

    private void OnTriggerEnter(Collider other) {
        if(other.transform.tag == "Player"){
            // AudioSourceController.instance.FadeOutBGM(5);
            AudioSourceController.instance.SetBGM(bgmPlaylistName+Random.Range(1,1+playerListLength),3);
            AudioSourceController.instance.currentBGMPlaylist = bgmPlaylistName;
        }    
    }

    private void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Player"){
            // AudioSourceController.instance.FadeOutBGM(5);
            AudioSourceController.instance.SetBGM("Default1",3);
            AudioSourceController.instance.currentBGMPlaylist = "Default";
        }    
    }
}