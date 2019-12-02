using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovementController : MonoBehaviour
{
    private Vector2 _vector2_curr_dir;
    public float _speed_scale = 0.01f;
    private Rigidbody2D _rigidbody2D;

    private bool isRotationStarted = false;
    private float remain_angle_deg = 0.0f;
    private float start_angle_deg = 0.0f;

    private bool isMovingStarted = false;
    private float remain_movement_distance = 0.0f;
    private float movement_distance = 4.0f;



    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = this.GetComponent<Rigidbody2D>();

        _vector2_curr_dir = new Vector2(0.0f, 1.0f); // our robot forward vector is north (up)


        action_rotate_left_45_and_move_forward();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //fixed update is called once per phisics calculate tick
    private void FixedUpdate()
    {
        rotate_and_move_tick();

        action_rotate_left_45_and_move_forward();
    }



    void action_rotate_left_45_and_move_forward(){

        if(isMovingStarted || isRotationStarted) {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, 45.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }






    void rotate_and_move_tick() {

        _vector2_curr_dir = rotate_robot_tick(_vector2_curr_dir, _speed_scale);
        if(!isRotationStarted)
        {
            move_forward_tick(movement_distance, _speed_scale);
        }
    }







    //------------------------------------------------------------------------------------------------
    //
    //moving part
    //
    //------------------------------------------------------------------------------------------------


    void move_forward_start(float distance, float movement_speed)
    {
        Vector2 current_direction = _vector2_curr_dir;
        float movement_speed_cur = 0.0f;
        float movement_tick = 0.0f;

        if(movement_speed < 1.0f){
            movement_speed_cur = movement_speed;
        } else {
            movement_speed_cur = 1.0f;
        }

        movement_tick = distance* movement_speed_cur;
        isMovingStarted = true;
        remain_movement_distance = movement_distance;



    }
    void move_forward_tick(float distance, float movement_speed)
    {
        if (!isMovingStarted)
        {
            return;
        }
        Vector2 current_direction = _vector2_curr_dir;
        float movement_speed_cur = 0.0f;
        float movement_tick = 0.0f;
       
        if (movement_speed < 1.0f)
        {
            movement_speed_cur = movement_speed;
        }
        else
        {
            movement_speed_cur = 1.0f;
        }
        movement_tick  = distance * movement_speed_cur;

        
        if(remain_movement_distance > 0.0f) {
            _rigidbody2D.MovePosition(_rigidbody2D.position + current_direction * movement_tick);
            remain_movement_distance = remain_movement_distance - movement_tick;
        } else {
            isMovingStarted = false;
        }
        
    }

    //------------------------------------------------------------------------------------------------
    //
    //rotation part
    //
    //------------------------------------------------------------------------------------------------

    Vector2 rotate_robot_start(Vector2 curr_dir, float angle_deg, float rotation_speed)
    {
        float rotation_speed_cur = 0.0f;
        float rotation_tick = 0.0f;
        Vector2 cur_dir_result;

        if (rotation_speed < 1.0f) {
            rotation_speed_cur = rotation_speed;
        }
        else {
            rotation_speed_cur = 1.0f;
        }
        
        isRotationStarted = true;
        rotation_tick = angle_deg * rotation_speed;
        remain_angle_deg = angle_deg;
        start_angle_deg = angle_deg;

        cur_dir_result = rotate__transform_robot(curr_dir, rotation_tick);
        remain_angle_deg = angle_deg - rotation_tick;

        return cur_dir_result;

    }

    Vector2 rotate_robot_tick(Vector2 curr_dir, float rotation_speed)
    {
        float rotation_speed_cur = 0.0f;
        float rotation_tick = 0.0f;
        Vector2 cur_dir_result = _vector2_curr_dir;
        if(isRotationStarted == false){
            return cur_dir_result;
        }

        if (rotation_speed < 1.0f) {
            rotation_speed_cur = rotation_speed;
        }
        else {
            rotation_speed_cur = 1.0f;
        }

        rotation_tick = start_angle_deg * rotation_speed;
        

        if(remain_angle_deg > 0.0f){
            cur_dir_result = rotate__transform_robot(curr_dir, rotation_tick);
            remain_angle_deg = remain_angle_deg - rotation_tick;
        } else {
            isRotationStarted = false;
        }
        return cur_dir_result;

    }


    Vector2 rotate__transform_robot(Vector2 curr_dir, float angle_deg)
    {
        Vector2 cur_dir_result;
        _rigidbody2D.MoveRotation( _rigidbody2D.rotation + angle_deg);

        cur_dir_result = Vector2Extension.Rotate(curr_dir, angle_deg);

        if (_rigidbody2D.rotation > 360.0f || _rigidbody2D.rotation < -360.0f)
        {
            _rigidbody2D.SetRotation(_rigidbody2D.rotation % 360.0f);
        }

        return cur_dir_result;
    }

}


public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}