using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
 
public class UdpConnection
{
    private UdpClient udpClient;
 
    private readonly Queue<string> incomingQueue = new Queue<string>();
    Thread receiveThread;
    private bool threadRunning = false;
    private string senderIp;
    private int senderPort;


    public bool randomLag = false;
    public float msLag = 0;
    public float packetDropChance = 0;
    private int currentBitCount = 0 ;
    
 
    public void StartConnection(string sendIp, int sendPort, int receivePort)
    {
        try { udpClient = new UdpClient(receivePort); }
        catch (Exception e)
        {
            Debug.Log("Failed to listen for UDP at port " + receivePort + ": " + e.Message);
            return;
        }
        Debug.Log("Created receiving client at ip  and port " + receivePort);
        senderIp = sendIp;
        senderPort = sendPort;
 
        Debug.Log("Set sendee at ip " + sendIp + " and port " + sendPort);
 
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
 
    public void Send(string message)
    {
        // Debug.Log(String.Format("Send msg to ip:{0} port:{1} msg:{2}",senderIp,senderPort,message));
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
        Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
        udpClient.Send(sendBytes, sendBytes.Length, serverEndpoint);
    }
    
    public void SendSlim(Pucket pucket)
    {
        //LOS BITS LEERLOS DE IZQ A DERECHA
        int[] arr = new int[Encoding.UTF8.GetBytes(pucket.Data).Length+2]; //seria como el max size ponele
        
        
        // del bit 1 al 4 seria el topic
        int x = pucket.Topic;
        string s = Convert.ToString(x, 2); //Convert to binary in a string
        int[] bits= s.PadLeft(8, '0') // Add 0's from left
            .Select(c => int.Parse(c.ToString())) // convert each char to int
            .ToArray(); // Convert IEnumerable from select to Array

        arr[0] = bits[31];
        arr[1] = bits[30];
        arr[2] = bits[29];
        arr[3] = bits[28];
        
        //el 5 bit es 1 o 0 para avisar ack
        arr[4] = pucket.Ack ? 1 : 0;
        
        // del bit 6 al 32 es el order que puede aumentar muchisimo,,, contemplar de alguna manera el reset
        long y = pucket.Topic;
        string s1 = Convert.ToString(y, 2); //Convert to binary in a string
        int[] bits1= s1.PadLeft(8, '0') // Add 0's from left
            .Select(c => int.Parse(c.ToString())) // convert each char to int
            .ToArray(); // Convert IEnumerable from select to Array
        for (int idx = 4+31; idx < 32; idx++) {
            arr[idx] = bits1[35-idx];
        }
        
        // el segundo byte (32 bits) es para decir cuantos bytes de texto
//        int numBytesText = Encoding.UTF8.GetBytes(pucket.Data).Length;
        long z = pucket.Topic;
        string s2 = Convert.ToString(z, 2); //Convert to binary in a string
        int[] bits2= s2.PadLeft(8, '0') // Add 0's from left
            .Select(c => int.Parse(c.ToString())) // convert each char to int
            .ToArray(); // Convert IEnumerable from select to Array
        for (int idx = 62; idx < 93; idx++) {
            arr[idx] = bits2[93-idx];
        }
        
        // 3cer byte en adelante es texto
//        Buffer.BlockCopy(Encoding.UTF8.GetBytes(pucket.Data), 0, bytes, 0, byte.Length);

        // Debug.Log(String.Format("Send msg to ip:{0} port:{1} msg:{2}",senderIp,senderPort,message));
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
        Byte[] sendBytes = Encoding.UTF8.GetBytes(pucket.Data);
        udpClient.Send(sendBytes, sendBytes.Length, serverEndpoint);
    }

    public void Stop()
    {
        threadRunning = false;
        receiveThread.Abort();
        udpClient.Close();
    }
}