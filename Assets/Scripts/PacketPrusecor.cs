using UnityEngine;
using System;

public sealed class PacketPrusecor {
	
	private static readonly PacketPrusecor INSTANCE = new PacketPrusecor();
//	private List<Strim> _unreliabables; //para cada user
//	private List<Strim> _reliablesSlow;
//	private List<Strim> _relaibelsFast;
//	lista de connections con cada user	

	private Strim _unrelisbasle;
	private Strim _reliabelSlow;
	private Strim _relasibFast;
	public UdpConnection _connection;
	// Use this for initialization
	private PacketPrusecor() {
		string serverIp = "192.168.1.54";
		int sendPort = 11000;
		int receivePort = 11000;
 
		_connection = new UdpConnection();
		_connection.StartConnection(serverIp, sendPort, receivePort);
	}

	public static PacketPrusecor Instance {
		get { return INSTANCE; }
	}
	
	public void initStrims(bool client){
		_unrelisbasle = new Strim(false, client);
		_relasibFast = new Strim(true, client);
		_reliabelSlow = new Strim(true, client);
		
	}
	
	// Update is called once per frame
	
	// cada message va a ser topic, ack, order, data
	public void Update () {
		foreach (var message in _connection.getMessages()) {
			// Debug.Log(message);
			string[] splited = message.Split(',');
			switch (int.Parse(splited[0])) {
				case Pucket.Snapshot:
					_unrelisbasle.ReceivePacket(new Pucket(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
				case Pucket.Input:
					// Debug.Log(splited[3]);
					_relasibFast.ReceivePacket(new Pucket(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
				case Pucket.Login:
				case Pucket.Logined:
				case Pucket.UpdatePlayersInfo:
					_reliabelSlow.ReceivePacket(new Pucket(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
			}
		}
	}

	public void CreatePukcet(string data, int topic, int id=-1, bool ack = false) {
		Pucket p = null;
		switch (topic) {
			case Pucket.Snapshot:
				p =_unrelisbasle.CreatePacket(data, topic, ack);
				_connection.SendAll(p.ToString());
				break;
			case Pucket.Input:
				p =_relasibFast.CreatePacket(data, topic, ack);
				_connection.Send(p.ToString(), id);		
				break;
			case Pucket.UpdatePlayersInfo:
				p =_reliabelSlow.CreatePacket(data, topic, ack);
				_connection.SendAll(p.ToString());
				break;	
			case Pucket.Login:
			case Pucket.Logined:
				Debug.Log(topic);
				p =_reliabelSlow.CreatePacket(data, topic, ack);
				_connection.Send(p.ToString(), id);				
				break;
		}
	}

	public void SubscribeToTopic(int topic, Action<string, long> obs) {
		switch (topic) {
			case Pucket.Snapshot:
				_unrelisbasle.addObserver(obs, topic);
				break;
			case Pucket.Input:
				_relasibFast.addObserver(obs, topic);
				break;
			case Pucket.UpdatePlayersInfo:
			case Pucket.Login:
			case Pucket.Logined:
				_reliabelSlow.addObserver(obs, topic);
				break;
		}
	}

	public void SubscribeToAckTopic(int topic, Action<long, long> obs) {
		switch (topic) {
			case Pucket.Snapshot:
			  break;
			case Pucket.Input:
				_relasibFast.addAckObserver(obs, topic);
				break;
			case Pucket.UpdatePlayersInfo:
			case Pucket.Login:
			case Pucket.Logined:
				_reliabelSlow.addAckObserver(obs, topic);
				break;
		}
	}

	public void AddIp(int id, string ip){
		_connection.addIp(id, ip);
	}
}

