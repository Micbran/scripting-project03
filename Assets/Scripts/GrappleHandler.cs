using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrappleInput))]
public class GrappleHandler : MonoBehaviour
{
    [SerializeField] private GameObject fistObject = null;
    [SerializeField] private Transform playerFistHome = null;

    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float grappleFlySpeed = 10f;
    private Camera mainCamera = null;

    private GrappleInput inputHandler = null;

    private bool grappleRetracted = true;
    private bool isGrappling = false;
    private Coroutine coroutRef;

    private int tester = 1;

    private RaycastHit grapplePoint;

    private void Awake()
    {
        inputHandler = this.GetComponent<GrappleInput>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        inputHandler.GrapplePressed += OnGrapplePressed;
        inputHandler.GrappleReleased += OnGrappleReleased;
    }

    private void OnDisable()
    {
        inputHandler.GrapplePressed -= OnGrapplePressed;
        inputHandler.GrappleReleased -= OnGrappleReleased;
    }

    private void OnGrapplePressed()
    {
        grappleRetracted = false;
        Debug.Log("Grapple fired!");
        fistObject.SetActive(true);
        Ray toBeRaycasted = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(toBeRaycasted, out grapplePoint, maxDistance))
        {
            fistObject.transform.LookAt(grapplePoint.point);
            coroutRef = StartCoroutine(Grappling());
        }

    }

    private void OnGrappleReleased()
    {
        grappleRetracted = true;
        Debug.Log("Grapple retracting...");
        fistObject.transform.position = playerFistHome.transform.position;
        StopCoroutine(coroutRef);
    }

    private IEnumerator Grappling()
    {
        while(true)
        {
            Vector3 grappleDirection = grapplePoint.point - fistObject.transform.position;
            if(grappleDirection.magnitude >= 0.5f)
                fistObject.transform.position = fistObject.transform.position + grappleDirection.normalized * grappleFlySpeed;
            Debug.Log("Test: " + tester++);
            yield return null;
        }
    }

}
