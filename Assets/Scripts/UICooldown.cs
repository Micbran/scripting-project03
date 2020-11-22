using System;
using UnityEngine;
using UnityEngine.UI;

public class UICooldown : MonoBehaviour
{
    [SerializeField] Text cooldownText = null;
    [SerializeField] GrappleHandler grappleObject = null;
    [SerializeField] GameObject CooldownFeedbackFlash = null;

    private bool playedAnim = true;
    
    void Update()
    {
        int grappleCurrentCooldown = Math.Max(0, Mathf.CeilToInt(grappleObject.GrappleCurrentCooldown));
        if(grappleCurrentCooldown > 0)
        {
            playedAnim = false;
            cooldownText.text = grappleCurrentCooldown.ToString();
        }
        else
        {
            cooldownText.text = "";
            if(!playedAnim)
            {
                AudioManager.Instance.PlaySoundEffect(SoundEffect.CooldownFinish);
                CooldownFeedbackFlash.SetActive(true);
                playedAnim = true;
            }
        }
    }
}
