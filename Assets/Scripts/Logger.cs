using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Logger : MonoBehaviour
{

    private const int NUMBER_OF_EXPERIMENT_MAXIMUM_DEFAULT = 10000;

    private static ExperimentConfig experimentConfig = new ExperimentConfig();

    private static int currentExperimentNumberPrev = 0;

    private ushort[] sensorsDataLogs;
    private byte[] actionsDataLogs;
    private int[] gradesDataLogs;

    public void getLogsData(ushort sensorState, byte actionNumber, int grade)
    {
        setCurrentExperimentNumber(experimentConfig.currentExperimentNumber + 1);
        try
        {
            sensorsDataLogs[experimentConfig.currentExperimentNumber] = sensorState;
            actionsDataLogs[experimentConfig.currentExperimentNumber] = actionNumber;
            gradesDataLogs[experimentConfig.currentExperimentNumber] = grade;
        }
        catch (IndexOutOfRangeException e)
        {
            // Perform some action here, such as logging this exception.
            throw;
        }
    }

    public void configureNumberOfExperimentMaximum(int value)
    {
        Debug.Log(" configured number of experiment maximum = " + value);
        setNumberOfExperimentMaximum(value);
        InitializeDataLogsBuffers();
    }

    public void setNumberOfExperimentMaximum(int value)
    {
        experimentConfig.numberOfExperimentMaximum = value;
    }

    public int getNumberOfExperimentMaximum()
    {
        return experimentConfig.numberOfExperimentMaximum;
    }

    public void setCurrentExperimentNumber(int value)
    {
        if(value >= experimentConfig.numberOfExperimentMaximum)
        {
            Debug.Log("current experiment more than maximum number of experiments ... set value to zero");
            value = 0;
        }
        //Debug.Log("current experiment now is " + value);
        experimentConfig.currentExperimentNumber = value;
    }

    public int getCurrentExperimentNumber()
    {
        return experimentConfig.currentExperimentNumber;
    }

    public void setFileName(string filename)
    {
        Debug.Log("filename now is " + filename);
        experimentConfig.fileName = filename;
    }

    public string getFileName()
    {
        return experimentConfig.fileName;
    }

    public void loadExperimentConfigAndSetupBuffers()
    {
        ExperimentConfigSetDefault();
        loadExperimentConfigFromFile();
        InitializeDataLogsBuffers();
    }

    public void saveExperimentConfig()
    {
        saveExperimentConfigToFile();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }


    private void InitializeDataLogsBuffers()
    {
        try
        {
            sensorsDataLogs = new ushort[experimentConfig.numberOfExperimentMaximum];
            actionsDataLogs = new byte[experimentConfig.numberOfExperimentMaximum];
            gradesDataLogs = new int[experimentConfig.numberOfExperimentMaximum];
        }
        catch (Exception exeption)
        {
            Debug.Log(exeption);
        }
    }

    private void ExperimentConfigSetDefault()
    {
        experimentConfig.numberOfExperimentMaximum = NUMBER_OF_EXPERIMENT_MAXIMUM_DEFAULT;
        experimentConfig.currentExperimentNumber = 0;
        experimentConfig.fileName = "defaultFileName";
    }

    private void saveExperimentConfigToFile()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(Application.persistentDataPath + "/ExperimentConfigInfo.dat", FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, experimentConfig);
        }
        Debug.Log("config saved \n");
    }


    private void loadExperimentConfigFromFile()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(Application.persistentDataPath + "/ExperimentConfigInfo.dat", FileMode.OpenOrCreate))
        {
            experimentConfig = (ExperimentConfig)binaryFormatter.Deserialize(fileStream);
        }
        Debug.Log("config loaded \n");
    }

    public async void PrepareAndSave() 
    {
        string stringToWrite = " ";
        Debug.Log("Started writing to file... ");
        await Task.Run(() => stringToWrite = PrepareDataForWriteToTxtFile());
        await SaveExperimentalDataToTxtFile(stringToWrite);
        Debug.Log("Ended writing to file... ");
    }

    private string PrepareDataForWriteToTxtFile()
    {
        string str = "";
        for(int i = currentExperimentNumberPrev; i < experimentConfig.currentExperimentNumber; i++)
        {
            str += i.ToString() + ") " + sensorsDataLogs[i].ToString()
                + "     " + actionsDataLogs[i].ToString()
                + "     " + gradesDataLogs[i].ToString() +"\n";
        }
        currentExperimentNumberPrev = experimentConfig.currentExperimentNumber;
        return str;
    }

    private async Task SaveExperimentalDataToTxtFile(string str)
    {
        using (StreamWriter writer = new StreamWriter(experimentConfig.fileName, true))
        {
            await writer.WriteLineAsync(str);  // асинхронная запись в файл
        }
    }
}



[Serializable]
class ExperimentConfig
{
    public int numberOfExperimentMaximum;
    public int currentExperimentNumber;
    public string fileName;
}