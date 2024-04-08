using System.Collections;
using System.Collections.Generic;
// using System.IO.Directory;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening; 

public class SequencerInstrument : MonoBehaviour
{
    // Reference switch scene script
    public SynthesizerManager SynthesizerManager; 

    [Header("Materials")]
    public Material noteOnMat;
    public Material noteOffMat;

    [Header("Note States")]
    public List<NoteData> notesStates = new List<NoteData>();

    [Header("Audio Variables")]
    public AudioSource audioSource;
    public List<AudioClip> instrumentClips = new List<AudioClip>();
    // public AudioClip selectedInstrumentClip; 

    [Header("Notes")]
    public GameObject notePrefab;
    // private List<GameObject> notes = new List<GameObject>();
    private bool muteInstrument = false;
    
    [Header("UI")]
    public TextMeshProUGUI instrumentText;
    public Slider instrumentSlider;
    public string noteTag;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();

        // Automatically adds all the audio clips from the AudioClips folder located in Assets/Resourcesinto the list
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("AudioClips");

        foreach (var audioClip in audioClips) {
            instrumentClips.Add(audioClip);
        }

        // Adjusts the slider and updates it       // Adding this to manager
        instrumentSlider.maxValue = instrumentClips.Count - 1;
        UpdateSelectedInstrument();
        SynthesizerManager.UpdateInitialInstruments();

        foreach (Transform child in transform) {
            if (child.CompareTag(noteTag)) {
                GameObject note = child.gameObject;
                bool noteState = false;
                NoteData noteData = new NoteData(note, noteState);
                notesStates.Add(noteData);
            }
        }
        
    }

    void Update() {
        // Touch Functionality
        RaycastHit? touchPosition = SynthesizerManager.HandleTouchInput();

        if (touchPosition != null) {
            RaycastHit hit = touchPosition.Value;

            if (hit.collider.gameObject.CompareTag(noteTag)) {
                GameObject note = hit.collider.gameObject;

                NoteData noteData = GetNoteData(note);

                if (noteData != null) {
                    noteData.noteState = !noteData.noteState;

                    Renderer renderer = note.GetComponent<Renderer>();
                    if (renderer != null) {
                        if (noteData.noteState) {
                            renderer.material = noteOnMat;
                            // Debug.Log("Is On");
                        }
                        else {
                            renderer.material = noteOffMat;
                            // Debug.Log("Is Off");
                        }
                    }
                }
            }
        }
    }

    public NoteData GetNoteData(GameObject note) {
        foreach (NoteData noteData in notesStates) {
            if (noteData.note == note) {
                return noteData;
            }
        }
        // Debug.Log("returning null");
        return null;        
    }

    public List<bool> GetActiveNoteStates() {
        List<bool> activeNoteStates = new List<bool>();

        foreach (NoteData noteData in notesStates) {
            activeNoteStates.Add(noteData.noteState);
        }
        return activeNoteStates;
    }

    public void OnInstrumentSliderChanged() {
        UpdateSelectedInstrument();
    }

    public void UpdateSelectedInstrument() {
        int selectedInstrumentIndex = (int)instrumentSlider.value;

        if (selectedInstrumentIndex >=0 && selectedInstrumentIndex < instrumentClips.Count) {
            // selectedInstrumentClip = instrumentClips[selectedInstrumentIndex];
            string selectedInstrumentName = instrumentClips[selectedInstrumentIndex].name;
            if (instrumentClips[selectedInstrumentIndex] != null) {
                audioSource.clip = instrumentClips[selectedInstrumentIndex];
                instrumentText.text = selectedInstrumentName; 
            }
            else {
                Debug.LogWarning("No audio clip assigned to the selected instrument.");
            }
        }
    }

    public void PlaySound() {
        if (!muteInstrument) {
            audioSource.PlayOneShot(instrumentClips[(int)instrumentSlider.value]);
        }
    }

    public void AddNote() {
        // Getting the transform of the last note
        int lastIndex = notesStates.Count - 1;
        NoteData lastNote = notesStates[lastIndex];
        Transform lastNoteTransform = lastNote.note.transform;
        Vector3 lastNotePosition = lastNoteTransform.position;
        lastNotePosition.x += 1;

        GameObject newNote = Instantiate(notePrefab, lastNotePosition, lastNoteTransform.rotation);

        // Creating a new NoteData object for the new note's data
        bool noteState = false; 
        NoteData noteData = new NoteData(newNote, noteState);
        notesStates.Add(noteData);
    }

    public void RemoveNote() {
        int lastIndex = notesStates.Count - 1;
        NoteData lastNote = notesStates[lastIndex];
        Destroy(lastNote.note);
        notesStates.RemoveAt(lastIndex);
    }

    // Is there a thing already in unity that is just mute already?
    public void MuteInstrument() {
        muteInstrument = !muteInstrument;
    }

    public void ResetNotes() {
        Debug.Log("Reset The Panels");
        foreach (NoteData noteData in notesStates) {
            noteData.noteState = false;
            Renderer renderer = noteData.note.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.material = noteOffMat; // Reset material to default off state
            }
        }
    }

    public void RemoveInstrument() {
        int index = SynthesizerManager.instruments.IndexOf(this);
        SynthesizerManager.instruments.Remove(this);
        Destroy(gameObject);
        Vector3 addButtonPosition = SynthesizerManager.addInstrumentButton.transform.position;
        addButtonPosition.y += SynthesizerManager.instrumentOffset;
        SynthesizerManager.addInstrumentButton.transform.position = addButtonPosition;

        SynthesizerManager.MoveLowerInstruments(index);
        // Debug.Log(index);
    }

}

public class NoteData {
    public GameObject note;
    public bool noteState;

    public NoteData(GameObject note, bool noteState) {
        this.note = note;
        this.noteState = noteState;
    }
}
