using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableManager : MonoBehaviour
{

    public static VariableManager Instance { get; private set; }

    public int startShape;

    public int numAgents;

    public float moveSpeed;
    public float turnSpeed;

    public float decay;
    public float diffuse;
    public float trailWeight;

    public float sensorWidth;
    public float sensorDist;
    public int sensorSize = 1;

    private Dropdown dropdown;

    // Awake singleton
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    // Set up dropdown listener
    private void Start()
    {
        dropdown = GameObject.Find("StartShape").GetComponentInChildren<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });

        startShape = dropdown.value;
    }

    // Set number agents
    public void SetNumAgents(float val)
    {
        numAgents = (int) val;
    }

    //On dropdown change, set value
    void DropdownValueChanged(Dropdown change)
    {
        startShape = change.value;
    }

    // Set Move Speed
    public void SetMoveSpeed(float val)
    {
        moveSpeed = val;
    }

    // Turn Speed
    public void SetTurnSpeed(float val)
    {
        turnSpeed = val;
    }

    // Trail decay rate
    public void SetDecay(float val)
    {
        decay = val;
    }

    // Trail diffusion rate
    public void SetDiffuse(float val)
    {
        diffuse = val;
    }

    // Trail weight
    public void SetTrailWeight(float val)
    {
        trailWeight = val;
    }

    // Sensor width (in degrees), so angle between left and right sensor
    public void SetSensorWidth(float val)
    {
        sensorWidth = val;
    }

    // Sensor distance, distance from agent to sensor ring
    public void SetSensorDist(float val)
    {
        sensorDist = val;
    }
}
