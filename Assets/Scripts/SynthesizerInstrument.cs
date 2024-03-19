using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesizerInstrument : MonoBehaviour
{
    // Reference switch scene script
    public SynthesizerManager SynthesizerManager; 

    [Header("Materials")]
    public bool isOn;
    public Material toggleOnMat;
    public Material toggleOffMat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Touch Functionality
        raycast? touchPosition = SynthesizerManager.HandleTouchInput();
        if (touchPosition != null) {
            if (touchPosition.collider.gameObject.CompareTag("Cube")){
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null) {
                    // If the state of the mat is on, the turn off.
                    if (isOn) {
                        renderer.material = toggleOnMat;
                    }
                    else {
                        renderer.material = toggleOffMat;
                    }              
                }
            }
        }
    }
}
