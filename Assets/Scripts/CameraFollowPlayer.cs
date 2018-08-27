﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    [SerializeField]
    public Transform player;

    private Vector3 _cameraOffset;

    [Range(0.01f, 1.0f)]
    public float smootFactor = 0.5f;

    // Use this for initialization
    void Start () {
        _cameraOffset = transform.position = player.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = player.position + _cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, smootFactor);
	}
}
