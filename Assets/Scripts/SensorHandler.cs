using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorHandler : MonoBehaviour
{

    public GameObject searchingTarget;

    private byte _sensor_state;
    public byte _direction_sensor_state;

    private List<Collider2D> childrenColliders;
    private List<SpriteRenderer> childrenSpriteRenderer;

    public float directionToTargetDegree;
    private Vector2 directionToTargetVec = new Vector2(0.0f, 0.0f); 

    private RobotMovementController robotMovementController;

    private const float SECTOR_1_LEFT_BOARDER    = -15.0f;
    private const float SECTOR_1_RIGHT_BOARDER   =  15.0f;
    private const float SECTOR_2_LEFT_BOARDER    =  15.0f;
    private const float SECTOR_2_RIGHT_BOARDER   =  45.0f;
    private const float SECTOR_3_LEFT_BOARDER    = -45.0f;
    private const float SECTOR_3_RIGHT_BOARDER   = -15.0f;
    private const float SECTOR_4_LEFT_BOARDER    =  45.0f;
    private const float SECTOR_4_RIGHT_BOARDER   =  75.0f;

    private const float SECTOR_5_LEFT_BOARDER    = -75.0f;
    private const float SECTOR_5_RIGHT_BOARDER   = -45.0f;
    private const float SECTOR_6_LEFT_BOARDER    =  75.0f;
    private const float SECTOR_6_RIGHT_BOARDER   = 105.0f;
    private const float SECTOR_7_LEFT_BOARDER    =-105.0f;
    private const float SECTOR_7_RIGHT_BOARDER   = -75.0f;
    private const float SECTOR_8_LEFT_BOARDER    = 105.0f;
    private const float SECTOR_8_RIGHT_BOARDER   = 135.0f;
    private const float SECTOR_9_LEFT_BOARDER    =-135.0f;
    private const float SECTOR_9_RIGHT_BOARDER   =-105.0f;
    private const float SECTOR_10_LEFT_BOARDER   = 135.0f;
    private const float SECTOR_10_RIGHT_BOARDER  = 165.0f;
    private const float SECTOR_11_LEFT_BOARDER   =-165.0f;
    private const float SECTOR_11_RIGHT_BOARDER  =-135.0f;
    private const float SECTOR_12_LEFT_BOARDER   =-165.0f;
    private const float SECTOR_12_RIGHT_BOARDER  = 165.0f;

    // Start is called before the first frame update
    void Start()
    {
        childrenColliders = new List<Collider2D>();
        childrenSpriteRenderer = new List<SpriteRenderer>();
        robotMovementController = gameObject.GetComponent<RobotMovementController>();
        AddChildrenSpriteRenderer(transform);
        AddChildrenColliders(transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        calculateDirection();
    }

    byte changeDirectionSensorState(float directionToTargetDegree)
    {
        byte directionSensorState = 12;
        //directionToTargetDegree

        if(isSector(SECTOR_1_LEFT_BOARDER, SECTOR_1_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 1;
        }
        if(isSector(SECTOR_2_LEFT_BOARDER, SECTOR_2_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 2;
        }
        if(isSector(SECTOR_3_LEFT_BOARDER, SECTOR_3_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 3;
        }
        if(isSector(SECTOR_4_LEFT_BOARDER, SECTOR_4_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 4;
        }
        if(isSector(SECTOR_5_LEFT_BOARDER, SECTOR_5_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 5;
        }
        if(isSector(SECTOR_6_LEFT_BOARDER, SECTOR_6_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 6;
        }
        if(isSector(SECTOR_7_LEFT_BOARDER, SECTOR_7_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 7;
        }
        if(isSector(SECTOR_8_LEFT_BOARDER, SECTOR_8_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 8;
        }
        if(isSector(SECTOR_9_LEFT_BOARDER, SECTOR_9_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 9;
        }
        if(isSector(SECTOR_10_LEFT_BOARDER, SECTOR_10_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 10;
        }
        if(isSector(SECTOR_11_LEFT_BOARDER, SECTOR_11_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 11;
        }
        if(isSectorCloseToCrossOver(SECTOR_12_LEFT_BOARDER, SECTOR_12_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 12;
        }
        //draw here

        return directionSensorState;
    }

    bool isSector(float left, float right, float value)
    {
        if(value > left && value < right)
        {
            return true;
        }
        return false;
    }
    bool isSectorCloseToCrossOver(float left, float right, float value) //sector between -165 degree and 165 degree
    {
        if(value < left || value > right)
        {
            return true;
        }
        return false;
    }
    void calculateDirection()
    {
        Vector2 currentRobotDirection = robotMovementController.getCurrentRobotDirection();
        Transform robotTransform = transform;
        if(searchingTarget == null)
        {
            return;
        }
        Transform targetTransform = searchingTarget.transform;
        // private 
        directionToTargetVec = targetTransform.position - robotTransform.position;
        directionToTargetDegree = Vector2.SignedAngle(currentRobotDirection, directionToTargetVec);

        _direction_sensor_state = changeDirectionSensorState(directionToTargetDegree);
    }


    private void AddChildrenColliders(Transform t)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            AddChildrenColliders(child);
            Collider2D c = child.gameObject.GetComponent<Collider2D>();
            if (c != null)
                childrenColliders.Add(c);
        }
    }

    private void AddChildrenSpriteRenderer(Transform t)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            //AddChildrenSpriteRenderer(child);
            SpriteRenderer spriteRenderer = child.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                childrenSpriteRenderer.Add(spriteRenderer);
        }
    }


    private void OnTriggerEnter2D(Collider2D collider_other)
    {
        checkChildColliderTrigger(collider_other);
    }

    private void OnTriggerStay2D(Collider2D collider_other)
    {
        checkChildColliderTrigger(collider_other);
    }

    private void OnTriggerExit2D(Collider2D collider_other)
    {
        checkChildColliderTrigger(collider_other);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnCollisionStay2D(Collision2D collision)
    {

    }
    private void OnCollisionExit2D(Collision2D collision)
    {

    }




    void checkChildColliderTrigger(Collider2D col)
    {
        
        for (int i = 0; i < childrenColliders.Count; ++i)
        {
            //Whether this collider is touching any collider on the specified layerMask or not.
            if (childrenColliders[i].IsTouchingLayers()) 
            {
                _sensor_state |= (byte)(1 << i);
                childrenSpriteRenderer[i].color = Color.red;
            }
            else
            {
                _sensor_state &= ((byte)~(1 << i));
                childrenSpriteRenderer[i].color = Color.white;
            }
                
        }

        
    }


}
