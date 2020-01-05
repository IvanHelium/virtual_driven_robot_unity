using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SensorHandler : MonoBehaviour
{

    public GameObject searchingTarget;

    private ushort _touching_sensor_state;
    public ushort _direction_sensor_state;
    private ushort _distance_sensor_state;

    private ushort g_global_sensor_state; //sensor state which we will be using for data transfer to robot

    private List<Collider2D> childrenColliders;
    private List<SpriteRenderer> childrenSpriteRenderer;
    private List<SpriteRenderer> childrenSpriteRendererDirections;

    private float directionToTargetDegree;
    private Vector2 directionToTargetVec = new Vector2(0.0f, 0.0f);

    private float maxDistance;
    private float sectorDistance;
    private const int NUMBERS_OF_SECTORS = 16;

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

    private const int SECTOR_1_SPRITE_INDEX = 0;
    private const int SECTOR_2_SPRITE_INDEX = 11;
    private const int SECTOR_3_SPRITE_INDEX = 1;
    private const int SECTOR_4_SPRITE_INDEX = 10;
    private const int SECTOR_5_SPRITE_INDEX = 2;
    private const int SECTOR_6_SPRITE_INDEX = 9;
    private const int SECTOR_7_SPRITE_INDEX = 3;
    private const int SECTOR_8_SPRITE_INDEX = 8;
    private const int SECTOR_9_SPRITE_INDEX = 4;
    private const int SECTOR_10_SPRITE_INDEX = 7;
    private const int SECTOR_11_SPRITE_INDEX = 5;
    private const int SECTOR_12_SPRITE_INDEX = 6;

    // Start is called before the first frame update
    void Start()
    {
        childrenColliders = new List<Collider2D>();
        childrenSpriteRenderer = new List<SpriteRenderer>();
        childrenSpriteRendererDirections = new List<SpriteRenderer>();
        robotMovementController = gameObject.GetComponent<RobotMovementController>();
        AddChildrenSpriteRenderer(transform);
        AddChildrenSpriteRendererDirections(transform);
        AddChildrenColliders(transform);
        initializeDistanceSensor();
    }

    // Update is called once per frame
    void Update()
    {
        drawDistanceSectorLine();
    }

    private void FixedUpdate()
    {
        calculateDirectionTick();
        calculateDistanceTick();
        mergeSensorData();
    }


    //-----------------------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------------------
    void mergeSensorData()
    {
    g_global_sensor_state &= 0xFFF8;
    g_global_sensor_state |= (_touching_sensor_state); //update touching sensor state
    g_global_sensor_state &= 0xFF87;
    g_global_sensor_state |= (ushort)(((_direction_sensor_state)) << 3); //update direction sensor state
    g_global_sensor_state &= 0xF87F;
    g_global_sensor_state |= (ushort)(((uint)(_distance_sensor_state)) << 7); //update distance sensor state
    }

    public ushort getGlobalSensorState()
    {
        return g_global_sensor_state;
    }

    //-----------------------------------------------------------------------------------------
    //work with distances
    //-----------------------------------------------------------------------------------------
    void calculateDistanceTick()
    {
        float currentDistance;
        currentDistance = calculateDistance();
        if (currentDistance > sectorDistance * 15)
        {
            _distance_sensor_state = 15;
        }
        else if(currentDistance > sectorDistance * 14)
        {
            _distance_sensor_state = 14;
        }
        else if (currentDistance > sectorDistance * 13)
        {
            _distance_sensor_state = 13;
        }
        else if (currentDistance > sectorDistance * 12)
        {
            _distance_sensor_state = 12;
        }
        else if (currentDistance > sectorDistance * 11)
        {
            _distance_sensor_state = 11;
        }
        else if (currentDistance > sectorDistance * 10)
        {
            _distance_sensor_state = 10;
        }
        else if (currentDistance > sectorDistance * 9)
        {
            _distance_sensor_state = 9;
        }
        else if (currentDistance > sectorDistance * 8)
        {
            _distance_sensor_state = 8;
        }
        else if (currentDistance > sectorDistance * 7)
        {
            _distance_sensor_state = 7;
        }
        else if (currentDistance > sectorDistance * 6)
        {
            _distance_sensor_state = 6;
        }
        else if (currentDistance > sectorDistance * 5)
        {
            _distance_sensor_state = 5;
        }
        else if (currentDistance > sectorDistance * 4)
        {
            _distance_sensor_state = 4;
        }
        else if (currentDistance > sectorDistance * 3)
        {
            _distance_sensor_state = 3;
        }
        else if (currentDistance > sectorDistance * 2)
        {
            _distance_sensor_state = 2;
        }
        else if (currentDistance > sectorDistance * 1)
        {
            _distance_sensor_state = 1;
        }
        else if (currentDistance > sectorDistance * 0)
        {
            _distance_sensor_state = 0;
        }
    }

    void initializeDistanceSensor() 
    {
        _distance_sensor_state = 255;
        initializeMaxDistance(calculateDistance());
    }
    void initializeMaxDistance(float maxDistanceValue)
    {   //whole distance devides into 16 sectors to make distance discrete
        maxDistance = maxDistanceValue;
        sectorDistance = maxDistance / NUMBERS_OF_SECTORS;
    }

    float calculateDistance()
    {
        Transform robotTransform;
        Transform targetTransform;
        robotTransform = transform;
        targetTransform = searchingTarget.transform;
        if (searchingTarget == null)
        {
            return -1.0f;
        }
        return Vector2.Distance(targetTransform.position, robotTransform.position);
    }

    //-----------------------------------------------------------------------------------------
    //draw line for distance sensor (with sectors)

    
    void drawDistanceSectorLine()
    {
        int numbersOfSectors;
        Transform robotTransform;
        Transform targetTransform;
        robotTransform = transform;
        targetTransform = searchingTarget.transform;
        numbersOfSectors = 0;
        Vector2 fromTargetToRobotVector = robotTransform.position - targetTransform.position;
        Vector2 fromTargetToRobotVectorNormilized = (fromTargetToRobotVector / fromTargetToRobotVector.magnitude) * sectorDistance; //calculate vector of one sector with magnitude equal to sectorDistance
        Vector2 currentPositionAtLine = targetTransform.position;
        Debug.DrawLine(robotTransform.position, targetTransform.position, Color.red, 0.0f);
        if (calculateDistance() > maxDistance)
        {
            numbersOfSectors = NUMBERS_OF_SECTORS;
        }
        else
        {
            numbersOfSectors = (int)(calculateDistance() / sectorDistance);
        }
        for (int i = 0; i < numbersOfSectors; i++)
        {
            currentPositionAtLine = currentPositionAtLine + fromTargetToRobotVectorNormilized; //next sector point(sector) on distance line
            Debug.DrawLine(currentPositionAtLine, currentPositionAtLine + Vector2.Perpendicular(fromTargetToRobotVectorNormilized), Color.red, 0.0f); //draw perpendicular line
        }




    }
    //-----------------------------------------------------------------------------------------
    //work with directions
    //-----------------------------------------------------------------------------------------
    byte changeDirectionSensorState(float directionToTargetDegree)
    {
        byte directionSensorState = 12;
        //directionToTargetDegree

        if(isSector(SECTOR_1_LEFT_BOARDER, SECTOR_1_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 1;
            highlightDirectionSpriteRenderer(SECTOR_1_SPRITE_INDEX);
        }
        if(isSector(SECTOR_2_LEFT_BOARDER, SECTOR_2_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 2;
            highlightDirectionSpriteRenderer(SECTOR_2_SPRITE_INDEX);
        }
        if(isSector(SECTOR_3_LEFT_BOARDER, SECTOR_3_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 3;
            highlightDirectionSpriteRenderer(SECTOR_3_SPRITE_INDEX);
        }
        if(isSector(SECTOR_4_LEFT_BOARDER, SECTOR_4_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 4;
            highlightDirectionSpriteRenderer(SECTOR_4_SPRITE_INDEX);
        }
        if(isSector(SECTOR_5_LEFT_BOARDER, SECTOR_5_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 5;
            highlightDirectionSpriteRenderer(SECTOR_5_SPRITE_INDEX);
        }
        if(isSector(SECTOR_6_LEFT_BOARDER, SECTOR_6_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 6;
            highlightDirectionSpriteRenderer(SECTOR_6_SPRITE_INDEX);
        }
        if(isSector(SECTOR_7_LEFT_BOARDER, SECTOR_7_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 7;
            highlightDirectionSpriteRenderer(SECTOR_7_SPRITE_INDEX);
        }
        if(isSector(SECTOR_8_LEFT_BOARDER, SECTOR_8_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 8;
            highlightDirectionSpriteRenderer(SECTOR_8_SPRITE_INDEX);
        }
        if(isSector(SECTOR_9_LEFT_BOARDER, SECTOR_9_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 9;
            highlightDirectionSpriteRenderer(SECTOR_9_SPRITE_INDEX);
        }
        if(isSector(SECTOR_10_LEFT_BOARDER, SECTOR_10_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 10;
            highlightDirectionSpriteRenderer(SECTOR_10_SPRITE_INDEX);
        }
        if(isSector(SECTOR_11_LEFT_BOARDER, SECTOR_11_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 11;
            highlightDirectionSpriteRenderer(SECTOR_11_SPRITE_INDEX);
        }
        if(isSectorCloseToCrossOver(SECTOR_12_LEFT_BOARDER, SECTOR_12_RIGHT_BOARDER, directionToTargetDegree))
        {
            directionSensorState = 12;
            highlightDirectionSpriteRenderer(SECTOR_12_SPRITE_INDEX);
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


    void calculateDirectionTick()
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

    void highlightDirectionSpriteRenderer(int index)
    {
        if (index < 0 || index > childrenSpriteRendererDirections.Count || childrenSpriteRendererDirections[index].color == Color.red)
            return;
        for(int j = 0; j < childrenSpriteRendererDirections.Count; j++)
        {
            childrenSpriteRendererDirections[j].color = Color.white;
        }
        childrenSpriteRendererDirections[index].color = Color.red;
    }


    //-----------------------------------------------------------------------------------------
    //work with colliders and collisions
    //-----------------------------------------------------------------------------------------


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


    private void AddChildrenSpriteRendererDirections(Transform t)
    {
        for (int i = 3; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            SpriteRenderer spriteRenderer = child.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                childrenSpriteRendererDirections.Add(spriteRenderer);
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
                _touching_sensor_state |= (ushort)(1 << i);
                childrenSpriteRenderer[i].color = Color.red;
            }
            else
            {
                _touching_sensor_state &= ((ushort)~(1 << i));
                childrenSpriteRenderer[i].color = Color.white;
            }
                
        }

        
    }


}
