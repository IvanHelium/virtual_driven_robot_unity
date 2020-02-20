using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO.Ports;
using UnityEngine;
using System.Threading;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RespondSensorDataPacket
{   
    public byte Header;
    public byte Type;
    [MarshalAs(
       UnmanagedType.ByValArray,
       SizeConst = 2)]
    public byte[] Data;
    public byte Sum;

    public RespondSensorDataPacket(byte header, byte type, byte[] data)
    {
        Header = header;
        Type = type;
        Data = data;
        Sum = 0;
        Data = data;
    }
}


public class SerialPortEventArgs
{
    public byte NumberOfAction { get; }
    public SerialPortEventArgs(byte numberOfAction)
    {
        NumberOfAction = numberOfAction;
    }
}

public class ComPort : MonoBehaviour
{

    public delegate void SerialPortHandler(object sender, SerialPortEventArgs e);
    public event SerialPortHandler NotifyActionCommandReceived;


    // Start is called before the first frame update
    private SerialPort _serial = new SerialPort();

    //state machine variables
    private static int stateMachineState;
    private const int HEADER_STATE = 0x00;
    private const int PACKET_TYPE_STATE = 0x01;
    private const int ACTION_NUMBER_STATE = 0x02;
    private const int CHECK_SUM_STATE = 0x03;

    private const int HEADER_VALUE = 0xBA;
    private const int PACKET_TYPE_ACTION_VALUE = 0x3A;

    private static byte actionNumber;
    private const byte resetActionNumberValue = 0xFF;
    private static byte checkSum;
    private const byte resetCheckSumValue = 0x00;

    void Start()
    {
        stateMachineState = HEADER_STATE;
        checkSum = resetCheckSumValue;
        actionNumber = resetActionNumberValue;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        HandleInputData();
    }



    //------------------------------------------------------------------------
    //
    //------------------------------------------------------------------------


    private void HandleInputData()
    {
        if(_serial.IsOpen)
        {
            if(_serial.BytesToRead > 0)
            {
                byte[] receivedArray = new byte[_serial.BytesToRead];
                _serial.Read(receivedArray, 0, _serial.BytesToRead);
                ParseInputData(receivedArray);
            }
            
        }

    }

    private void ParseInputData(byte[] receivedArray)
    {
        for(int i = 0; i < receivedArray.Length; i++)
        {
            FeedByteToStateMachine(receivedArray[i]);
        }
    }

    private void FeedByteToStateMachine(byte nextReceivedByte)
    {
        switch(stateMachineState)
        {
            case HEADER_STATE:
                if(nextReceivedByte == HEADER_VALUE)
                {
                    checkSum = 0x00;
                    checkSum += nextReceivedByte;
                    stateMachineState = PACKET_TYPE_STATE;
                }
            break;
            case PACKET_TYPE_STATE:
                if (nextReceivedByte == PACKET_TYPE_ACTION_VALUE)
                {
                    checkSum += nextReceivedByte;
                    stateMachineState = ACTION_NUMBER_STATE;
                }
                break;

            case ACTION_NUMBER_STATE:
                checkSum += nextReceivedByte;
                actionNumber = nextReceivedByte;
                stateMachineState = CHECK_SUM_STATE;
                break;
            case CHECK_SUM_STATE:
                if(checkSum == nextReceivedByte)
                {
                    //invoke event
                    Debug.Log("Received command packet and event invoked");
                    NotifyActionCommandReceived?.Invoke(this, new SerialPortEventArgs(actionNumber));
                    actionNumber = resetActionNumberValue;
                }
                stateMachineState = HEADER_STATE;
            break;
            default:
                stateMachineState = HEADER_STATE;
            break;

        }
    }

    //------------------------------------------------------------------------
    //
    //------------------------------------------------------------------------


    public static byte[] RawSerialize(object anything)
    {
        int rawsize = Marshal.SizeOf(anything);
        byte[] rawdata = new byte[rawsize];
        GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
        Marshal.StructureToPtr(anything, handle.AddrOfPinnedObject(), false);
        handle.Free();
        return rawdata;
    }

    //------------------------------------------------------------------------

    private byte[] FormPacketForRespondIntoByteArray(ushort SensorsState)
    {
        byte sum = 0x00;
        byte[] respondPacketInBytes;

        RespondSensorDataPacket respondPacket = new RespondSensorDataPacket((byte)0xBA, (byte)0x1A, BitConverter.GetBytes(SensorsState));
        respondPacketInBytes = RawSerialize(respondPacket);
        for (int i = 0; i < respondPacketInBytes.Length - 1; i++)
        {
            sum += respondPacketInBytes[i];
        }
        respondPacketInBytes[respondPacketInBytes.Length - 1] = sum;
        return respondPacketInBytes;
    }

    
    //------------------------------------------------------------------------
    //
    //------------------------------------------------------------------------

    public void sendRespondFromRobot(ushort SensorsState)
    {
        byte[] respondPacketInBytes;
        int count;
        respondPacketInBytes = FormPacketForRespondIntoByteArray(SensorsState);
        //send to serial port
        _serial.Write(respondPacketInBytes, 0, respondPacketInBytes.Length);
    }


    //------------------------------------------------------------------------
    //general com port functions
    //------------------------------------------------------------------------

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
        _serial.Handshake = Handshake.None;
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
                portNames = System.IO.Ports.SerialPort.GetPortNames();

                break;

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
