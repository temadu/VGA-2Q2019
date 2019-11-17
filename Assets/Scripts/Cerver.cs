// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Cerver : MonoBehaviour {
//     public bool Server;

//     private GameObject[] _cubes;
//     private Dictionary<int, GameObject> _cubesById;

//     private UdpConnection connection;
//     public GameObject playerPrefab;
//     private PacketPrusecor _pp = PacketPrusecor.Instance;	
//     private int counter;
// //	public Text text;
// //	public InputField inputField;

//     // Use this for initialization
//     void Start () {
//         _cubes = GameObject.FindGameObjectsWithTag("Cubo");
//     _cubesById = new Dictionary<int, GameObject>();
//         counter = 0;
            
//         foreach (GameObject cube in _cubes) {
//             if (Server) {
//                 cube.AddComponent<Rigidbody>();
//             }
//             cube.GetComponent<CubeClass>().Id = counter;
//             _cubesById[counter] = cube;
//             counter++;
//             print(counter);
            
//         }
//         string sendIp = "10.17.64.90";
//         int sendPort = 11000;
//         int receivePort = 11000;
 
//         connection = new UdpConnection();
//         connection.StartConnection(sendIp, sendPort, receivePort);
//         if(!Server) {
//             // data de login? ip? name?
//             _pp.CreatePukcet("neim", Pucket.Connection);
//             _pp.SubscribeToTopic(Pucket.UpdatePlayersInfo, message => {
//                 GameObject newPlayer = Instantiate(playerPrefab);
//                 int id = int.Parse(message);
//                 newPlayer.GetComponent<CubeClass>().Id = id;
//                 Array.Resize(ref _cubes, _cubes.Length + 1);
//                 _cubes[_cubes.GetUpperBound(0)] = newPlayer;	
//                 _cubesById[counter] = newPlayer;
//             });
//         } else {
//             _pp.SubscribeToTopic(Pucket.Connection, message => {
//                 print(message);
//                 GameObject newPlayer = Instantiate(playerPrefab);
//                 newPlayer.AddComponent<Rigidbody>();
//                 newPlayer.GetComponent<CubeClass>().Id = counter++;
//                 Array.Resize(ref _cubes, _cubes.Length + 1);
//                 _cubes[_cubes.GetUpperBound(0)] = newPlayer;
//                 _cubesById[counter] = newPlayer;
//                 _pp.CreatePukcet(counter.ToString(), Pucket.UpdatePlayersInfo);
//             });
//         }
//     }


//     void Update()
//     {		
//         string positions = "";

// //		if (Input.GetKeyDown(KeyCode.Return)) {
//         if (Server) {
//             foreach (var cube in _cubes) {
//                 Vector3 pos = cube.gameObject.transform.position;
//                 Quaternion rot = cube.gameObject.transform.rotation;
//                 positions += cube.GetComponent<CubeClass>().Id + "," + pos.x + "," + pos.y + "," + pos.z + "," + rot.w + "," + rot.x + "," + rot.y + "," + rot.z + "\n";
//             }
//             connection.Send(positions);
//         } else {
//             foreach (var message in connection.getMessages()) {
//                 Debug.Log(message);
//                 string[] cubes = message.Split('\n');
//                 foreach (string c in cubes) {
//                     string[] newPos = c.Split(',');
//                     _cubesById[int.Parse(newPos[0])].gameObject.transform.position = new Vector3 (float.Parse(newPos[1]), float.Parse(newPos[2]), float.Parse(newPos[3]));
//                 }
//             } 	
//         }

//         //		}
//     }

//     void OnDestroy() {
//         connection.Stop();
//     }
// }
