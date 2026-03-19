using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // For the text
/*
public class CinematicDirector : MonoBehaviour
{[Header("Player & Movement")]
    public Transform xrRig; // Drag your XR Origin here
    public Transform startPoint;
    public Transform satelliteViewPoint;
    public Transform earthZoomPoint;
    public float movementSpeed = 2f;

    [Header("UI Elements")]
    public CanvasGroup fadeScreen; // A black image covering the screen
    public CanvasGroup textPanel1; // Title + Info 1
    public CanvasGroup textPanel2; // Info 2 (Zimbabwe)

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip introVoiceover;
    public AudioClip zimbabweVoiceover;

    [Header("Scene Management")]
    public string nextSceneName = "Scene2_OnSite"; // Name of your Milestone 2 scene

    void Start()
    {
        // Start the cinematic sequence the moment the game loads
        StartCoroutine(PlayCinematicSequence());
    }

    IEnumerator PlayCinematicSequence()
    {
        // 1. SETUP: Black screen, player at start position, UI invisible
        fadeScreen.alpha = 1;
        textPanel1.alpha = 0;
        textPanel2.alpha = 0;
        xrRig.position = startPoint.position;
        xrRig.rotation = startPoint.rotation;

        // 2. FADE IN FROM BLACK (takes 2 seconds)
        yield return StartCoroutine(FadeCanvas(fadeScreen, 1, 0, 2f));
        yield return new WaitForSeconds(1f); // Brief pause

        // 3. SHOW TEXT 1 & PLAY AUDIO 1
        yield return StartCoroutine(FadeCanvas(textPanel1, 0, 1, 1.5f));
        audioSource.clip = introVoiceover;
        audioSource.Play();

        // 4. WAIT FOR AUDIO TO FINISH
        yield return new WaitForSeconds(introVoiceover.length + 0.5f);

        // 5. FADE OUT TEXT 1
        yield return StartCoroutine(FadeCanvas(textPanel1, 1, 0, 1f));

        // 6. MOVE TO MAIN SATELLITE
        yield return StartCoroutine(MovePlayer(satelliteViewPoint));

        // 7. SHOW TEXT 2 & PLAY AUDIO 2 (Looking at Zimbabwe beam)
        yield return StartCoroutine(FadeCanvas(textPanel2, 0, 1, 1.5f));
        audioSource.clip = zimbabweVoiceover;
        audioSource.Play();

        yield return new WaitForSeconds(zimbabweVoiceover.length + 5.0f);
        yield return StartCoroutine(FadeCanvas(textPanel2, 1, 0, 1f));

        // 8. ZOOM INTO EARTH
        yield return StartCoroutine(MovePlayer(earthZoomPoint));

        // 9. FADE TO BLACK
        yield return StartCoroutine(FadeCanvas(fadeScreen, 0, 1, 2f));
        yield return new WaitForSeconds(1f);

        // 10. LOAD MILESTONE 2 SCENE
        SceneManager.LoadScene(nextSceneName);
    }

    // Helper function to fade UI panels
    IEnumerator FadeCanvas(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = endAlpha;
    }
*/
    
    // Helper function to smoothly move the VR player
    /*
    IEnumerator MovePlayer(Transform target)
    {
        Vector3 startPos = xrRig.position;
        Quaternion startRot = xrRig.rotation;
        float elapsed = 0;
        // Calculate distance to maintain constant speed
        float timeToMove = Vector3.Distance(startPos, target.position) / movementSpeed;

        while (elapsed < timeToMove)
        {
            // Smoothly interpolate position and rotation
            float t = elapsed / timeToMove;
            // Ease-in-out curve for smoother VR movement
            float smoothStep = t * t * (3f - 2f * t); 
            
            xrRig.position = Vector3.Lerp(startPos, target.position, smoothStep);
            xrRig.rotation = Quaternion.Slerp(startRot, target.rotation, smoothStep);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        //xrRig.position = target.position;
        //xrRig.rotation = target.rotation;
    }*/

    // Helper function to smoothly move the VR player WITHOUT snapping
   /* IEnumerator MovePlayer(Transform target)
    {
        Vector3 startPos = xrRig.position;
        Quaternion startRot = xrRig.rotation;
        
        // Calculate distance to maintain constant speed
        float timeToMove = Vector3.Distance(startPos, target.position) / movementSpeed;
        float elapsed = 0;

        // Failsafe in case the target is already at the same position
        if (timeToMove <= 0.01f) yield break;

        while (true) // We will break out of this manually
        {
            elapsed += Time.deltaTime;
            
            // Mathf.Clamp01 ensures 't' NEVER goes above 1.0 (100%)
            float t = Mathf.Clamp01(elapsed / timeToMove); 
            
            // Ease-in-out curve for smoother VR movement
            float smoothStep = t * t * (3f - 2f * t); 
            
            xrRig.position = Vector3.Lerp(startPos, target.position, smoothStep);
            xrRig.rotation = Quaternion.Slerp(startRot, target.rotation, smoothStep);
            
            // Once we hit exactly 100%, break the loop naturally
            if (t >= 1f) 
            {
                break;
            }
            
            yield return null;
        }

        // 🚨 IMPORTANT VR FIX FOR MOVING TARGETS:
        // If your target viewpoint is attached to a satellite that is ORBITING, 
        // the player needs to become a child of the satellite when they arrive, 
        // otherwise the satellite will keep moving and leave the player behind!
        
        // Uncomment the line below IF your target points are moving/orbiting:
        // xrRig.SetParent(target); 
    }
}*/