using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 5052;

    public bool printToConsole = false;
    public string data;

    private bool isRunning = false;

    void Start()
    {
        isRunning = true;
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        try
        {
            client = new UdpClient(port);
            client.Client.ReceiveTimeout = 1000; // 1 segundo

            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

            while (isRunning)
            {
                try
                {
                    byte[] dataByte = client.Receive(ref anyIP);
                    data = Encoding.UTF8.GetString(dataByte);
                }
                catch (SocketException)
                {
                    // Timeout normal, permite revisar isRunning
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("UDP detenido: " + e.Message);
        }
        finally
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
    }

    void OnDisable()
    {
        isRunning = false;
    }

    void Update()
    {
        if (printToConsole && !string.IsNullOrEmpty(data))
        {
            Debug.Log(data);
            data = null;
        }
    }
}