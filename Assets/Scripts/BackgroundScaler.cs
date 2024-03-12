using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        ScaleBackgroundToScreen();
    }

    // This is WAYYY too big. But it works.. just isn't correct
    void ScaleBackgroundToScreen()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;
        // Debug.Log("Width: " + screenWidth + "       Height: " + screenHeight + "       ScreenAspect: " + screenAspect);

        // Get the size of the background plane in world units
        Vector3 backgroundSize = GetComponent<Renderer>().bounds.size;
        // Debug.Log("BackgroundSize: " + backgroundSize);

        // Calculate the desired width of the background plane based on the screen width
        float desiredWidth = backgroundSize.x * (screenWidth / backgroundSize.x);
        // Debug.Log("DesiredWidth: " + desiredWidth);
        
        // Calculate the scale factor
        float scaleFactor = desiredWidth / backgroundSize.x;
        // Debug.Log("ScaleFactor: " + scaleFactor);

        // Scale the background plane
        transform.localScale = new Vector3(scaleFactor, 1f, 1f);
    }
}


