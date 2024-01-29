using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //Debug
    public TMP_Text debug_text;

    //Camera Variables
    public Camera cam;
    private Vector3 look_input = Vector3.zero;
    private float look_speed = 60;
    private float horizontal_look_angle = 0f;

    //Camer Option Variables
    public bool invert_x = false;
    public bool invert_y = false;
    private int invert_factor_x = 1;
    private int invert_factor_y = 1;
    [Range(0.01f, 1f)] public float sensitivity;

    //Movement Variables
    public float max_speed = 10f;
    public float acceleration = 0.2f;
    public float friction_multiplier = 0.9f;
    public float gravity = -0.03f;
    public float jump_power = 10f;
    private Vector2 move_input = Vector2.zero;
    private CharacterController character_controller;
    private Vector3 player_velocity = Vector3.zero;

    private void Start()
    {

        //Hide the mouse.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Inverting Camera
        if (invert_x) invert_factor_x = -1;
        if (invert_y) invert_factor_y = -1;

        //Get Components
        character_controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Look();
        Move();

        //Debug
        debug_text.text = "Player Velocity: " + player_velocity.ToString();
    }

    public void GetLookInput(InputAction.CallbackContext context)
    {
        look_input = context.ReadValue<Vector2>();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        move_input = context.ReadValue<Vector2>();
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) Jump();
    }

    private void Jump()
    {
        if (character_controller.isGrounded) player_velocity.y = jump_power;
    }

    private void Look()
    {
        //Left/Right
        transform.Rotate(Vector3.up, look_input.x * look_speed * Time.deltaTime * invert_factor_x * sensitivity);

        //Up/Down
        float angle = look_input.y * look_speed * Time.deltaTime * invert_factor_y * sensitivity;
        horizontal_look_angle = Mathf.Clamp(horizontal_look_angle, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(horizontal_look_angle, 0, 0);
    }

    private void Move()
    {
        //Accelerate the player.
        player_velocity += (transform.right * move_input.x + transform.forward * move_input.y) * acceleration;
        Vector2 xz_velocity = new Vector2(player_velocity.x, player_velocity.z);
        xz_velocity = Vector2.ClampMagnitude(xz_velocity, max_speed);

        //Friction
        if (move_input == Vector2.zero)
        {
            //Will use air resistance for in_air state.
            xz_velocity *= friction_multiplier;
        }

        //Reconstruct Player Velocity
        player_velocity = new Vector3(xz_velocity.x, player_velocity.y, xz_velocity.y);


        //Gravity
        player_velocity.y += gravity;
        if (character_controller.isGrounded && player_velocity.y < -2f)
        {
            player_velocity.y = -2f;
        }

        //Move player
        character_controller.Move(player_velocity * Time.deltaTime);
    }

    private Vector3 Accelerate(Vector3 wish_dir, Vector3 current_velocity, float accel, float max_speed)
    {
        //project curent velocity
        float proj_speed = Vector3.Dot(current_velocity, wish_dir);
        float accel_speed = accel * Time.deltaTime;

        if (proj_speed + accel_speed > max_speed) accel_speed = max_speed - proj_speed;

        return current_velocity + (wish_dir * accel_speed);
    }

    // private Vector3 MoveGround(Vector3 wish_dir, Vector3 current_velocity )
    // {
    //     Vector3 new_velocity = new Vector3(current_velocity.x, 0 current_velocity.z);

    //     float speed = new_velocity.magnitude;
    //     //if(speed <= stop_speed)
    // }
}
