using DefaultNamespace;
using UnityEngine;

public sealed class PacketPrusecor {
	
	private static readonly PacketPrusecor INSTANCE = new PacketPrusecor();
//	private List<Strim> _unreliabables; //para cada user
//	private List<Strim> _reliablesSlow;
//	private List<Strim> _relaibelsFast;
//	lista de connections con cada user	

	private Strim _unrelisbasle;
	private Strim _reliabelSlow;
	private Strim _relasibFast;
	private UdpConnection _connection;

	// Use this for initialization
	private PacketPrusecor() {
//		_unreliabables= new List<Strim>();
//		_relaibelsFast = new List<Strim>();
//		_reliablesSlow = new List<Strim>();
		
		_unrelisbasle = new Strim(false);
		_relasibFast = new Strim(true);
		_reliabelSlow = new Strim(true);
		
		
		string sendIp = "10.17.66.51";
		int sendPort = 11000;
		int receivePort = 11000;
 
		_connection = new UdpConnection();
		_connection.StartConnection(sendIp, sendPort, receivePort);
	}

	public static PacketPrusecor Instance {
		get { return INSTANCE; }
	}
	
	
	
	// Update is called once per frame
	
	// cada message va a ser topic, ack, order, data
	// topic 0 unr
	// topic 1 unr
	// topic 2 rel
	// topic 3 rel
	// topic 4 rel-slow
	public void Update () {
		foreach (var message in _connection.getMessages()) {
			Debug.Log(message);
			string[] splited = message.Split(',');
			switch (int.Parse(splited[0])) {
				case Pucket.Snapshot:
					_unrelisbasle.ReceivePacket(new Pucket(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
				case Pucket.Input:
					_relasibFast.ReceivePacket(new Pucket(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
				case Pucket.Connection:
					// cada 1 segundo hacer if
					_reliabelSlow.ReceivePacket(new Pucket(int.Parse(splited[0]), long.Parse(splited[2]), splited[3],
						bool.Parse(splited[1])));
					break;
			}
		}
	}

	public void CreatePukcet(string data, int topic) {
		Pucket p = null;
		switch (topic) {
			case Pucket.Snapshot:
				p =_unrelisbasle.CreatePacket(data, topic);
				break;
			case Pucket.Input:
				p =_relasibFast.CreatePacket(data, topic);
				break;
			case Pucket.Connection:
				p =_reliabelSlow.CreatePacket(data, topic);
				break;
		}

		_connection.Send(p.ToString());
	}

	public void SubscribeToTopic(int topic, StrimObserver obs) {
		switch (topic) {
			case Pucket.Snapshot:
				_unrelisbasle.addObserver(obs);
				break;
			case Pucket.Input:
				_relasibFast.addObserver(obs);
				break;
			case Pucket.Connection:
				_reliabelSlow.addObserver(obs);
				break;
		}
	}

}

