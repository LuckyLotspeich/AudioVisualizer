using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TouchInputManager : MonoBehaviour
{
    public Camera cam;
    public LayerMask ignoreLayer;

    [Header("Test Objects")]
    public GameObject testGameObject;
    public TextMeshProUGUI screenSizeText;
    public TextMeshProUGUI touchPositionText;    

    [Header("VFXs")]
    public List<GameObject> vfxPrefabs = new List<GameObject>();
    public float vfxMaxDuration = 30f;

    [Header ("UI GameObjects")]
    public GameObject mainUIPanel;
    public GameObject openUIButton;
    public GameObject scenePanel;
    public TextMeshProUGUI vfxSelectionText;
    public Slider vfxSelectionSlider;
    public TextMeshProUGUI durationVFXText;
    public Slider vfxDurationSlider;

    [Header ("UI Animations")]
    [SerializeField] float animationDistance;
    [SerializeField] float tweenDuration;

    //  X Value of the position of the canvas element
    private RectTransform mainPanelRect;
    private RectTransform scenePanelRect;
    private RectTransform openUIButtonRect;
    [SerializeField] float mainPanelPos, scenePanelPos, openUIButtonPos;

    // private bool scenePanelWasOpen = false;      Used with no animationss
    private bool mainUIPanelisVisible = false;
    private bool scenePanelisVisible = false;
    // private bool openUIButtonisVisible = true;
    
    // Start is called before the first frame update
    void Start()
    {
        screenSizeText.text = "Screen Width: " + Screen.width.ToString();
        vfxSelectionSlider.maxValue = vfxPrefabs.Count - 1;
        vfxDurationSlider.maxValue = vfxMaxDuration;
        UpdatevfxSelectionText();

        // if (cam = null) {
        //     cam = Camera.main;
        // }

        // Declaring Panel values
        mainPanelRect = mainUIPanel.GetComponent<RectTransform>();
        scenePanelRect = scenePanel.GetComponent<RectTransform>();
        openUIButtonRect = openUIButton.GetComponent<RectTransform>();

        // Panel Placements
        mainUIPanel.SetActive(true);
        openUIButton.SetActive(true);
        scenePanel.SetActive(true);
        mainPanelRect.DOAnchorPosX(mainPanelPos - animationDistance, .01f);
        scenePanelRect.DOAnchorPosX(scenePanelPos - animationDistance, .01f);
    }

    // Update is called once per frame
    void Update()
    {
        // vfxSelectionText.text = "Slider Value: " + vfxSelectionSlider.value;
        if (vfxDurationSlider.value == 1) {
            durationVFXText.text = "Duration: " + vfxDurationSlider.value + " second";
        }
        else {
            durationVFXText.text = "Duration: " + vfxDurationSlider.value + " seconds";
        }

        // Quit Function
        if (Input.GetKeyUp(KeyCode.Q))
        {
            QuitApplication();
        }
        
        // Touch Functionality
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // This checks to see if the ray hits somehting and if the raycast did not something with the layer UI
            if (Physics.Raycast(ray, out hit) && ((1 << hit.collider.gameObject.layer) & ignoreLayer) == 0)
            {
                // Find and record touch position
                Vector3 touchPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                touchPositionText.text = touchPoint.ToString();

                // Spawn Sydney's VFX effect as a prefab
                int vfxIndex = (int)vfxSelectionSlider.value;
                GameObject vfxObject = InstantiateVFX(touchPoint, vfxIndex);
                Destroy(vfxObject, vfxDurationSlider.value);           
            }
        }
    }

    bool IsPointerOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    GameObject InstantiateVFX(Vector3 position, int vfxIndex) {
        GameObject vfxPrefab = vfxPrefabs[vfxIndex];
        GameObject vfxObject = Instantiate(vfxPrefab, position + vfxPrefab.transform.localPosition, Quaternion.identity);
        VisualEffect visualEffect = vfxObject.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
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

    public void ToggleMainPanel() {
        // Without Animations Toggle
        // mainUIPanel.SetActive(!mainUIPanel.activeSelf);
        // openUIButton.SetActive(!openUIButton.activeSelf);

        // // Saves the state of if the panel was open too
        // if (!mainUIPanel.activeSelf && scenePanel.activeSelf) {
        //     scenePanel.SetActive(false);
        // }
        // else if (mainUIPanel.activeSelf && scenePanelWasOpen) {
        //     scenePanel.SetActive(true);
        // }

        // Since we activated from this function running, this is when we do bring it in to the screen
        // if (mainUIPanel.activeSelf) {
        //     mainPanelRect.DOAnchorPosX(mainPanelPos, tweenDuration);
        // }
        // else if (!mainUIPanel.activeSelf) {
        //     mainPanelRect.DOAnchorPosX(mainPanelPos - animationDistance, tweenDuration);
        // }

        if (!mainUIPanelisVisible) {
            mainPanelRect.DOAnchorPosX(mainPanelPos, tweenDuration);
            openUIButtonRect.DOAnchorPosX(openUIButtonPos - animationDistance, tweenDuration);

            mainUIPanelisVisible = true;

            if (scenePanelisVisible) {
                scenePanelRect.DOAnchorPosX(scenePanelPos, tweenDuration);
            }            
        }
        else {
            mainPanelRect.DOAnchorPosX(mainPanelPos - animationDistance, tweenDuration);
            openUIButtonRect.DOAnchorPosX(openUIButtonPos, tweenDuration);

            mainUIPanelisVisible = false;

            if (scenePanelisVisible) {
                scenePanelRect.DOAnchorPosX(scenePanelPos - animationDistance, tweenDuration);
            }
        }

    }

    public void ToggleScenePanel() {
        // Without Animations Toggle
        // scenePanel.SetActive(!scenePanel.activeSelf);

        // scenePanelWasOpen = scenePanel.activeSelf;

        // if (scenePanel.activeSelf) {
        //     scenePanelRect.DOAnchorPosX(scenePanelPos, tweenDuration);
        // }
        // else if (!scenePanel.activeSelf) {
        //     scenePanelRect.DOAnchorPosX(scenePanelPos - animationDistance, tweenDuration);
        // }

        // asdf

        if (!scenePanelisVisible) {
            scenePanelRect.DOAnchorPosX(scenePanelPos, tweenDuration);
            scenePanelisVisible = true;       
        }
        else {
            scenePanelRect.DOAnchorPosX(scenePanelPos - animationDistance, tweenDuration);
            scenePanelisVisible = false;
        }
    }

    public void SwitchScene(string Scene) {
        Debug.Log("SceneSwitch was pressed");
        SceneManager.LoadScene(Scene);
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
