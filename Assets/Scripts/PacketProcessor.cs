using UnityEngine;
using System;

public sealed class PacketProcessor {
	
	private static readonly PacketProcessor INSTANCE = new PacketProcessor();
//	private List<Stream> _unreliabables; //para cada user
//	private List<Stream> _reliablesSlow;
//	private List<Stream> _relaibelsFast;
//	lista de connections con cada user	

	private Stream _unreliable;
	private Stream _reliableSlow;
	private Stream _reliableFast;
	public UdpConnection _connection;
	// Use this for initialization
	private PacketProcessor() {
		string serverIp = "10.17.65.68";
		int sendPort = 11000;
		int receivePort = 11000;
 
		_connection = new UdpConnection();
		_connection.StartConnection(serverIp, sendPort, receivePort);
	}

	public static PacketProcessor Instance {
		get { return INSTANCE; }
	}
	
	public void initStreams(bool client){
		_unreliable = new Stream(false, client);
		_reliableFast = new Stream(true, client);
		_reliableSlow = new Stream(true, client);
		
	}
	
	// Update is called once per frame
	
	// cada message va a ser topic, ack, order, data
	public void Update () {
		foreach (var message in _connection.getMessages()) {
			// Debug.Log(message);
			string[] splited = message.Split(',');
			switch (int.Parse(splited[0])) {
				case Packet.Snapshot:
					_unreliable.ReceivePacket(new Packet(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
				case Packet.Input:
					// Debug.Log(splited[3]);
					_reliableFast.ReceivePacket(new Packet(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
				case Packet.Login:
				case Packet.Logined:
				case Packet.UpdatePlayersInfo:
					_reliableSlow.ReceivePacket(new Packet(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
			}
		}
	}

	public void CreatePacket(string data, int topic, int id=-1, bool ack = false) {
		Packet p = null;
		switch (topic) {
			case Packet.Snapshot:
				p =_unreliable.CreatePacket(data, topic, ack);
				_connection.SendAll(p.ToString());
				break;
			case Packet.Input:
				p =_reliableFast.CreatePacket(data, topic, ack);
				_connection.Send(p.ToString(), id);		
				break;
			case Packet.UpdatePlayersInfo:
				p =_reliableSlow.CreatePacket(data, topic, ack);
				_connection.SendAll(p.ToString());
				break;	
			case Packet.Login:
			case Packet.Logined:
				Debug.Log(topic);
				if(topic == Packet.Login){
					Debug.Log("Send Login");
				}
				p =_reliableSlow.CreatePacket(data, topic, ack);
				_connection.Send(p.ToString(), id);				
				break;
		}
	}

	public void SubscribeToTopic(int topic, Action<string, long> obs) {
		switch (topic) {
			case Packet.Snapshot:
				_unreliable.addObserver(obs, topic);
				break;
			case Packet.Input:
				_reliableFast.addObserver(obs, topic);
				break;
			case Packet.UpdatePlayersInfo:
			case Packet.Login:
			case Packet.Logined:
				_reliableSlow.addObserver(obs, topic);
				break;
		}
	}

	public void SubscribeToAckTopic(int topic, Action<long, long> obs) {
		switch (topic) {
			case Packet.Snapshot:
			  break;
			case Packet.Input:
				_reliableFast.addAckObserver(obs, topic);
				break;
			case Packet.UpdatePlayersInfo:
			case Packet.Login:
			case Packet.Logined:
				_reliableSlow.addAckObserver(obs, topic);
				break;
		}
	}

	public void AddIp(int id, string ip){
		_connection.addIp(id, ip);
	}
}

