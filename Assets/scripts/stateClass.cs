using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stateClass : MonoBehaviour
{
    [SerializeField] public float stateColor;
    [SerializeField] public float stateRumble;
    // Start is called before the first frame update
    public void setValues(float highFreqValue, float lowFreqValue){
        stateColor = highFreqValue;
        stateRumble = lowFreqValue;

    }
    
}
