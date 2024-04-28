using UnityEngine;

public class ChangeSceneButton : MonoBehaviour {
    public void ChangeScene(int sceneIndex){
        MySceneManager.instance.LoadScene(sceneIndex);
    }
}