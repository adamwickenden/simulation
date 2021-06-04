﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorWidthValue : MonoBehaviour
{
    private Text writeOut;

    // Start is called before the first frame update
    void Start()
    {
        writeOut = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        writeOut.text = VariableManager.Instance.sensorWidth.ToString();
    }
}
