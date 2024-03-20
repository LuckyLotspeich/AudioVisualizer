using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TouchManager : MonoBehaviour
{
    // Reference switch scene script
    public SwitchScenes switchScenes; 

    [Header("Test Objects")]
    public GameObject testGameObject;
    public TextMeshProUGUI screenSizeText;
    //public TextMeshProUGUI touchPositionText;    

    [Header("VFXs")]
    public List<GameObject> vfxPrefabs = new List<GameObject>();
    public float vfxMaxDuration = 30f;

    [Header ("UI GameObjects")]
    public GameObject vfxPanel;
    public TextMeshProUGUI vfxSelectionText;
    public Slider vfxSelectionSlider;
    public TextMeshProUGUI durationVFXText;
    public Slider vfxDurationSlider;

    //  X Value of the position of the canvas element
    private RectTransform vfxPanelRect;
    private float vfxPanelPos;
    private bool vfxPanelisVisible = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Test Objects
        // screenSizeText.text = "Screen Width: " + Screen.width.ToString();

        vfxSelectionSlider.maxValue = vfxPrefabs.Count - 1;
        vfxDurationSlider.maxValue = vfxMaxDuration;
        UpdatevfxSelectionText();

        // Declaring VFX Panel Values
        vfxPanelRect = vfxPanel.GetComponent<RectTransform>();
        vfxPanelPos = vfxPanelRect.anchoredPosition.x;
        
        // Initialize Panel settings
        vfxPanel.SetActive(true);
        vfxPanelRect.DOAnchorPosX(-vfxPanelPos, .01f);
        Cursor.visible = vfxPanelisVisible;
      
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn VFX
        Vector3? touchPosition = switchScenes.HandleTouchInput();
        if (touchPosition != null)
        {
            int vfxIndex = (int)vfxSelectionSlider.value;
            GameObject vfxObject = InstantiateVFX((Vector3)touchPosition, vfxIndex);
            Destroy(vfxObject, vfxDurationSlider.value);
        }

        // vfxSelectionText.text = "Slider Value: " + vfxSelectionSlider.value;
        if (vfxDurationSlider.value == 1) {
            durationVFXText.text = "Duration: " + vfxDurationSlider.value + " second";
        }
        else {
            durationVFXText.text = "Duration: " + vfxDurationSlider.value + " seconds";
        }

        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (switchScenes.scenePanelisVisible) {
                vfxPanelRect.DOAnchorPosX(vfxPanelPos, switchScenes.tweenDuration);
                vfxPanelisVisible = true;       
            }
            else {
                vfxPanelRect.DOAnchorPosX(-vfxPanelPos, switchScenes.tweenDuration);
                vfxPanelisVisible = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            vfxSelectionSlider.value -= 1; 
        }
        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            vfxSelectionSlider.value += 1;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            vfxDurationSlider.value += 1; 
        }
        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            vfxDurationSlider.value -= 1;
        }
    }

    GameObject InstantiateVFX(Vector3 position, int vfxIndex) {
        GameObject vfxPrefab = vfxPrefabs[vfxIndex];
        GameObject vfxObject = Instantiate(vfxPrefab, position + vfxPrefab.transform.localPosition, Quaternion.identity);
        VisualEffect visualEffect = vfxObject.GetComponent<VisualEffect>();
        if (visualEffect != null) {
            visualEffect.Play();
        }
        return vfxObject;
    }

    // Make sure to assign this function to the slider you have in the Hierarchy
    void UpdatevfxSelectionText() {
        int vfxIndex = (int)vfxSelectionSlider.value;
        string selectedVFXName = vfxPrefabs[vfxIndex].name; 
        vfxSelectionText.text = "Selected VFX: " + selectedVFXName; 
        // Debug.Log("Update Selection Slider: " + selectedVFXName);
    }

    public void OnVFXSliderValueChanged() {
        // Debug.Log("Slider value changed");
        UpdatevfxSelectionText(); // Update the selected VFX text when the slider value changes
    }

    public void ToggleVFXPanel() {
        if (!vfxPanelisVisible) {
            vfxPanelRect.DOAnchorPosX(vfxPanelPos, switchScenes.tweenDuration);
            switchScenes.openUIButtonRect.DOAnchorPosX(-switchScenes.openUIButtonPos, switchScenes.tweenDuration);

            vfxPanelisVisible = true;
            Cursor.visible = vfxPanelisVisible;

            if (switchScenes.scenePanelisVisible) {
                switchScenes.scenePanelRect.DOAnchorPosX(switchScenes.scenePanelPos, switchScenes.tweenDuration);
            }            
        }
        else {
            vfxPanelRect.DOAnchorPosX(-vfxPanelPos, switchScenes.tweenDuration);
            switchScenes.openUIButtonRect.DOAnchorPosX(switchScenes.openUIButtonPos, switchScenes.tweenDuration);

            vfxPanelisVisible = false;
            Cursor.visible = vfxPanelisVisible;

            if (switchScenes.scenePanelisVisible) {
                switchScenes.scenePanelRect.DOAnchorPosX(-switchScenes.scenePanelPos, switchScenes.tweenDuration);
            }
        }
    }

    public void ToggleScenePanel() {
        if (!switchScenes.scenePanelisVisible) {
            switchScenes.scenePanelRect.DOAnchorPosX(switchScenes.scenePanelPos, switchScenes.tweenDuration);
            switchScenes.scenePanelisVisible = true;       
        }
        else {
            switchScenes.scenePanelRect.DOAnchorPosX(-switchScenes.scenePanelPos, switchScenes.tweenDuration);
            switchScenes.scenePanelisVisible = false;
        }
    }
}