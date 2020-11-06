﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHandler : MonoBehaviour
{
    [SerializeField] private Transform playerFistHome = null;
    [SerializeField] private GrappleInput inputHandler = null;
    [SerializeField] private GameObject objectArt = null;

    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float grappleFlySpeed = 10f;
    private Camera mainCamera = null;
    private BoxCollider fistCollider = null;

    private bool grappleOut = false;
    private bool isGrappling = false;
    private bool grappleRetracting = false;
    private Transform thisTransform = null;


    private Vector3 fistDirection = Vector3.zero;

    private float FistDistance
    { get { return (Vector3.Distance(thisTransform.position, playerFistHome.position)); } }


    private void Awake()
    {
        mainCamera = Camera.main;
        fistCollider = this.GetComponent<BoxCollider>();
        thisTransform = this.transform;
        fistCollider.enabled = false;
        thisTransform.position = playerFistHome.position;
        objectArt.SetActive(false);
        fistCollider.enabled = false;
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
        if(!grappleOut)
        {
            grappleOut = true;
            grappleRetracting = false;
            objectArt.SetActive(true);
            fistCollider.enabled = true;
            thisTransform.position = playerFistHome.position;
            Ray toBeRaycasted = mainCamera.ScreenPointToRay(Input.mousePosition);
            fistDirection = toBeRaycasted.direction;
            thisTransform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }

    private void OnGrappleReleased()
    {
        grappleOut = false;
        isGrappling = false;
        fistCollider.enabled = false;
        thisTransform.position = playerFistHome.transform.position;
        objectArt.SetActive(false);
    }

    private void FixedUpdate()
    {
        if(grappleOut)
        {
            Vector3 grappleDirection = fistDirection;
            if (grappleDirection.magnitude >= 0.5f && FistDistance < maxDistance && !isGrappling && !grappleRetracting)
                thisTransform.position = thisTransform.position + grappleDirection.normalized * grappleFlySpeed;
            if(Mathf.Abs(FistDistance - maxDistance) <= 0.5f)
            {
                isGrappling = false;
                grappleRetracting = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision.");
        if (other.gameObject.CompareTag("Terrain"))
        {
            isGrappling = true;
            Debug.Log("Collision with terrain.");
        }
    }
}
