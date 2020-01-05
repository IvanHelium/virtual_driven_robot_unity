using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class ComPort : MonoBehaviour
{
    // Start is called before the first frame update

    private SerialPort _serial = new SerialPort();

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void setUpSerialPort(string serialPortName)
    {
        if (_serial.IsOpen)
        {
            Debug.Log("port already opened");
            return;
        }
        _serial.PortName = serialPortName;
        _serial.BaudRate = 115200;
        _serial.Parity = Parity.None;
        _serial.DataBits = 8;
        _serial.StopBits = StopBits.One;
        Debug.Log("port " + serialPortName + " configured");
    }

    public bool openSerialPort()
    {
        if (_serial.IsOpen)
        {
            return false;
        }
        else
        {
            _serial.Open();
            Debug.Log("port opened ");
        }
        return true;
    }

    public bool closeSerialPort()
    {
        if (_serial.IsOpen)
        {
            _serial.Close();
            Debug.Log("port succesfully closed ");
            return true;
        }
        Debug.Log("port already closed");
        return false;
    }

    public string[] GetPortsNames()
    {

        string[] portNames;

        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxPlayer:
            default: // Windows
                portNames = System.IO.Ports.SerialPort.GetPortNames();
                break;

        }
        if (portNames.Length > 0)
            return portNames;
        else
            return null;

    }

}
