using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = System.Random;

public class UdpConnection
{
    private UdpClient udpClient;
    
    const int SEND_DELAY = 5000;
    const double DROP_PROBABILITY = 0.7;
 
    private readonly Queue<string> incomingQueue = new Queue<string>();
    Thread receiveThread;
    private bool threadRunning = false;
    private string serverIp;
    private int senderPort;
    private Dictionary<int, string> ips;

    public bool randomLag = false;
    public float msLag = 0;
    public float packetDropChance = 0;
    

    public void addIp(int id, string ip){
        ips.Add(id, ip);
    }
 
    public void StartConnection(string serverIp, int sendPort, int receivePort)
    {
        ips = new Dictionary<int, string>();
        try { udpClient = new UdpClient(receivePort); }
        catch (Exception e)
        {
            Debug.Log("Failed to listen for UDP at port " + receivePort + ": " + e.Message);
            return;
        }
        Debug.Log("Created receiving client at ip  and port " + receivePort);
        this.serverIp = serverIp;
        senderPort = sendPort;
 
        Debug.Log("Set sendee at ip " + serverIp + " and port " + sendPort);

        StartReceiveThread();
    }
 
    private void StartReceiveThread()
    {
        receiveThread = new Thread(() => ListenForMessages(udpClient));
        receiveThread.IsBackground = true;
        threadRunning = true;
        receiveThread.Start();
    }
 
    private void ListenForMessages(UdpClient client)
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
 
        while (threadRunning)
        {
            try
            {
                Byte[] receiveBytes = client.Receive(ref remoteIpEndPoint); // Blocks until a message returns on this socket from a remote host.
                string returnData = Encoding.UTF8.GetString(receiveBytes);
 
                lock (incomingQueue)
                {
                    incomingQueue.Enqueue(returnData);
                }
            }
            catch (SocketException e)
            {
                // 10004 thrown when socket is closed
                if (e.ErrorCode != 10004) Debug.Log("Socket exception while receiving data from udp client: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.Log("Error receiving data from udp client: " + e.Message);
            }
            Thread.Sleep(1);
        }
    }
 
    public string[] getMessages()
    {
        string[] pendingMessages = new string[0];
        lock (incomingQueue)
        {
            pendingMessages = new string[incomingQueue.Count];
            int i = 0;
            while (incomingQueue.Count != 0)
            {
                pendingMessages[i] = incomingQueue.Dequeue();
                i++;
            }
        }
 
        return pendingMessages;
    }
 
    public void Send(string message, int id=-1) {
        IPEndPoint endpoint;
        if(id == -1){
            endpoint = new IPEndPoint(IPAddress.Parse(this.serverIp), senderPort);  
        } else {
            endpoint = new IPEndPoint(IPAddress.Parse(ips[id]), senderPort);
        }
        (new Thread(() => SendMessageThreaded(udpClient, message, endpoint))).Start();
    }

    public void SendAll(string message) {
        foreach (string ip in ips.Values) {
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ip), senderPort);
            (new Thread(() => SendMessageThreaded(udpClient, message, serverEndpoint))).Start();
        }
    }
    
    private void SendMessageThreaded(UdpClient client, string message, IPEndPoint endpoint) {
        if(SEND_DELAY>0) Thread.Sleep(SEND_DELAY);
        Random random = new Random();
        if (random.NextDouble() > DROP_PROBABILITY) return;
        Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
        client.Send(sendBytes, sendBytes.Length, endpoint);
    }
 
    public void Stop()
    {
        threadRunning = false;
        receiveThread.Abort();
        udpClient.Close();
    }
}