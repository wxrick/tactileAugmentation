using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;



using UnityEngine.UI;


public class newGamepad : MonoBehaviour
{


    [SerializeField]
    public PlayerInput playerInput;
    [SerializeField]
    public RectTransform cursorTransform;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    public RectTransform canvasTransform;
    [SerializeField]
    public Canvas canvas;
    [SerializeField]
    private RawImage rawImage;
    private int count = 0;

    Texture2D t2D;

    private Camera mainCamera;
    private Mouse vMouse;
    private float motorSpeed;
    private int height = 3;
    private int width = 3;
  
    private float pulseRate = 3f;
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
            vMouse = (Mouse)InputSystem.AddDevice("virtualMouse");
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

        //print(Input.mousePosition);
        //print(Screen.width + ": width and height: " + Screen.height);
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
        //Read in gamepad position and update its postion basedon the "speed" of the stick movement
        Vector2 delta = Gamepad.current.leftStick.ReadValue();
        delta *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = vMouse.position.ReadValue();
        Vector2 newPosition = (currentPosition + delta) ;
        
        InputState.Change(vMouse.position, newPosition);
        InputState.Change(vMouse.delta, delta);

       //clamp to the height and width of the screen
        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

        
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
        //End solution from chatGPT
        
    
        float sumIntensity = 0;
        float roughness = 0;



        bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
        bool bButtonIsPressed = Gamepad.current.bButton.IsPressed();
        bool xButtonIsPressed = Gamepad.current.xButton.IsPressed();
        if ( aButtonIsPressed)
        {
            
            (sumIntensity, roughness) = getIntensityValue(x, y);

            Gamepad.current.SetMotorSpeeds(0, sumIntensity);
            //print(Gamepad.current);
         
        }
        else if(bButtonIsPressed){

            /*
            https://search.r-project.org/CRAN/refmans/spatialEco/html/tri.html
            0-80 - level terrain surface.
            81-116 - nearly level surface.
            117-161 - slightly rugged surface.
            162-239 - intermediately rugged surface.
            240-497 - moderately rugged surface.
            498-958 - highly rugged surface.
            gt 959 - extremely rugged surface.
            units not specified so not really useful right now
            */
            
            
            (sumIntensity, roughness) = getIntensityValue(x, y);
            //nextPulse = Time.time + 1+roughness;
            print("b were pressed");
            pulseMotorSpeed(roughness, sumIntensity);
        }
        else if (xButtonIsPressed ){
            print("x were pressed");
            (sumIntensity, roughness) = getIntensityValue(x, y);
            pulseMotorSpeed(roughness, 0);
        }

        else if (Gamepad.current.yButton.IsPressed()){
            (sumIntensity, roughness) = getIntensityValue(x, y);
            pulseRate = 2 - (2 * roughness);

            if  ( nextPulse > Time.time ){
                
                print("Next Pulse Value " );//+ nextPulse);
                //sumIntensity = sumIntensity
                sumIntensity = sumIntensity - (nextPulse - Time.time)*sumIntensity;
                Gamepad.current.SetMotorSpeeds(0, sumIntensity);
                print(sumIntensity + " , " + roughness );
            }
           else{
                if (roughness == 0){
                    pulseRate = 2;
                }
                Gamepad.current.SetMotorSpeeds(0,0);
                nextPulse = Time.time + pulseRate;
                print("Else" + nextPulse + ", " + Time.time);
           }

        }
    
        else if (Gamepad.current.rightShoulder.IsPressed()){
            (sumIntensity, roughness) = getIntensityValue(x, y);
            if  (nextPulse2 > Time.time && count < 6 ){
                //print("First IF");
                if ( nextPulse > Time.time && noMotor){
                    //print("SecondIF");
                    
                    Gamepad.current.SetMotorSpeeds(0, sumIntensity);
                    //print("if statement count: "+ nextPulse +" : "+ Time.time);
                                     
                }
                else if ((nextPulse > Time.time && !noMotor )){
                    print("else if1");
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
        
        else if (Gamepad.current.rightTrigger.IsPressed()){

            Texture2D texture = (Texture2D) rawImage.texture;
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform,
                newPosition, Camera.main, out localCursor);
            float xNorm = Mathf.InverseLerp(-rawImage.rectTransform.pivot.x * rawImage.rectTransform.rect.width,
                (1 - rawImage.rectTransform.pivot.x) * rawImage.rectTransform.rect.width, localCursor.x);
            float yNorm = Mathf.InverseLerp(-rawImage.rectTransform.pivot.y * rawImage.rectTransform.rect.height,
                (1 - rawImage.rectTransform.pivot.y) * rawImage.rectTransform.rect.height, localCursor.y);
            int x = Mathf.RoundToInt(xNorm * texture.width);
            int y = Mathf.RoundToInt(yNorm * texture.height);

            // Read the color of the pixel at the mouse position
            Color pixelColor = texture.GetPixel(x, y);

            // Log the pixel color
            Debug.Log("Pixel color: " + pixelColor);
    
        }
        */
      
        else
        {
            Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
          
        }


        placeCursor(newPosition);
     
        
    }

    
    private void placeCursor(Vector2 position)
    {
        Vector2 anchor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchor);
       
        cursorTransform.anchoredPosition = anchor;
    }

    private (float, float) getIntensityValue(float xpos, float ypos){
        // get rid of color and just use mid I'm too tired to do it tonight
            var color = t2D.GetPixel((int)xpos, (int)ypos);
            var x = t2D.GetPixels((int)(xpos-1), (int)(ypos-1), height, width, 0);
            //print(color + ": COLOR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            
            float sumIntensity = 0;
            float sum2 = 0;
            float value;
            float Variance;
            float n = height * width;
            float mid = x[4][0];

            // refer to the below  for explanation of TRI calculation
            //https://www.usna.edu/Users/oceano/pguth/md_help/html/topo_rugged_index.htm
            for (int i = 0; i < n; i++)
            {
                if (i != 4){
                    //sum2 += (value * value);
                    value = x[i][0];
                    sumIntensity += value;
                    sum2 += Mathf.Pow((mid - value), 2);
                }
                else {
                    value = x[i][0];
                    sumIntensity += value;
                }
            }
            Variance = Mathf.Sqrt(sum2);//(sum2 - (sumIntensity * sumIntensity)/N)/N;
            sumIntensity /= (n);
            
            //print( x[0] + " "+ sumIntensity + " " + Variance + " " + N);//"color2 " + color + " " + newPosition.x + ", " + newPosition.y + " " + x[0] + " "+ sumIntensity + " " + Variance);

            //maxRGB = (color[0] + color[1] + color[2]);
            //motorSpeed = Mathf.Max(color[0], color[1], color[2]);
            //motorSpeed = sumIntensity;
            return (sumIntensity, Variance);

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
