using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;


public class gamepadCursorVector : MonoBehaviour
{
    // Start is called before the first frame update
    private Mouse vMouse;
    [SerializeField] private float cursorSpeed;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Camera mainCamera;
    

    [SerializeField] private Material defaultMat;
    [SerializeField] private Material highlightMat;
 
    private Transform selection;
   
    
    [SerializeField] private string stateFlag = "State";
    private Transform currentTransform;
 

    

    void Start()
    {
        if (vMouse == null)
        {
            vMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!vMouse.added)
        {
            InputSystem.AddDevice(vMouse);
        }


        InputUser.PerformPairingWithDevice(vMouse, playerInput.user);
        PhysicsRaycaster physicsRaycaster = GameObject.FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }
        InputSystem.onAfterUpdate -= Update;
    }

    // Update is called once per frame
    void Update()
    {
        if (vMouse == null || Gamepad.current == null)
        {
            print("something is null");
            return;
            
        }
        
       
        
        Vector2 delta = Gamepad.current.leftStick.ReadValue();
        delta *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = vMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + delta;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);
        
        InputState.Change(vMouse.position, newPosition);
        InputState.Change(vMouse.delta, delta);
        AnchorCursor(newPosition);
     
        
        if (currentTransform != null){
            //setMaterial(currentTransform, defaultMat);
            currentTransform = null;
        }
        
        // Cast a ray from the controller position and direction
        

        if (Gamepad.current.aButton.IsPressed()){
            doTheRayThing(newPosition, 0, 1);

        }
        else if (Gamepad.current.bButton.IsPressed()){
            doTheRayThing(newPosition, 1, 0);

        }
        else if (Gamepad.current.xButton.IsPressed()){
            doTheRayThing(newPosition, 1, 1);

        }
        else{
            Gamepad.current.SetMotorSpeeds(0,0);
        }
        

        
    
    }
    private void doTheRayThing(Vector2 newPosition, float low, float high){
        Ray ray = mainCamera.ScreenPointToRay(newPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            var selection = hit.transform;
            var selRender = selection.GetComponent<Renderer>();
            if (selRender != null){
                low = selection.GetComponent<stateClass>().stateRumble * low;
                high = selection.GetComponent<stateClass>().stateColor * high;

                Gamepad.current.SetMotorSpeeds(low, high);

            }
            
            currentTransform = selection;
            
            
        }

    }
    private void setMaterial(Transform selectedRend, Material mat ){
        var selRender = selectedRend.GetComponent<Renderer>();
        if (selRender != null){
            selRender.material = mat;
        }
    }
     private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }
    private void disable(){
        InputSystem.RemoveDevice(vMouse);
        InputSystem.onAfterUpdate -= Update;
    }
}
