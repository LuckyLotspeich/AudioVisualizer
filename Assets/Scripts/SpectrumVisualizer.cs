using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumVisualizer : MonoBehaviour
{
    public GameObject cube;
    public GameObject[] cubeArray;   
    private float rotationAngle; 
    public float maxScale = 10;

    // Mic Input
    public bool usingMic;

    void Start()
    {
        cubeArray = new GameObject[AudioInputManager.fftSize];
        rotationAngle = 360f / AudioInputManager.fftSize;

        for (int i = 0; i < 1024; i++) {
            // Instantiating Cube and setting values
            GameObject instanceCube = (GameObject)Instantiate(cube);
            instanceCube.transform.position = this.transform.position;
            instanceCube.transform.parent = this.transform;
            instanceCube.name = "SpectrumCube" + i;

            // Spawning Conditions
            this.transform.eulerAngles = new Vector3(0, -rotationAngle * i, 0);
            instanceCube.transform.position = Vector3.forward * 100;

            // Adding to array
            cubeArray[i] = instanceCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 1024; i++) {
            if (cubeArray != null) {
                // Accessing the frequency at each value. They are very small as seen during the test.
                cubeArray[i].transform.localScale = new Vector3(10, (AudioInputManager.samples[i] * maxScale) + 2, 10);
            }
        }
    }
}
