using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public int band;
    public float startScale, scaleMultiplier, speed;
    public bool useBuffer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (useBuffer) {
            Vector3 previousScale = transform.localScale;
            float level = scaleMultiplier*AudioInputManager.bandBuffer[band]*Time.deltaTime*1000;
            previousScale.y = Mathf.Lerp(previousScale.y, level, speed*Time.deltaTime);
            transform.localScale = previousScale;
        }
        else {
            transform.localScale = new Vector3(transform.localScale.x, (AudioInputManager.freqBand[band] * scaleMultiplier) + startScale, transform.localScale.z); 
        } 
    }
}
