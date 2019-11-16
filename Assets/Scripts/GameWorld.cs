using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameWorld : MonoBehaviour {
    public bool Server;
    private GameObject[] _cubes;
    private Dictionary<int, GameObject> _cubesById;
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    public GameObject playerPrefab;
    public GameObject otherPlayerPrefab;
    private int counter = 0;
    private int counterTest = 0;
    private void Start() {
        _cubesById = new Dictionary<int, GameObject>();
        if(Server) {
            _cubes = GameObject.FindGameObjectsWithTag("Cubo");
            _cubesById = new Dictionary<int, GameObject>();
            counter = 0;

            foreach (GameObject cube in _cubes)
            {
                cube.AddComponent<Rigidbody>();
                _cubesById[cube.GetComponent<CubeClass>().Id] = cube;
                // cube.GetComponent<CubeClass>().Id = counter;
                counter++;
                print(counter);
                
            }
        }

        if(!Server) {
            // data de login? ip? name?
            _pp.CreatePukcet("neim", Pucket.Login);

            _pp.SubscribeToTopic(Pucket.Logined, message => {
                int id = int.Parse(message);
                GameObject newPlayer = Instantiate(playerPrefab);
                newPlayer.GetComponent<CubeClass>().Id = id;
                newPlayer.AddComponent<ImputChandre>();
                newPlayer.transform.position = new Vector3(0,0,0);
                _cubesById[id] = newPlayer;
            });

            _pp.SubscribeToTopic(Pucket.Snapshot, message => {
                string[] cubes = message.Split('\n');
                foreach (string c in cubes)
                {
                    if (c.Length == 0) continue;
                    string[] pos = c.Split(';');
                    if (_cubesById.ContainsKey(int.Parse(pos[0]))) {
                        _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3(float.Parse(pos[1]),
                            float.Parse(pos[2]), float.Parse(pos[3]));
                    }
                }
            });
            
            _pp.SubscribeToTopic(Pucket.Connected, message => {
                string[] split = message.Split('-');
                string[] ids = split[0].Split(',');
                string[] names = split[1].Split(',');
                for (int i = 0; i < ids.Length; i++) {
                    GameObject cube = _cubesById[int.Parse(ids[i])];
                    if (cube == null) {
                        cube = Instantiate(otherPlayerPrefab);
                        cube.transform.position = new Vector3(0,0,0);
                    } 
                    cube.GetComponent<CubeClass>().Id = int.Parse(ids[i]);
                    cube.GetComponent<CubeClass>().Name = names[i];
                    _cubesById[int.Parse(ids[i])] = cube;
                }
            });
        } else {
            _pp.SubscribeToTopic(Pucket.Login, message => {
                GameObject newPlayer = Instantiate(playerPrefab);
                newPlayer.AddComponent<Rigidbody>();
                newPlayer.AddComponent<MoveVehicle>();
                newPlayer.GetComponent<CubeClass>().Id = ++counter;
                newPlayer.GetComponent<CubeClass>().Name = message;
                Array.Resize(ref _cubes, _cubes.Length + 1);
                _cubes[_cubes.GetUpperBound(0)] = newPlayer;
                _cubesById[counter] = newPlayer;
                string ids = "";
                string names = "";
                foreach(int key in _cubesById.Keys) {
                    ids += key.ToString() + ',';
                    names += _cubesById[key].name + ',';
                }

                ids = ids.Remove(ids.Length - 1);
                names = names.Remove(names.Length - 1);
                _pp.CreatePukcet(counter.ToString(), Pucket.Logined);
                _pp.CreatePukcet(ids + '-' + names, Pucket.Connected);
            });
        }
    }

    public void Update() {
        // print(Time.deltaTime);
        string positions = "";
        _pp.Update();
        if (Server && counterTest++ % 100 == 0) {
            foreach (var cube in _cubes) {
                Vector3 pos = cube.gameObject.transform.position;
                Quaternion rot = cube.gameObject.transform.rotation;
                positions += cube.GetComponent<CubeClass>().Id + ";" + pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.w +
                             ";" + rot.x + ";" + rot.y + ";" + rot.z + "\n";
            }
            print(positions);
            _pp.CreatePukcet(positions,Pucket.Snapshot);
        }

    }
}
