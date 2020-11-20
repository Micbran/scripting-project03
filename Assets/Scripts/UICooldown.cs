using System;
using UnityEngine;
using UnityEngine.UI;

public class UICooldown : MonoBehaviour
{
    [SerializeField] Text cooldownText = null;
    [SerializeField] GrappleHandler grappleObject = null;

    void Update()
    {
        int grappleCurrentCooldown = Math.Max(0, Mathf.CeilToInt(grappleObject.GrappleCurrentCooldown));
        if(grappleCurrentCooldown > 0)
        {
            cooldownText.text = grappleCurrentCooldown.ToString();
        }
        else
        {
            cooldownText.text = "";
        }
    }
}
