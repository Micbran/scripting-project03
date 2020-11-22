using UnityEngine;

public class DisableObjectOnAnimationFinish : MonoBehaviour
{
    public void FinishAnimation()
    {
        gameObject.SetActive(false);
    }
}