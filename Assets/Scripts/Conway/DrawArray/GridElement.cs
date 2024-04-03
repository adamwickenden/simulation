using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridElement : MonoBehaviour
{
    [SerializeField]
    private GameObject gridElement;

    private int State {get; set;}

    private Image buttonImage {get; set;} 

    public void Awake() {
        State = 0;
        buttonImage = gridElement.GetComponent<Image>();
    }

    public void Click() {
        if (State == 0) {
            State = 1;
            buttonImage.color = new Color(1f,1f,1f,1f);
        }
        else {
            State = 0;
            buttonImage.color = new Color(0.2641f,0.2641f,0.2641f,1f);
        }
    }

    public int GetState() {
        return State;
    }

}
