using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerver : MonoBehaviour {
	public bool Server;

	private GameObject[] _cubes;
	private GameObject[] _cubesById;

	private UdpConnection connection;
	public GameObject playerPrefab;
    private PacketPrusecor _pp = PacketPrusecor.Instance;	
	private int counter;
//	public Text text;
//	public InputField inputField;

	// Use this for initialization
	void Start () {
		_cubes = GameObject.FindGameObjectsWithTag("Cubo");
	    _cubesById = new GameObject[10];
		counter = 0;
			
		foreach (GameObject cube in _cubes) {
			if (Server) {
				cube.AddComponent<Rigidbody>();
			}
			cube.GetComponent<CubeClass>().Id = counter;
			_cubesById[counter] = cube;
			counter++;
			print(counter);
			
		}
		string sendIp = "10.17.66.51";
		int sendPort = 11000;
		int receivePort = 11000;
 
		connection = new UdpConnection();
		connection.StartConnection(sendIp, sendPort, receivePort);
		if(!Server) {
			// data de login? ip? name?
			_pp.CreatePukcet("neim", Pucket.Connection);
			_pp.SubscribeToTopic(Pucket.Connected, message => {
				GameObject newPlayer = Instantiate(playerPrefab);
				int id = int.Parse(message);
				newPlayer.GetComponent<CubeClass>().Id = id;				
				_cubes.ToList().Add(newPlayer).ToArray();
				_cubesById[counter] = newPlayer;
			});
		} else {
			_pp.SubscribeToTopic(Pucket.Connection, message => {
				GameObject newPlayer = Instantiate(playerPrefab);
				newPlayer.AddComponent<Rigidbody>();
				newPlayer.GetComponent<CubeClass>().Id = counter++;
				_cubes.ToList().Add(newPlayer).ToArray();				
				_cubesById[counter] = newPlayer;
				_pp.CreatePukcet(counter.ToString(), Pucket.Connected);
			});
		}
	}


	void Update()
	{		
		string positions = "";

//		if (Input.GetKeyDown(KeyCode.Return)) {
		if (Server) {
			foreach (var cube in _cubes) {
				Vector3 pos = cube.gameObject.transform.position;
				Quaternion rot = cube.gameObject.transform.rotation;
				positions += cube.GetComponent<CubeClass>().Id + "," + pos.x + "," + pos.y + "," + pos.z + "," + rot.w + "," + rot.x + "," + rot.y + "," + rot.z + "\n";
			}
			connection.Send(positions);
		} else {
			foreach (var message in connection.getMessages()) {
				Debug.Log(message);
				string[] cubes = message.Split('\n');
				foreach (string c in cubes) {
					string[] newPos = c.Split(',');
					_cubesById[int.Parse(newPos[0])].gameObject.transform.position = new Vector3 (float.Parse(newPos[1]), float.Parse(newPos[2]), float.Parse(newPos[3]));
				}
			} 	
		}

		//		}
	}

	void OnDestroy() {
		connection.Stop();
	}
}
