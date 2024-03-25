using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenesKeybinds : MonoBehaviour
{
    // Scene Values
    private int currentSceneIndex;
    private int maxSceneIndex;
    private int totalScenes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            QuitApplication();
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            if (currentSceneIndex == 0) {
                SceneManager.LoadScene(maxSceneIndex);  
            }
            else {
                SceneManager.LoadScene(currentSceneIndex - 1);
            }
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(currentSceneIndex);
        }

        if (Input.GetKeyUp(KeyCode.D)) {
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

    public void SwitchScene(string Scene) {
        // Debug.Log("SceneSwitch was pressed");
        SceneManager.LoadScene(Scene);
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
