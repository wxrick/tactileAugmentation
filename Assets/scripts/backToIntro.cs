using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backToIntro : MonoBehaviour
{
    [SerializeField]
    public string Scene;
    // Start is called before the first frame update
   
    public void returnToTitle(){
        SceneManager.LoadScene(Scene);
    }
}
