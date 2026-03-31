using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Attach this to your button's OnClick() event
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Switching to scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Attach this to your Quit/Exit button's OnClick() event
    public void QuitApp()
    {
        Debug.Log("Quit button pressed! Exiting the simulation...");
        
        // This closes the actual app on the Quest 2 headset
        Application.Quit();
        
        // This magically stops the game if you are just testing inside the Unity Editor!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}