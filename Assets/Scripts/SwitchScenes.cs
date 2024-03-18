using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class SwitchScenes : MonoBehaviour
{
    public Camera cam;
    public LayerMask ignoreLayer;

    [Header("UI Elements")]
    public GameObject openUIButton;
    public GameObject scenePanel;

    [Header ("UI Animations")]
    public float tweenDuration = .5f;

    // UI Canvas Elements
    [HideInInspector]
    public float scenePanelPos, openUIButtonPos;
    public RectTransform scenePanelRect;
    public RectTransform openUIButtonRect;
    public bool scenePanelisVisible = false;
    public bool openUIButtonisVisible = true;

    // Scene Values
    private int currentSceneIndex;
    private int maxSceneIndex;
    private int totalScenes;

    // Start is called before the first frame update
    void Start()
    {
        // Camera Initialized
        cam = Camera.main;

        // Scene Index Values
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        totalScenes = SceneManager.sceneCountInBuildSettings;
        maxSceneIndex = totalScenes - 1;

        // Declaring Initial Positions
        scenePanelRect = scenePanel.GetComponent<RectTransform>();
        openUIButtonRect = openUIButton.GetComponent<RectTransform>();

        // Assigning Initial Positions
        scenePanelPos = scenePanelRect.anchoredPosition.x;
        openUIButtonPos = openUIButtonRect.anchoredPosition.x;

        // Activating UI Elements
        scenePanel.SetActive(true);
        openUIButton.SetActive(true);

        // Move the scenePanel offscreen
        scenePanelRect.DOAnchorPosX(-scenePanelPos, .01f);
    }

    // Update is called once per frame
    void Update()
    {
        // Touch Functionalty
        HandleTouchInput();

        // Open Panel here and switch scenes with similar ones too.
        if(Input.GetKeyUp(KeyCode.Escape)) {
            ToggleUI();
        }

        // Open Panel here and switch scenes with similar ones too.
        if(Input.GetKeyUp(KeyCode.A)) {
            if (currentSceneIndex == 0) {
                SceneManager.LoadScene(maxSceneIndex);  
            }
            else {
                SceneManager.LoadScene(currentSceneIndex - 1);
            }
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(currentSceneIndex);
        }

        if(Input.GetKeyUp(KeyCode.D)) {
            if (currentSceneIndex ==  maxSceneIndex) {
                SceneManager.LoadScene(0);
            }
            else {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(currentSceneIndex);
        }
    }

    // Touch Functionality
    public Vector3? HandleTouchInput() {
        
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // This checks to see if the ray hits somehting and if the raycast did not something with the layer UI
            if (Physics.Raycast(ray, out hit) && ((1 << hit.collider.gameObject.layer) & ignoreLayer) == 0)
            {
                // Find and record touch position
                Vector3 touchPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);          
                return touchPoint;
            }
        }
        return null;
    }

    bool IsPointerOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void ToggleUI() {
        if (!scenePanelisVisible) {
            scenePanelRect.DOAnchorPosX(scenePanelPos, tweenDuration);
            openUIButtonRect.DOAnchorPosX(-openUIButtonPos, tweenDuration);
            scenePanelisVisible = true;       
        }
        else {
            scenePanelRect.DOAnchorPosX(-scenePanelPos, tweenDuration);
            openUIButtonRect.DOAnchorPosX(openUIButtonPos, tweenDuration);
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