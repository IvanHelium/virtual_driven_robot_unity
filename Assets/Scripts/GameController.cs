using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CatcherData
{
    private bool isFirstCapturedValue = false;
    public ushort sensorStateCatched = 0;
    public byte numberOfActionCatched = 0;
    

    void firstCaptureDataOccured()
    {
        isFirstCapturedValue = true;
    }
    bool getIsFirstCapturedValue()
    {
        return isFirstCapturedValue;
    }


}

public class GameController : MonoBehaviour {

    public static GameController gameController;

    private Dropdown dropdownSerialPort;
    private Button openComPortButton;
    private InputField inputFieldNumberOfExperimentMaximum;
    private InputField inputFieldCurrentExperiment;
    private InputField inputFieldFileName;
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

    private static Logger logger;

    private static CatcherData catherData = new CatcherData();

    private static int numberOfExperiment = 0;

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

        if (scene.name == "GameScene")
        {
            findRobotComponents();
            addRobotActionEventListener();
            addSerialPortCommandReceivedEventListener();
        }
        if (scene.name == "Settings")
        {
            findUIComponentsOnSettingsScene();
            logger.loadExperimentConfigAndSetupBuffers();
            StartCoroutine(refillDropdownList());
            loadInputFields();
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
        logger = gameController.GetComponent<Logger>();
    }

    //-----------------------------------------------------------------------------

    static int Grade(ushort sensorData)
    {
        return sensorData * 10;
    }


    //-----------------------------------------------------------------------------
    //mitm (men in the middle)
    //-----------------------------------------------------------------------------
    private static void CatchSensorState(object sender, RobotEventArgs e)
    {
        catherData.sensorStateCatched = e.SensorState;
    }

    private static void CatchActionAndLogData(object sender, SerialPortEventArgs e)
    {
        int grade = 0;
        catherData.numberOfActionCatched = e.NumberOfAction;
        //grade here
        grade = Grade(catherData.sensorStateCatched);
        //log here
        logger.getLogsData(catherData.sensorStateCatched, catherData.numberOfActionCatched, grade);

        Save(logger);
        
    }

    private static void Save(Logger logger)
    {
        if(numberOfExperiment % 1000 == 0 && numberOfExperiment != 0)
        {
            Debug.Log(numberOfExperiment);
            logger.PrepareAndSave();
        }
        if(numberOfExperiment < logger.getNumberOfExperimentMaximum() - 1)
        {
            numberOfExperiment++;
        }       
    }

    //-----------------------------------------------------------------------------

    private static void SendRespond(object sender, RobotEventArgs e) //this function send robot's state to control unit
    {
        //Debug.Log("send respond : SensorsState = " + e.SensorState);
        //send to serial port
        comPort.sendRespondFromRobot(e.SensorState);
    }

    //-----------------------------------------------------------------------------

    private static void MakeAction(object sender, SerialPortEventArgs e)
    {
        //Debug.Log("Make Action number : " + e.NumberOfAction);
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
            robotMovementController.NotifyActionDone += CatchSensorState;
        }

    }

    void addSerialPortCommandReceivedEventListener()
    {
        if(comPort != null)
        {
            comPort.NotifyActionCommandReceived += MakeAction;
            comPort.NotifyActionCommandReceived += CatchActionAndLogData;
        }
    }
    //----------------------------------------------------------------------------
    // GUI part
    //-----------------------------------------------------------------------------


    private void findRobotComponents()
    {
        if (UnityEngine.Object.FindObjectOfType<RobotMovementController>() != null)
        {
            robotMovementController = UnityEngine.Object.FindObjectOfType<RobotMovementController>();
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
        if(GameObject.Find("InputFieldNumberOfExperimentMaximum") != null)
        {
            inputFieldNumberOfExperimentMaximum = GameObject.Find("InputFieldNumberOfExperimentMaximum").GetComponentInChildren<InputField>();
            inputFieldNumberOfExperimentMaximum.onEndEdit.RemoveAllListeners();
            inputFieldNumberOfExperimentMaximum.onEndEdit.AddListener(onEndEditInputFieldNumberOfExperimentMaximum);
        }
        if (GameObject.Find("InputFieldCurrentExperiment") != null)
        {
            inputFieldCurrentExperiment = GameObject.Find("InputFieldCurrentExperiment").GetComponentInChildren<InputField>();
            inputFieldCurrentExperiment.onEndEdit.RemoveAllListeners();
            inputFieldCurrentExperiment.onEndEdit.AddListener(onEndEditInputFieldCurrentExperiment);
        }
        if (GameObject.Find("InputFieldFileName") != null)
        {
            inputFieldFileName = GameObject.Find("InputFieldFileName").GetComponentInChildren<InputField>();
            inputFieldFileName.onEndEdit.RemoveAllListeners();
            inputFieldFileName.onEndEdit.AddListener(onEndEditInputFieldFileName);
        }
    }

    private void onEndEditInputFieldNumberOfExperimentMaximum(string s)
    {
        logger.configureNumberOfExperimentMaximum(Convert.ToInt32(s));
    }

    private void onEndEditInputFieldCurrentExperiment(string s)
    {
        logger.setCurrentExperimentNumber(Convert.ToInt32(s));
    }

    private void onEndEditInputFieldFileName(string s)
    {
        logger.setFileName(s);
    }


    private void loadInputFields()
    {
        if(inputFieldNumberOfExperimentMaximum != null)
        {
            inputFieldNumberOfExperimentMaximum.SetTextWithoutNotify( Convert.ToString(logger.getNumberOfExperimentMaximum()));
        }
        if(inputFieldCurrentExperiment != null)
        {
            inputFieldCurrentExperiment.SetTextWithoutNotify(Convert.ToString(logger.getCurrentExperimentNumber()));
        }
        if(inputFieldFileName != null)
        {
            inputFieldFileName.SetTextWithoutNotify(Convert.ToString(logger.getFileName()));
        }
    }
    //-----------------------------------------------------------------------------
    private IEnumerator refillDropdownList()
    {
        int currentDropdownIndex = 0;
        while(true)
        {
            currentDropdownIndex = dropdownSerialPort.value;
            dropdownSerialPort.ClearOptions();
            portNames = comPort.GetPortsNames();
            if(portNames == null)
            {
                Debug.Log("portNames empty breaking...");
                break;
            }
            portNamesList.Clear();
            for (int i = 0; i < portNames.Length; i++)
            {
                portNamesList.Add(portNames[i]);
            }
            dropdownSerialPort.AddOptions(portNamesList);
            dropdownSerialPort.value = currentDropdownIndex;
            yield return new WaitForSeconds(timeToUpdateListOfComPorts);
        }
    }

    private void onClickOpenPort()
    {
        comPort.setUpSerialPort(dropdownSerialPort.captionText.text);
        comPort.openSerialPort();
    }

    private void OnDestroy()
    {
        logger.saveExperimentConfig();
        Debug.Log("experiment config saved");
        
    }
    void OnApplicationQuit()
    {
        comPort.closeSerialPort();
        Debug.Log("serial port closed");
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

 


	// Update is called once per frame
	void Update () {

    }
}
