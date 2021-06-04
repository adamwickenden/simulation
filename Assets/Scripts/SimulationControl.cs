using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationControl : MonoBehaviour
{
    public GameObject simulationPrefab;
    private GameObject simulation;

    // Start is called before the first frame update
    public void StartSimulation()
    {
        simulation = Instantiate(simulationPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    public void StopSimulation()
    {
        Destroy(simulation);
    }
}
