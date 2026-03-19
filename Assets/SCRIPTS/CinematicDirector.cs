using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // For the text
//for debug log console
//using  UnityEngine.Debug;

public class CinematicDirector : MonoBehaviour
{[Header("Player & Movement")]
    public Transform xrRig; 
    public Transform startPoint;
    public Transform satelliteViewPoint;
    public Transform mainSatellite;
    public Transform earthZoomPoint;
    public float movementSpeed = 2f;[Header("UI Elements")]
    public CanvasGroup fadeScreen; 
    public CanvasGroup textPanel1; 
    public CanvasGroup textPanel2; 

    [Header("Voiceover Audio")]
    public AudioSource audioSource; // The AudioSource playing the Voiceovers
    public AudioClip introVoiceover;
    public AudioClip zimbabweVoiceover;

    [Header("Background Audio Ducking")][Tooltip("Drag your background music or space hum AudioSources here.")]
    public AudioSource[] bgSFXSources; 
    public float normalVolume = 1.0f;
    public float duckedVolume = 0.2f; // How quiet it gets during dialogue
    public float audioFadeDuration = 0.5f; // How fast it fades up/down[Header("Scene Management")]
    public string nextSceneName = "Scene2_OnSite"; 

    [Header("The sun dying part lol")][Tooltip("Drag your Directional Light (Sun) here!")]
    public Light sunLight; // Drag your Directional Light (Sun) here!
    void Start()
    {
        // Make sure all BG audio starts at normal volume
        SetBGAudioVolume(normalVolume);
        StartCoroutine(PlayCinematicSequence());
    }

    IEnumerator PlayCinematicSequence()
    {
        // 1. SETUP
        fadeScreen.alpha = 1;
        textPanel1.alpha = 0;
        textPanel2.alpha = 0;
        xrRig.position = startPoint.position;
        xrRig.rotation = startPoint.rotation;

        // 2. FADE IN FROM BLACK
        yield return StartCoroutine(FadeCanvas(fadeScreen, 1, 0, 2f));
        yield return new WaitForSeconds(1f); 

        // 3. SHOW TEXT 1 & PLAY AUDIO 1
        yield return StartCoroutine(FadeCanvas(textPanel1, 0, 1, 1.5f));
        
        // Lower BG volume smoothly, then play Voiceover
        StartCoroutine(FadeBGAudio(duckedVolume)); 
        audioSource.clip = introVoiceover;
        audioSource.Play();

        // 4. WAIT FOR AUDIO TO FINISH
        yield return new WaitForSeconds(introVoiceover.length);
        
        // Bring BG volume back up smoothly
        StartCoroutine(FadeBGAudio(normalVolume)); 
        yield return new WaitForSeconds(0.5f);

        // 5. FADE OUT TEXT 1
        yield return StartCoroutine(FadeCanvas(textPanel1, 1, 0, 1f));

        //look at the satellite as we move towards it
        //yield return StartCoroutine(LookAtTarget(mainSatellite, 2f));

        // 6. MOVE TO MAIN SATELLITE 
        yield return StartCoroutine(MovePlayer(satelliteViewPoint));

        //yield return StartCoroutine(LookAtTarget(mainSatellite, 2f));

        // 7. SHOW TEXT 2 & PLAY AUDIO 2 
        //yield return StartCoroutine(FadeCanvas(textPanel2, 0, 1, 1.5f));
        //yield return StartCoroutine(LookAtTarget(mainSatellite, 2f));
        // Lower BG volume again for the second Voiceover
        StartCoroutine(FadeBGAudio(duckedVolume));
        audioSource.clip = zimbabweVoiceover;
        audioSource.Play();
        
        yield return new WaitForSeconds(zimbabweVoiceover.length);
        
        // Bring BG volume back up smoothly
        StartCoroutine(FadeBGAudio(normalVolume));
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(FadeCanvas(textPanel2, 1, 0, 1f));

        // 8. SLOWLY TURN TO FACE ZIMBABWE/EARTH
        //yield return StartCoroutine(LookAtTarget(earthZoomPoint, 2f));

        // 9. ZOOM INTO EARTH & FADE TO DARKNESS SIMULTANEOUSLY
        // Notice there is NO "yield return" on the fades. This makes them happen 
        // AT THE EXACT SAME TIME the MovePlayer coroutine is running!
        float zoomDuration = 7f; // Adjust this to match how long your zoom takes
        
        StartCoroutine(FadeSceneBrightness(zoomDuration));     // Turn off the sun
        //StartCoroutine(FadeCanvas(fadeScreen, 0, 1, zoomDuration)); // Fade UI to black
        
        // This is the one we yield on, so the script waits for the movement to finish
        yield return StartCoroutine(MovePlayer(earthZoomPoint)); 

        // 10. BRIEF PAUSE IN THE DARKNESS
        yield return new WaitForSeconds(1f);

        // 11. LOAD MILESTONE 2
        SceneManager.LoadScene(nextSceneName);
    }

    // --- Helper Functions Below ---

