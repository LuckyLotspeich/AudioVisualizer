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
    public static bool autoSwitchScenes = false;
    public float sceneSwitchTimer = 0f;
    public float sceneSwitchInterval = 30f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        // Scene Index Values
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        totalScenes = SceneManager.sceneCountInBuildSettings;
        maxSceneIndex = totalScenes - 1;

        scenePanelRect = scenePanel.GetComponent<RectTransform>();
        openUIButtonRect = openUIButton.GetComponent<RectTransform>();
        scenePanelPos = scenePanelRect.anchoredPosition.x;
        openUIButtonPos = openUIButtonRect.anchoredPosition.x;

        // Activating all elements in case they are turned off when editing in play mode
        scenePanel.SetActive(true);
        openUIButton.SetActive(true);

        // Initial Animations to move which elements off screen
        Cursor.visible = scenePanelisVisible;
        scenePanelRect.DOAnchorPosX(-scenePanelPos, .01f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouchInput();

        // Open Panel here and switch scenes with similar ones too.
        if (Input.GetKeyUp(KeyCode.Space)) {
            ToggleUI();
        }

        if (autoSwitchScenes) {
            sceneSwitchTimer += Time.deltaTime;

            if (sceneSwitchTimer >= sceneSwitchInterval) {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else {
                    SceneManager.LoadScene(2);
                }
            }
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
            Cursor.visible = scenePanelisVisible;
        }
        else {
            scenePanelRect.DOAnchorPosX(-scenePanelPos, tweenDuration);
            openUIButtonRect.DOAnchorPosX(openUIButtonPos, tweenDuration);
            scenePanelisVisible = false;
            Cursor.visible = scenePanelisVisible;
        }
    }

    public void SwitchScene(string Scene) {
        // Debug.Log("SceneSwitch was pressed");
        SceneManager.LoadScene(Scene);
    }

    public void AutoSwitchScenes() {
        autoSwitchScenes = !autoSwitchScenes;
    }
}
