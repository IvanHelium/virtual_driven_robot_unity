using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotEventArgs
{
    public ushort SensorState { get; }
    public RobotEventArgs(ushort sensorState)
    {
        SensorState = sensorState;
    }
}

public class RobotMovementController : MonoBehaviour
{
    public float _speed_scale = 0.01f;
    private Rigidbody2D _rigidbody2D;

    private bool isRotationStarted = false;
    private float remain_angle_deg = 0.0f;
    private float start_angle_deg = 0.0f;

    private bool isMovingStarted = false;
    private float remain_movement_distance = 0.0f;
    private float movement_distance = 2.5f;

    private Vector2 getVector; //vector between our movement direction (_vector2_curr_dir) and vector of robot rigidbody (it is not same)
    private SensorHandler sensorHandler;

    public delegate void RobotHandler(object sender, RobotEventArgs e);
    public event RobotHandler NotifyActionDone;              

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = this.GetComponent<Rigidbody2D>();
        sensorHandler = this.GetComponent<SensorHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //fixed update is called once per phisics calculate tick
    private void FixedUpdate()
    {
        rotate_and_move_tick();

        //action_move_forward();//debug
        action_rotate_left_90_and_move_forward();//debug


    }


    //----------------------------------------------------------------------
    //
    // utilities functions
    //
    //----------------------------------------------------------------------

    public Vector2 getCurrentRobotDirection()
    {
        return _rigidbody2D.transform.up;
    }

    //----------------------------------------------------------------------
    //----------------------------------------------------------------------
    //
    // different actions
    //
    //----------------------------------------------------------------------
    //----------------------------------------------------------------------

    public void action_rotate_left_45_and_move_forward(){
        if(isMovingStarted || isRotationStarted) {
            return;
        }
        rotate_robot_start(45.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    public void action_rotate_left_90_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(90.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }
    public void action_rotate_left_135_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(135.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }
    //----------------------------------------------------------------------
    public void action_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(0.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    //----------------------------------------------------------------------
    public void action_rotate_right_45_and_move_forward() {
        if (isMovingStarted || isRotationStarted) {
            return;
        }
        rotate_robot_start(-45.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    public void action_rotate_right_90_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start(-90.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    public void action_rotate_right_135_and_move_forward()
    {
        if (isMovingStarted || isRotationStarted)
        {
            return;
        }
        rotate_robot_start( -135.0f, _speed_scale);
        move_forward_start(movement_distance, _speed_scale);
    }

    //----------------------------------------------------------------------




    void rotate_and_move_tick() {

        rotate_robot_tick( _speed_scale);
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
        Vector2 current_direction = _rigidbody2D.transform.up;
        float movement_speed_cur = 0.0f;
        float movement_tick = 0.0f;

        ushort sensorsState = 0x0000;


        if (movement_speed < 1.0f)
        {
            movement_speed_cur = movement_speed;
        }
        else
        {
            movement_speed_cur = 1.0f;
        }
        movement_tick  = distance * movement_speed_cur;

        
        if(remain_movement_distance > 0.0f)
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + current_direction * movement_tick);
            remain_movement_distance = remain_movement_distance - movement_tick;
        } else
        {   //in last step our action we correct mismatch
            //action is done
            isMovingStarted = false;
            //notify about event
            sensorsState = sensorHandler.getGlobalSensorState();
            NotifyActionDone?.Invoke(this, new RobotEventArgs(sensorsState) );
        }
        
    }

    //------------------------------------------------------------------------------------------------
    //
    //rotation part
    //
    //------------------------------------------------------------------------------------------------

    void rotate_robot_start(float angle_deg, float rotation_speed)
    {
        float rotation_speed_cur = 0.0f;
        float rotation_tick = 0.0f;
        if(angle_deg == 0.0f) {
            return;
        }
        if (rotation_speed < 1.0f)
        {
            rotation_speed_cur = rotation_speed;
        }
        else
        {
            rotation_speed_cur = 1.0f;
        }
        isRotationStarted = true;
        rotation_tick = angle_deg * rotation_speed;
        remain_angle_deg = angle_deg;
        start_angle_deg = angle_deg;
        rotate_transform_robot(rotation_tick);
        remain_angle_deg = angle_deg - rotation_tick;  
    }

    void rotate_robot_tick(float rotation_speed)
    {
        float rotation_speed_cur = 0.0f;
        float rotation_tick = 0.0f;
        getVector = _rigidbody2D.transform.up; //debug

        if (isRotationStarted == false){
            return;
        }
        if (rotation_speed < 1.0f) {
            rotation_speed_cur = rotation_speed;
        }
        else {
            rotation_speed_cur = 1.0f;
        }
        rotation_tick = start_angle_deg * rotation_speed;
        if(remain_angle_deg > 1.0f || remain_angle_deg < -1.0f) { //epsilon angle to stop (2 degree)
            rotate_transform_robot(rotation_tick);
            remain_angle_deg = remain_angle_deg - rotation_tick;
        } else {
            isRotationStarted = false;
        }
    }


    void rotate_transform_robot(float angle_deg)
    {
        _rigidbody2D.MoveRotation( _rigidbody2D.rotation + angle_deg);
        if (_rigidbody2D.rotation > 360.0f || _rigidbody2D.rotation < -360.0f)
        {
            _rigidbody2D.SetRotation(_rigidbody2D.rotation % 360.0f);
        }
    }
}

