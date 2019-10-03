//using System.Collections;
//using System.Collections.Generic;
//using System.Security.Policy;
//using UnityEngine;
//using UnityEngine.UI;
//
//public class Connecter : MonoBehaviour {
//
//	private UdpConnection connection;
//	public Text text;
//	public InputField inputField;
//	void Start()
//	{
//		string sendIp = "10.17.68.204";
//		int sendPort = 11000;
//		int receivePort = 11000;
// 
//		connection = new UdpConnection();
//		connection.StartConnection(sendIp, sendPort, receivePort);
//	}
// 
//	void Update()
//	{
//		foreach (var message in connection.getMessages()) Debug.Log(message);
//		if (Input.GetKeyDown(KeyCode.Return)) {
//			connection.Send(inputField.text);
//			inputField.text = "";
//		}
//	}
//
//	public void SendMessageTruch() {
//		connection.Send(inputField.text);
//	}
// 
//	void OnDestroy() {
//		connection.Stop();
//	}
//
//}
