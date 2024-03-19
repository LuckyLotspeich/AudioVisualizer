using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SynthesizerManager : MonoBehaviour
{   
    public Camera cam;

    [Header("Synth Settings")]
    public float bpm = 120f;
    // Used to track the time ingame
    public float beatInterval;
    public float beatTimer; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouchInput();
        // If keys are pressed.
        if(Input.GetKeyUp(KeyCode.Q)) {
            QuitApplication();
        }

        beatTimer += Time.deltaTime;
        if (beatTimer >= beatInterval)
        {
            beatTimer -= beatInterval;
            ToggleInstrument();
        }
    }

    public RaycastHit? HandleTouchInput() {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // This checks to see if the ray hits somehting and if the raycast did not something with the layer UI
            if (Physics.Raycast(ray, out hit)) {
                return hit;       
            }
        }
        return null;
    }
}
