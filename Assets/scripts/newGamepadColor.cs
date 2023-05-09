using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;


using System.Linq;


using UnityEngine.UI;


public class newGamepadColor : MonoBehaviour
{


    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    private RectTransform canvasTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RawImage rawImage;
    private int count = 0;
    private float rumbleStrength;
    private float rumbleSpeed;
    Texture2D t2D;
    
    private Camera mainCamera;
    private Mouse vMouse;
    
   
    private float nextPulse;
 
    private float nextPulse2;
    private bool noMotor = true;

    


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        t2D = rawImage.texture as Texture2D;
        if (vMouse == null)
        {
            vMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!vMouse.added)
        {
            InputSystem.AddDevice(vMouse);
        }


        InputUser.PerformPairingWithDevice(vMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(vMouse.position, position);
        }

        
        //print("start began");

    }

    // Update is called once per frame
    void Update()
    {
        if (vMouse == null || Gamepad.current == null)
        {
            print("something is null");
            return;
            
        }

       //print("Working");
        Vector2 delta = Gamepad.current.leftStick.ReadValue();
        delta *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = vMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + delta;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);
        
        InputState.Change(vMouse.position, newPosition);
        InputState.Change(vMouse.delta, delta);

       
        
        Vector2 localCursor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform,
            newPosition, Camera.main, out localCursor);

        //Begin solution from chatGPT transforms from 
        float xNorm = Mathf.InverseLerp(-rawImage.rectTransform.pivot.x * rawImage.rectTransform.rect.width,
            (1 - rawImage.rectTransform.pivot.x) * rawImage.rectTransform.rect.width, localCursor.x);
        float yNorm = Mathf.InverseLerp(-rawImage.rectTransform.pivot.y * rawImage.rectTransform.rect.height,
            (1 - rawImage.rectTransform.pivot.y) * rawImage.rectTransform.rect.height, localCursor.y);
        int x = (int)(xNorm * t2D.width);
        int y = (int)(yNorm * t2D.height);
        //end solution from chatGPT..



       
        if(Gamepad.current.leftShoulder.IsPressed()){
            int low=0;
            int high=0;
            Color color = t2D.GetPixel((int)x, (int)y);
            float hue, sat, val;
            Color.RGBToHSV(color, out hue, out sat, out val); //getHueValue(x, y);
            
            hue = hue * 360;
            print(hue + ": is the hue value" + color);
            if (hue >= 331  || (0 <= hue && hue <= 30)){
                //print("red, the blood of angry men");
                low = 1;
                high = 0;


            }
            else if( hue >= 71 && hue <= 150){
                //print("green with envy");
                low = 0;
                high = 1;
            }
            
            //(sumIntensity, roughness) = getIntensityValue(x, y);
            if  (nextPulse2 > Time.time && count < 6 ){
                //print("First IF");
                if ( nextPulse > Time.time && noMotor){
                    //print("SecondIF");
                    //print("Next Pulse Value " + pulseRate2 );//+ nextPulse);
                    
                    Gamepad.current.SetMotorSpeeds(low, high );
                    //print("if statement count: "+ nextPulse +" : "+ Time.time);
                                     
                }
                else if ((nextPulse > Time.time && !noMotor )){
                    //print("else if1");
                    Gamepad.current.SetMotorSpeeds(0, 0);
                }
                else{
                    //Gamepad.current.SetMotorSpeeds(0, 0);
                    nextPulse = Time.time + .5f;
                    //print(noMotor + "noMotor1");
                    noMotor = !noMotor;
                    //print(noMotor +"noMotor2");
                    count +=1;
                    //print(count  +":count");
                }
                //print(pulseRate + ": Pulserate");

            }
            else if(nextPulse2 > Time.time && count >= 6) { 
                //print("else if 2");
                Gamepad.current.SetMotorSpeeds(0,0);
            }
            else{
                count = 0;
                nextPulse2 = Time.time +5f;
                nextPulse = Time.time + .5f;
                //print("else");
            } 

        }
        
        /*
        else if (Gamepad.current.rightShoulder.IsPressed()){
            StartCoroutine(pulse());
        }
        */
        else
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
            
        }

        placeCursor(newPosition);
     
        
    }

    
private void stopVibration(){
    if (Gamepad.current != null){
        Gamepad.current.SetMotorSpeeds(0f, 0f);

    }
}
 private void placeCursor(Vector2 position)
    {
        Vector2 anchor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchor);
        
        cursorTransform.anchoredPosition = anchor;
    }

    
    private float getHueValue(float xpos, float ypos){
        float hue = 0;
        var color = t2D.GetPixel((int)xpos, (int)ypos);
        print(color + " Color:::::");
        //red >= Blue
        if (color[0] >= color[2]){
            //also Green >= Blue
            if(color[1] >= color[2]){
            print("red, green, blue");
            hue = 60 * ((color[1]-color[2])/(color[0]-color[2]));
            }
            // else  Blue > Green
            else if (color[2] > color[1]){
                print("red,blue, green");
                hue = 60 * (6-((color[2]-color[1])/(color[0]-color[1])));
            }
        }
        //green >=blue
        else if (color[1] >= color[2]){
            //blue > red
            if (color[0] >= color[2]){
                print("green, red, blue");
                hue = 60* (2-((color[0]-color[2])/(color[1]-color[2])));
            }
            else if( color[2] > color[0]){
                print("green, blue red");
                hue = 60* (2+((color[2]-color[0])/(color[1]-color[0])));
            }

        }
        else if (color[2]> color[1]){
            if (color[1] > color[0]){
                print("blue, green, red");
                hue = 60* (4-((color[1]-color[0])/(color[2]-color[0])));
            }
            else if (color[0] >= color[1]){
                print("blue, red, green");
                hue = 60* (4+((color[0]-color[1])/(color[0]-color[2])));
            }
        }
        //something ridiculaous if it goes wrong. 
        else {
            hue= 1000;
        }

        return hue;
    }
    private void pulseMotorSpeed(float intensity1, float intensity2 ){
        //print("voied");
        print(Gamepad.current+ " " + intensity1 +" " + intensity2);
        Gamepad.current.SetMotorSpeeds(intensity1, intensity2);

    }
    /*
    IEnumerator pulse(){
        print(Time.time + ": Time ");
        Gamepad.current.SetMotorSpeeds(.5f, .5f);
        
        print (Time.time + ": time 2");

    }
    */
  

}
