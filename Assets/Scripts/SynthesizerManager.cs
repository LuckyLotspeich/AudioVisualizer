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
    public SequencerInstrument SequencerInstrument;
    public Camera cam;

    [Header("Test")]
    public TextMeshProUGUI screenSizeText;

    [Header("UI")]
    public TextMeshProUGUI bpmText;
    public Slider bpmSlider;

    [Header("Instruments")]
    public List<SequencerInstrument> instruments = new List<SequencerInstrument>();
    public float bpm = 60f;
    // Used to track the time ingame
    private float beatInterval;
    private float beatTimer;
    private int currentBeatIndex = 0;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            Debug.Log("Instance is this");
        }
        else {
            Destroy(gameObject);
        }    
    }

    // Start is called before the first frame update
    void Start()
    {
        // Test Values
        screenSizeText.text = "Screen Width: " + Screen.width.ToString();

        // Automatically adds the sequencers in scene to the manager
        SequencerInstrument[] sequencerInstruments = GetComponentsInChildren<SequencerInstrument>();
        instruments.AddRange(sequencerInstruments);

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
            ActivateNotes();
            currentBeatIndex++;
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
        bpm = newBPM;
        CalculateBeatInterval();
        bpmText.text = "BPM: " + bpm;
    }

    public void ActivateNotes() {
        foreach (SequencerInstrument instrument in instruments) {
            List<bool> activeNoteStates = instrument.GetActiveNoteStates();

            if (currentBeatIndex >= activeNoteStates.Count) {
                currentBeatIndex = 0;
            }
            
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

    public void QuitApplication() {
        Application.Quit();
    }


}
