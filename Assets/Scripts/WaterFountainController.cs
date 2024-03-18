using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaterFountainController : MonoBehaviour
{
    public List<VisualEffect> VisualEffects = new List<VisualEffect>();

    public float maxSpeed = 5;

    void Update()
    {
        foreach (VisualEffect visualEffect in VisualEffects)
        {
            visualEffect.SetFloat("MaxSpeed", maxSpeed);
        }
    }
}