    // Smoothly fades all background audio sources up or down
    IEnumerator FadeBGAudio(float targetVolume)
    {
        if (bgSFXSources.Length == 0) yield break;

        // Store the starting volume of each source so we can lerp from it
        float[] startVolumes = new float[bgSFXSources.Length];
        for (int i = 0; i < bgSFXSources.Length; i++)
        {
            if (bgSFXSources[i] != null) startVolumes[i] = bgSFXSources[i].volume;
        }

        float elapsed = 0f;
        while (elapsed < audioFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / audioFadeDuration);

            for (int i = 0; i < bgSFXSources.Length; i++)
            {
                if (bgSFXSources[i] != null)
                {
                    bgSFXSources[i].volume = Mathf.Lerp(startVolumes[i], targetVolume, t);
                }
            }
            yield return null;
        }

        // Ensure we hit the exact target volume at the end
        SetBGAudioVolume(targetVolume);
    }

    // Instantly sets volume for all BG sources (used for setup and safety checks)
    void SetBGAudioVolume(float volume)
    {
        foreach (AudioSource source in bgSFXSources)
        {
            if (source != null) source.volume = volume;
        }
    }

    // Fades UI panels smoothly
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

    // Moves the player to a target position without snapping at the end
    IEnumerator MovePlayer(Transform target)
    { Debug.Log("Starting MovePlayer to target: " + target.name);
        Vector3 startPos = xrRig.position;
        Quaternion startRot = xrRig.rotation;
        //target.rotation = startRot; // Start with the current rotation to prevent snapping
        
        float distance = Vector3.Distance(startPos, target.position);
        if (distance <= 0.01f) yield break; // Safety check

        float timeToMove = distance / movementSpeed;
        float elapsed = 0;

        while (elapsed < timeToMove - 0.1f) // Stop slightly early to avoid snapping
        {
            elapsed += Time.deltaTime;
            
            float t = Mathf.Clamp01(elapsed / timeToMove); 
            float smoothStep = t * t * (3f - 2f * t); 
            
            xrRig.position = Vector3.Lerp(startPos, target.position, smoothStep);
            //if(elapsed < timeToMove - 0.3f) // Only rotate during the first 30% of the movement to avoid snapping at the end
            //    xrRig.rotation = Quaternion.Slerp(startRot, target.rotation, smoothStep);
            //xrRig.rotation = Quaternion.Slerp(startRot, target.rotation, smoothStep);
            //target.rotation = Quaternion.Slerp(startRot, target.rotation, smoothStep);
            
            yield return null;
        }

        Debug.Log("Finished MovePlayer to target: " + target.name);
        //xrRig.position = target.position;
        //xrRig.rotation = target.rotation; // Ensure final rotation is correct at the end
        //xrRig.position = target.position;
        //target.rotation = xrRig.rotation; // Keep the final rotation consistent with the player's last orientation
    }

    
    // Smoothly rotates the player to look directly at a live, moving target
    IEnumerator LookAtTarget(Transform target, float duration)
    {
        Debug.Log("Starting LookAt tracking towards: " + target.name);
        Quaternion startRot = xrRig.rotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothStep = t * t * (3f - 2f * t);

            // 1. Find the live direction to the target THIS frame
            Vector3 directionToTarget = target.position - xrRig.position;
            
            // 2. Ensure we don't get a math error if we are perfectly inside the target
            if (directionToTarget != Vector3.zero) 
            {
                // 3. Calculate what rotation looks directly at that target
                Quaternion dynamicTargetRotation = Quaternion.LookRotation(directionToTarget);
                
                // 4. Smoothly blend towards that live rotation
                xrRig.rotation = Quaternion.Slerp(startRot, dynamicTargetRotation, smoothStep);
            }

            yield return null;
        }

        // Final lock-on at the exact end of the timer
        Vector3 finalDirection = target.position - xrRig.position;
        if (finalDirection != Vector3.zero)
        {
            xrRig.rotation = Quaternion.LookRotation(finalDirection);
        }
        Debug.Log("Finished LookAt tracking");
    }

    // Smoothly reduces all scene lighting to absolute darkness
        IEnumerator FadeSceneBrightness(float duration)
        {
            Debug.Log("Fading scene brightness to zero...");
            
            float elapsed = 0f;
            float startIntensity = (sunLight != null) ? sunLight.intensity : 1f;
            Color startAmbient = RenderSettings.ambientLight;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 1. Fade the Sun / Directional Light to 0
                if (sunLight != null)
                {
                    sunLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
                }

                // 2. Fade the background space ambient light to pure black
                RenderSettings.ambientLight = Color.Lerp(startAmbient, Color.black, t);

                yield return null;
            }

            if (sunLight != null) sunLight.intensity = 0f;
            RenderSettings.ambientLight = Color.black;
        }

//------------USELESS FUNCTIONS BELOW (OLD VERSIONS) - KEEP FOR REFERENCE-----------------
    // Smoothly fades UI panels (OLD VERSION - KEPT FOR REFERENCE)
    // Slowly rotates the player to face a new direction
    /*IEnumerator RotatePlayer(Quaternion targetRotation, float duration)
    {
        Debug.Log("Starting rotation to target: " + targetRotation.eulerAngles);
        Quaternion startRot = xrRig.rotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothStep = t * t * (3f - 2f * t);

            xrRig.rotation = Quaternion.Slerp(startRot, targetRotation, smoothStep);
            yield return null;
        }
        xrRig.rotation = targetRotation; 
    }*/
}