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
    public SynthesizerManager synthesizerManager; // This is used for the create instrument button
    // public SequencerInstrument SequencerInstrument;
    public Camera cam;

    [Header("Test")]
    public TextMeshProUGUI screenSizeText;

    [Header("Instruments")]
    public List<SequencerInstrument> instruments = new List<SequencerInstrument>();
    public GameObject instrumentPrefab;

    [Header("Beat variables")]
    public float bpm = 60f;
    public int bpmStepSize = 2;
    private float beatInterval;
    private float beatTimer;
    private int currentBeatIndex = 0;
    public bool pauseBPM = true;

    [Header("Beat Timer")]
    public GameObject beatPrefab;
    public int maxNotes;
    private List<Renderer> beatLightsRenderers = new List<Renderer>();
    public Material beatLightOn;
    public Material beatLightOff;

    [Header("UI Elements")]
    public TextMeshProUGUI bpmText;
    public Slider bpmSlider;
    public Button playPauseButton;
    public TextMeshProUGUI noteNumberText;
    public Button addInstrumentButton;
    public float instrumentOffset;
    public RectTransform scrollContent;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            // Debug.Log("Instance is this");
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
        Renderer[] beatLightsRends = beatTimer.GetComponentsInChildren<Renderer>();

        // Can use this value for seeing how many notes are for each instrument too
        foreach (Renderer renderer in beatLightsRends) {  
            beatLightsRenderers.Add(renderer);
        }
        noteNumberText.text = beatLightsRenderers.Count + " Notes";
       
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

        // Update the instruments to have different sounds
        UpdateBPM();
    }

    // Update is called once per frame
    void Update() {

        HandleTouchInput();

        if(Input.GetKeyUp(KeyCode.Q)) {
            QuitApplication();
        }

        if (!pauseBPM) {
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
        // Debug.Log(currentBeatIndex);
        
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

    public void AddBeat() {
        if (beatLightsRenderers.Count < maxNotes) {
            Renderer lastRenderer = beatLightsRenderers[beatLightsRenderers.Count - 1];
            Transform lastNote = lastRenderer.gameObject.transform;
            Vector3 lastNotePosition = lastNote.position;
            lastNotePosition.x += 1;
            GameObject newNote = Instantiate(beatPrefab, lastNotePosition, lastNote.rotation);

            Renderer newRenderer = newNote.GetComponent<Renderer>();
            beatLightsRenderers.Add(newRenderer);

            noteNumberText.text = beatLightsRenderers.Count + " Notes";

            // Spawn in the notes for each instrument now
            foreach (SequencerInstrument instrument in instruments) {
                instrument.AddNote();
            } 
        }
        else {
            Debug.LogWarning("Maximum number of notes reached.");
        }
    }

    public void RemoveBeat() {
        if (beatLightsRenderers.Count > 1) {
            Renderer lastRenderer = beatLightsRenderers[beatLightsRenderers.Count - 1];
            beatLightsRenderers.Remove(lastRenderer);
            Destroy(lastRenderer.gameObject);

            noteNumberText.text = beatLightsRenderers.Count + " Notes";

            // Remove the notes for each instrument now
            foreach (SequencerInstrument instrument in instruments) {
                instrument.RemoveNote();
            } 
        } 
        else {
            Debug.LogWarning("Cannot remove last note.");
        }
    }

    public void AddInstrument() {
        Transform newInstrumentTransform;

        if (instruments.Count == 1) {
            // Get transform of first instrument
            SequencerInstrument firstInstrument = instruments[0];
            newInstrumentTransform = firstInstrument.transform;
        }
        else {
            // Get transform of last instrument
            SequencerInstrument lastInstrument = instruments[instruments.Count - 1];
            newInstrumentTransform = lastInstrument.transform;
            
        }

        // Debug.Log("Instrument Count: " + instruments.Count);

        Vector3 newInstrumentPosition = newInstrumentTransform.position;
        newInstrumentPosition.y -= instrumentOffset;

        // Create the new instrument and assign values
        GameObject newInstrument = Instantiate(instrumentPrefab, newInstrumentPosition, newInstrumentTransform.rotation, addInstrumentButton.transform.parent);
        newInstrument.name = "Instrument";
        instruments.Add(newInstrument.GetComponent<SequencerInstrument>());
        SequencerInstrument sequencerInstrument = newInstrument.GetComponent<SequencerInstrument>();
        sequencerInstrument.SynthesizerManager = synthesizerManager;

        scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, scrollContent.sizeDelta.y + instrumentOffset);

        Vector3 addButtonPosition = addInstrumentButton.transform.position;
        addButtonPosition.y -= instrumentOffset;
        addInstrumentButton.transform.position = addButtonPosition;
    }

    public void MoveLowerInstruments(int removedInstrumentIndex) {
        for (int i = removedInstrumentIndex; i < instruments.Count; i++) {
            Vector3 movingInstrumentPosition = instruments[i].transform.position;
            movingInstrumentPosition.y += instrumentOffset;
            instruments[i].transform.position = movingInstrumentPosition;
        }
    }

    public void PlayPauseToggle() {
        // Changing from paused to play so making text pause because sound UI displays other state for play/pause
        if (pauseBPM) {
            playPauseButton.GetComponentInChildren<TMP_Text>().text = "Pause";
            pauseBPM = false;
        }
        else {
            playPauseButton.GetComponentInChildren<TMP_Text>().text = "Play";
            pauseBPM = true;
        }
    }

    public void MuteAllInstruments() {
        foreach (SequencerInstrument instrument in instruments) {
            instrument.MuteInstrument();
        } 
    }

    public void ResetAllInstruments() {
        foreach (SequencerInstrument instrument in instruments) {
            instrument.ResetNotes();
        } 
    }

    public void UpdateInitialInstruments() {
        int instrumentStartingValue = 0;
        foreach (SequencerInstrument instrument in instruments) {
            SequencerInstrument sequencerInstrument = instrument.GetComponent<SequencerInstrument>();
            sequencerInstrument.instrumentSlider.value = instrumentStartingValue;
            // sequencerInstrument.instrumentText.text = "test";

            Debug.Log(instrumentStartingValue);
            instrumentStartingValue++;
        }
    }
    
    public void QuitApplication() {
        Application.Quit();
    }
}
