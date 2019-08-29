using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class PacketPrusecor : MonoBehaviour {
//	private List<Strim> _unreliabables; //para cada user
//	private List<Strim> _reliablesSlow;
//	private List<Strim> _relaibelsFast;
//	lista de connections con cada user	

	private Strim _unrelisbasle;
	private Strim _reliabelSlow;
	private Strim _relasibFast;
	private UdpConnection _connection;

	// Use this for initialization
	private void Start () {
//		_unreliabables= new List<Strim>();
//		_relaibelsFast = new List<Strim>();
//		_reliablesSlow = new List<Strim>();
		
		_unrelisbasle = new Strim(false);
		_relasibFast = new Strim(true);
		_reliabelSlow = new Strim(true);
		
		
		string sendIp = "10.17.67.180";
		int sendPort = 11000;
		int receivePort = 11000;
 
		_connection = new UdpConnection();
		_connection.StartConnection(sendIp, sendPort, receivePort);
	}
	
	// Update is called once per frame
	private void Update () {
		foreach (var message in _connection.getMessages()) {
			Debug.Log(message);
			string[] splited = message.Split(',');
			if (int.Parse(splited[0]) == 0) {
				_unrelisbasle.ReceivePacket(new Strim.Pucket()); //ver como verga hacer un pucket			
			} else if (int.Parse(splited[0]) == 1) {
				
			} else if (int.Parse(splited[0]) == 2)
	
			{
				
			}
			
		}
		
		
	}

	private class Pucket {
		public bool Ack;
		public int Topic;
		public int Order;
		public string Data;

		Pucket(int topic, int order, string data, bool ack) {
			Ack = ack;
			Topic = topic;
			Order = order;
			Data = data;
		}
	}
}

