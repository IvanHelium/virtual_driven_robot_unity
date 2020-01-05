using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneSwitcher : MonoBehaviour {

    private int sceneToLoad;


    private AudioSource audio;

    // Use this for initialization
    void Start () {

        sceneToLoad = Application.loadedLevel;

        

    }




   
    // Update is called once per frame
    void Update () {
       

    }


    private void sceneLoad(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        SceneManager.sceneLoaded += OnSceneLoaded;

        
    }

    public void loadMenuScene()
    {
        sceneLoad(0);
    }
    public void loadGameScene()
    {
        sceneLoad(1); 
    }

    public void loadSettingsScene()
    {
        sceneLoad(2);
    }

    public void quitApplication()
    {
        Application.Quit();
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; //destroy connections
    }




}


