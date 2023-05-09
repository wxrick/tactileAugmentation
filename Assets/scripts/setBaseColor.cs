using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class setBaseColor : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> stateList = new List<string>();
    List<float> colorList = new List<float>();
    //Federal Funds perPerson
    List<float> rumbleList = new List<float>();
    [SerializeField] string colorVar;
    [SerializeField] string rumbleVar;
    
    
    void Start()
    {
       
        string fPath = "Assets\\Scripts\\states.csv";
        //print(Directory.GetCurrentDirectory());
        //print(fPath + ":  " + File.Exists(fPath));
        if (File.Exists(fPath)){
            
            

            using (StreamReader reader = new StreamReader(fPath)){
                while(!reader.EndOfStream){
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    stateList.Add(values[0]);
                    colorList.Add(Convert.ToSingle(values[4]));
                    rumbleList.Add(Convert.ToSingle(values[6]));

                }
            }

        }
       
        
        for (int i = 0; i < stateList.Count-1; i++ ){
            var stateObj = GameObject.Find(stateList[i]);
            Color matColor = new Color(0.0f, colorList[i], 0.0f, 1.0f);
            //Debug.Log(stateObj + ": " + i);
            stateObj.GetComponent<Renderer>().material.SetColor("_Color", matColor);
            stateObj.GetComponent<stateClass>().setValues(colorList[i],rumbleList[i]);
            

            

        }

        
    }

    // Update is called once per frame
  
}
