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
    public Material toggleOnMat;
    public Material toggleOffMat;

    [Header("Note States")]
    public List<NoteData> notesStates = new List<NoteData>();

    [Header("Note")]
    public AudioSource audioSource;
    public List<AudioClip> instrumentClips = new List<AudioClip>();
    // public AudioClip selectedInstrumentClip; 
    
    [Header("UI")]
    public TextMeshProUGUI instrumentText;
    public Slider instrumentSlider;
    public string noteTag;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Automatically adds all the audio clips from the AudioClips folder into the list
        string[] audioFilesPaths = AssetDatabase.FindAssets("t:AudioClip", new[] {"Assets/Audio/AudioClips"});

        foreach (var audioFilePath in audioFilesPaths) {
            string path = AssetDatabase.GUIDToAssetPath(audioFilePath);
            AudioClip audioFile = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (audioFile != null) {
                instrumentClips.Add(audioFile);
            }
            
        }

        // Adjusts the slider and updates it
        instrumentSlider.maxValue = instrumentClips.Count - 1;
        UpdateSelectedInstrument();

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
                            renderer.material = toggleOnMat;
                            Debug.Log("Is On");
                        }
                        else {
                            renderer.material = toggleOffMat;
                            Debug.Log("Is Off");
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
        Debug.Log("returning null");
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
        audioSource.PlayOneShot(instrumentClips[(int)instrumentSlider.value]);
    }

    public void ResetNotes() {
        Debug.Log("Reset The Panels");
        foreach (NoteData noteData in notesStates) {
            noteData.noteState = false;
            Renderer renderer = noteData.note.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.material = toggleOffMat; // Reset material to default off state
            }
        }
    }

}

public class NoteData
{
    public GameObject note;
    public bool noteState;

    public NoteData(GameObject note, bool noteState) {
        this.note = note;
        this.noteState = noteState;
    }
}
