using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect;
    private bool isDragging = false;
    private Vector2 startDragPosition;

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startDragPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 dragDelta = touch.position - startDragPosition;
                        scrollRect.normalizedPosition += new Vector2(dragDelta.x / Screen.width, dragDelta.y / Screen.height);
                        startDragPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }
    }
}
