using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Video;


public class GameController : MonoBehaviour {

    public static GameController gameController;





    //AudioSource backgroundMusic;

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

        //backgroundMusic = GetComponent<AudioSource>();
        //audio.Pause();
        

    }

    

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