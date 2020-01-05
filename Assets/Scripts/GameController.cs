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

    private ComPort comPort;

    private float timeToUpdateListOfComPorts = 2.0f;



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
        findUIComponents();
        if (isSettingMenuScene)
        {
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


    private void findUIComponents()
    {
        isSettingMenuScene = false;
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
            isSettingMenuScene = true;
        }
    }

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