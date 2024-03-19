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
    // Reference switch scene script
    public SwitchScenes switchScenes; 

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
        
    }
}
