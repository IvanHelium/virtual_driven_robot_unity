using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GameController : MonoBehaviour {

    public static GameController gameController;

    private Dropdown dropdownSerialPort;
    private Button openComPortButton;
    private Canvas canvas;
    private bool isSettingMenuScene;

    private string[] portNames;
    private List<string> portNamesList = new List<string>();

    private static ComPort comPort;

    private float timeToUpdateListOfComPorts = 2.0f;

    private const byte ACTION_LEFT_135 = 0x00;
    private const byte ACTION_LEFT_90 = 0x01;
    private const byte ACTION_LEFT_45 = 0x02;
    private const byte ACTION_FORWARD = 0x03;
    private const byte ACTION_RIGHT_45 = 0x04;
    private const byte ACTION_RIGHT_90 = 0x05;
    private const byte ACTION_RIGHT_135 = 0x06;

    private static RobotMovementController robotMovementController;

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        
        if(scene.name == "GameScene")
        {
            findRobotComponents();
            addRobotActionEventListener();
            addSerialPortCommandReceivedEventListener();
        }
        if (scene.name == "Settings")
        {
            findUIComponentsOnSettingsScene();
            StartCoroutine(refillDropdownList());
        }
    }

    // Use this for initialization
    void Awake() {

        if (gameController == null)
        {
            DontDestroyOnLoad(gameObject);
            gameController = this;
        }
        else if (gameController != this)
        {
            Destroy(gameObject);

        }
        comPort = gameController.GetComponent<ComPort>();
    }
    //-----------------------------------------------------------------------------

    private static void SendRespond(object sender, RobotEventArgs e) //this function send robot's state to control unit
    {
        Debug.Log("send respond : SensorsState = " + e.SensorState);
        //send to serial port
        comPort.sendRespondFromRobot(e.SensorState);
    }

    //-----------------------------------------------------------------------------

    private static void MakeAction(object sender, SerialPortEventArgs e)
    {
        Debug.Log("Make Action number : " + e.NumberOfAction);
        switch(e.NumberOfAction)
        {
            case ACTION_LEFT_135:
                robotMovementController.action_rotate_left_135_and_move_forward();
            break;
            case ACTION_LEFT_90:
                robotMovementController.action_rotate_left_90_and_move_forward();
                break;
            case ACTION_LEFT_45:
                robotMovementController.action_rotate_left_45_and_move_forward();
                break;
            case ACTION_FORWARD:
                robotMovementController.action_move_forward();
                break;
            case ACTION_RIGHT_45:
                robotMovementController.action_rotate_right_45_and_move_forward();
                break;
            case ACTION_RIGHT_90:
                robotMovementController.action_rotate_right_90_and_move_forward();
                break;
            case ACTION_RIGHT_135:
                robotMovementController.action_rotate_right_135_and_move_forward();
                break;
            default:
                Debug.Log("Incorrect action");
            break;
}
    }

    //-----------------------------------------------------------------------------

    void addRobotActionEventListener()
    {
        if(robotMovementController != null)
        {
            robotMovementController.NotifyActionDone += SendRespond;
        }

    }

    void addSerialPortCommandReceivedEventListener()
    {
        if(comPort != null)
        {
            comPort.NotifyActionCommandReceived += MakeAction;
        }
    }

    //-----------------------------------------------------------------------------
    private void findRobotComponents()
    {
        if (UnityEngine.Object.FindObjectOfType<RobotMovementController>() != null)
        {
            robotMovementController = UnityEngine.Object.FindObjectOfType<RobotMovementController>();
            Debug.Log(robotMovementController);
        }
    }

    private void findUIComponentsOnSettingsScene()
    {
        if (GameObject.Find("Canvas") != null)
        {
            canvas = GameObject.Find("Canvas").GetComponentInChildren<Canvas>();
        }
        if (GameObject.Find("DropdownSerialPort") != null)
        {
            dropdownSerialPort = GameObject.Find("DropdownSerialPort").GetComponentInChildren<Dropdown>();
        }
        if (GameObject.Find("ButtonOpenPort") != null)
        {
            openComPortButton = GameObject.Find("ButtonOpenPort").GetComponentInChildren<Button>();
            openComPortButton.onClick.RemoveAllListeners();
            openComPortButton.onClick.AddListener(onClickOpenPort);  
        }
    }
    //-----------------------------------------------------------------------------
    private IEnumerator refillDropdownList()
    {
        while(true)
        {
            dropdownSerialPort.ClearOptions();
            portNames = comPort.GetPortsNames();
            portNamesList.Clear();
            for (int i = 0; i < portNames.Length; i++)
            {
                portNamesList.Add(portNames[i]);
            }
            dropdownSerialPort.AddOptions(portNamesList);
            yield return new WaitForSeconds(timeToUpdateListOfComPorts);
        }
    }

    private void onClickOpenPort()
    {
        comPort.setUpSerialPort(dropdownSerialPort.captionText.text);
        comPort.openSerialPort();
    }


    void OnApplicationQuit()
    {
        comPort.closeSerialPort();
        //save someone
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

 



    //----------------------------------------------------------------------
    // dont use for a while its just prototype
    //----------------------------------------------------------------------
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        //data.WINS = WINS;
        //data.LOSES = LOSES;

        bf.Serialize(file, data);
        file.Close();
    }


    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //WINS = data.WINS;
           // LOSES = data.LOSES;
        }
    }

	// Update is called once per frame
	void Update () {

    }
}

[Serializable]
class PlayerData //will be example
{
    public int WINS;
    public int LOSES;
}