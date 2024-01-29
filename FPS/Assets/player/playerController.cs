using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class playerController : MonoBehaviour
{
    //Camera Variables
    public Camera cam;
    private Vector3 look_input = Vector3.zero;
    private float look_speed = 60;
    private float horizontal_look_angle = 0f;

    public bool invert_x = false;
    public bool invert_y = false;
    private int inverter_factor_x = 1;
    private int inverter_factor_y = 1;
    [Range(0.01f, 1f)] public float sensitivity;
    
    // Start is called before the first frame update
    void Start()
    {
        //hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Invert cam
        if (invert_x) inverter_factor_x = -1;
        if (invert_y) inverter_factor_y = -1;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    private void Look()
    {
        //left or right
        transform.Rotate(Vector3.up, look_input.x * look_speed * Time.deltaTime *inverter_factor_x * sensitivity);
        float angle = look_input.y * look_speed * Time.deltaTime * inverter_factor_y * sensitivity;
        horizontal_look_angle -= angle;
        horizontal_look_angle = Mathf.Clamp(horizontal_look_angle, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(horizontal_look_angle, 0, 0);

    }

    public void GetLookInput(InputAction.CallbackContext context)
    {
        look_input = context.ReadValue<Vector2>();

    }
}
