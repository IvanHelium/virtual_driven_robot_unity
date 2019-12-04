using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovementController : MonoBehaviour
{
    private Vector2 _vector2_curr_dir;
    private Vector2 _vector2_dir_north = new Vector2(0.0f, 1.0f); //but it have to be const
    public float _speed_scale = 0.01f;
    private Rigidbody2D _rigidbody2D;

    private bool isRotationStarted = false;
    private float remain_angle_deg = 0.0f;
    private float start_angle_deg = 0.0f;

    private bool isMovingStarted = false;
    private float remain_movement_distance = 0.0f;
    private float movement_distance = 4.0f;

    private Vector2 getVector; //vector between our movement direction (_vector2_curr_dir) and vector of robot rigidbody (it is not same)


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = this.GetComponent<Rigidbody2D>();

        _vector2_curr_dir = _vector2_dir_north; // our robot forward vector is north (up)


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //fixed update is called once per phisics calculate tick
    private void FixedUpdate()
    {
        rotate_and_move_tick();

        //action_move_forward();
        action_rotate_left_90_and_move_forward();


    }

    //----------------------------------------------------------------------
    //----------------------------------------------------------------------
    //
    // different actions
    //
    //----------------------------------------------------------------------
    //----------------------------------------------------------------------

    void action_rotate_left_45_and_move_forward(){
        if(isMovingStarted || isRotationStarted) {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, 45.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    void action_rotate_left_90_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, 90.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }
    void action_rotate_left_135_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, 135.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }
    //----------------------------------------------------------------------
    void action_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, 0.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    //----------------------------------------------------------------------
    void action_rotate_right_45_and_move_forward() {
        if (isMovingStarted || isRotationStarted) {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, -45.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    void action_rotate_right_90_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, -90.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    void action_rotate_right_135_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(_vector2_curr_dir, -135.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    //----------------------------------------------------------------------




    void rotate_and_move_tick() {

        _vector2_curr_dir = rotate_robot_tick(_vector2_curr_dir, _speed_scale);
        if(!isRotationStarted)
        {
            move_forward_tick(movement_distance, _speed_scale);
        }
    }







    void correct_rigidbody_rotation()
    {
        float angle_correction = 0.0f;
        float scalar_product = 0.0f;
        float module_a = 0.0f;
        float module_b = 0.0f;
        float cos_angle = 0.0f;

        getVector = _rigidbody2D.GetVector(_vector2_curr_dir); 
        
        scalar_product = getVector.x * _vector2_dir_north.x + getVector.y * _vector2_dir_north.y;
        module_a = Mathf.Sqrt(Mathf.Pow(getVector.x, 2.0f) + Mathf.Pow(getVector.y, 2.0f));
        module_b = Mathf.Sqrt(Mathf.Pow(_vector2_dir_north.x, 2.0f) + Mathf.Pow(_vector2_dir_north.y, 2.0f));
        cos_angle = scalar_product / (module_a * module_b);

        angle_correction = Mathf.Acos(cos_angle);
        angle_correction = angle_correction * Mathf.Rad2Deg;


        if ((_vector2_dir_north.x * getVector.y - _vector2_dir_north.y * getVector.x) >= 0.0f) { //check sign of angle (clockwize = -1,  counterwize = 1)
            _rigidbody2D.MoveRotation(_rigidbody2D.rotation + angle_correction);
        } else {
            _rigidbody2D.MoveRotation(_rigidbody2D.rotation - angle_correction);
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
            //in last step our action we correct mismatch
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

        if(angle_deg == 0.0f) {
            return curr_dir;
        }

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

        cur_dir_result = rotate_transform_robot(curr_dir, rotation_tick);

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
        

        if(remain_angle_deg > 1.0f || remain_angle_deg < -1.0f) { //epsilon angle to stop (2 degree)
            cur_dir_result = rotate_transform_robot(curr_dir, rotation_tick);
            remain_angle_deg = remain_angle_deg - rotation_tick;
        } else {
            correct_rigidbody_rotation();
            isRotationStarted = false;
        }
        return cur_dir_result;

    }


    Vector2 rotate_transform_robot(Vector2 curr_dir, float angle_deg)
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