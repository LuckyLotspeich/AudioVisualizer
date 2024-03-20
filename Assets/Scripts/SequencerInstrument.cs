using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
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
    public string noteTag; // The tag assigned in the inspector

    [Header("Test")]
    public TextMeshProUGUI outputText;

    // Start is called before the first frame update
    void Start()
    {
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

        if (touchPosition != null)
        {
            RaycastHit hit = touchPosition.Value;

            if (hit.collider.gameObject.CompareTag(noteTag))
            {
                GameObject note = hit.collider.gameObject;

                NoteData noteData = GetNoteData(note);

                if (noteData != null)
                {
                    // Get the state of the hit note
                    bool hitnoteState = noteData.noteState;
                    hitnoteState = !hitnoteState;

                    // Updates the list with the new value
                    noteData.noteState = hitnoteState;

                    Renderer renderer = note.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        if (hitnoteState)
                        {
                            renderer.material = toggleOnMat;
                            Debug.Log("Is On");
                        }
                        else
                        {
                            renderer.material = toggleOffMat;
                            Debug.Log("Is Off");
                        }
                    }
                }
                else {
                    Debug.LogWarning("NoteData not found for the note GameObject.");
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
        return null;
    }

    public void ResetArray() {
        Debug.Log("Reset The Panels");
    }

    public void PrintActiveNotes() {
        string activeNotes = "Active Notes:\n";

        foreach (NoteData noteData in notesStates) {
            if (noteData.noteState) {
                activeNotes += noteData.note.name + "\n";
            }
        }

        outputText.text = activeNotes;
    }
}


public class NoteData
{
    public GameObject note;
    public bool noteState;

    public NoteData(GameObject note, bool noteState)
    {
        this.note = note;
        this.noteState = noteState;
    }
}
