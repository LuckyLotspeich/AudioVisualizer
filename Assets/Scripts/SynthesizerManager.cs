using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening; 

public class SynthesizerManager : MonoBehaviour
{
    public static SynthesizerManager Instance;
    // public SequencerInstrument SequencerInstrument;
    public Camera cam;

    // [Header("Test")]
    public TextMeshProUGUI screenSizeText;

    [Header("UI")]
    public TextMeshProUGUI bpmText;
    public Slider bpmSlider;

    [Header("Instruments")]
    public List<SequencerInstrument> instruments = new List<SequencerInstrument>();

    [Header("Beat")]
    public float bpm = 60f;
    public int bpmStepSize = 2;
    private float beatInterval;
    private float beatTimer;
    private int currentBeatIndex = 0;
    private List<Renderer> beatLightsRenderers = new List<Renderer>();
    public Material beatLightOn;
    public Material beatLightOff;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            Debug.Log("Instance is this");
        }
        else {
            Destroy(gameObject);
        }    
    }

    // Start is called before the first frame update
    void Start() {
        // Test Values
        screenSizeText.text = "Screen Width: " + Screen.width.ToString();

        // Automatically adds the sequencers in scene to the manager
        SequencerInstrument[] sequencerInstruments = GetComponentsInChildren<SequencerInstrument>();
        instruments.AddRange(sequencerInstruments);

        // Select all the beat lights
        Transform beatTimer = transform.Find("BeatTimer");

        if (beatTimer != null) {
            Renderer[] beatLightsRends = beatTimer.GetComponentsInChildren<Renderer>();

            // Can use this value for seeing how many notes are for each instrument too
            foreach (Renderer renderer in beatLightsRends) {  
                beatLightsRenderers.Add(renderer);
            }
        }
       
        // Get sequences added
        GameObject sequencerManagerObject = GameObject.Find("SequencerManager");

        if (sequencerManagerObject != null) {
            Transform sequencerManagerTransform = sequencerManagerObject.transform;

            foreach (Transform child in sequencerManagerTransform) {

                if (child.name == "Sequencer") {    // can instantiate prefabs with this so the name does match
                    SequencerInstrument sequencerInstrument = child.GetComponent<SequencerInstrument>();

                    if (sequencerInstrument != null) {
                        instruments.Add(sequencerInstrument);
                    }
                }
            }
        }
        CalculateBeatInterval();
    }

    // Update is called once per frame
    void Update() {

        HandleTouchInput();

        if(Input.GetKeyUp(KeyCode.Q)) {
            QuitApplication();
        }

        beatTimer += Time.deltaTime;
        if (beatTimer >= beatInterval)
        {
            // This is where the beat occurs! Ba-dum!
            beatTimer -= beatInterval;
            UpdateBeatLights();
            ActivateNotes();
            currentBeatIndex++;

            if (currentBeatIndex >= beatLightsRenderers.Count) {
                currentBeatIndex = 0;
            }
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

    public void CalculateBeatInterval() {
        beatInterval = 60f / bpm;
    }

    public void UpdateBPM() {
        int newBPM = (int)bpmSlider.value;
        newBPM = (int)(newBPM / bpmStepSize) * bpmStepSize;
        bpm = newBPM;
        CalculateBeatInterval();
        bpmText.text = "BPM: " + bpm;
    }

    public void UpdateBeatLights() {
        beatLightsRenderers[currentBeatIndex].material = beatLightOn;
        Debug.Log(currentBeatIndex);
        
        if (currentBeatIndex == 0) {
            beatLightsRenderers[beatLightsRenderers.Count - 1 ].material = beatLightOff;
        }
        else {
            beatLightsRenderers[currentBeatIndex - 1].material = beatLightOff;
        }
    }

    public void ActivateNotes() {
        foreach (SequencerInstrument instrument in instruments) {
            List<bool> activeNoteStates = instrument.GetActiveNoteStates();

            if (currentBeatIndex >= 0 && currentBeatIndex < activeNoteStates.Count) {   
                if (activeNoteStates[currentBeatIndex]) {
                    instrument.PlaySound();
                    // Debug.Log("Note activated: Instrument " + instrument.name + ", Beat Index " + currentBeatIndex);
                }
            }
            else {
                Debug.LogWarning("Beat index out of range for instrument " + instrument.name);
            }
        }
    }

    public void PlayPauseToggle() {
        
    }

    public void QuitApplication() {
        Application.Quit();
    }




}
