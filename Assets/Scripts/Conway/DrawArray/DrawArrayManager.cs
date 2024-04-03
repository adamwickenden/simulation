using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawArrayManager : MonoBehaviour
{
    [SerializeField] private GameObject conwayManagerObject;
    private ConwayManager conwayManager;

    private List<GameObject> gridElements = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        // Collect game objects so we can generate brush state
        foreach (Transform child in transform){

            if (child.CompareTag("GridElement")){
                gridElements.Add(child.gameObject);
            }
        }
    }

    void Start() {
        conwayManager = conwayManagerObject.GetComponent<ConwayManager>();
    }

    public void BrushAction() {
        bool isActive = transform.parent.gameObject.activeInHierarchy;

        Debug.Log(isActive);

        if (isActive) {
            conwayManager.SetBrush(GetGridBrush());
            transform.parent.gameObject.SetActive(false);
        }
        else {
            transform.parent.gameObject.SetActive(true);
        }
    }

    private int[] PadIntArray(int[] inputArray) {
        int paddedShape = inputArray.Length * 4;
        int[] paddedArray = new int[paddedShape];
        
        for (int i = 0; i < paddedArray.Length; i++) {
            if (i % 4 == 0){
                paddedArray[i] = inputArray[i/4];
            }
        }

        return paddedArray;
    }

    public int[] GetGridBrush()
    {
        List<int> gridState = new List<int>();

        foreach (GameObject item in gridElements) {
            gridState.Add(item.GetComponent<GridElement>().GetState());
        }

        int[] gridStateArray = PadIntArray(gridState.ToArray());

        return gridStateArray;
    }

    public int[] GetDefaultBrush() {
        int[] brushArray = new int[100];
        Array.Fill(brushArray, 1);

        return PadIntArray(brushArray);
    }
}
