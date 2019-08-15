using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerver : MonoBehaviour {
	public bool Server;

	private GameObject[] _cubes;
	private GameObject[] _cubesById;

	private UdpConnection connection;
//	public Text text;
//	public InputField inputField;

	// Use this for initialization
	void Start () {
		_cubes = GameObject.FindGameObjectsWithTag("Cubo");
		if (Server) {
			int counter = 0;
			foreach (GameObject cube in _cubes) {
				cube.AddComponent<Rigidbody>();
				cube.GetComponent<CubeClass>().Id = counter;
				_cubesById[counter] = cube;
				counter++;
				print(counter);
			}
		}
		string sendIp = "127.0.0.1";
		int sendPort = 11000;
		int receivePort = 11000;
 
		connection = new UdpConnection();
		connection.StartConnection(sendIp, sendPort, receivePort);
	}
 
	void Update()
	{		
		string positions = "";

//		if (Input.GetKeyDown(KeyCode.Return)) {
		if (Server) {
			foreach (var cube in _cubes) {
				Vector3 pos = cube.gameObject.transform.position;
				positions += cube.GetComponent<CubeClass>().Id + "," + pos.x + "," + pos.y + "," + pos.z + "\n";
			}

			connection.Send(positions);
		}
		else {
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
