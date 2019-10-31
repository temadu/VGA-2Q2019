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
            _pp.CreatePukcet("neim", Pucket.Connection);
            
            _pp.SubscribeToTopic(Pucket.Snapshot, message =>
            {
                string[] cubes = message.Split('\n');
                foreach (string c in cubes)
                {
                    if (c.Length == 0) continue;
                    string[] pos = c.Split(';');
                    if (_cubesById.ContainsKey(int.Parse(pos[0])))
                    {
                        _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3(float.Parse(pos[1]),
                            float.Parse(pos[2]), float.Parse(pos[3]));
                    }
                }
            });
            
            _pp.SubscribeToTopic(Pucket.Connected, message => {
                GameObject newPlayer = Instantiate(playerPrefab);
                string[] split = message.Split(';');
                int id = int.Parse(split[0]);
                newPlayer.GetComponent<CubeClass>().Id = id;
                _cubesById[id] = newPlayer;
                string[] ids = split[1].Split('-');
                foreach(string pId in ids) {
                    if (!pId.Equals(id.ToString())) {
                        GameObject newCube = Instantiate(otherPlayerPrefab);
                        newCube.GetComponent<CubeClass>().Id = int.Parse(pId);
                        _cubesById[int.Parse(pId)] = newCube;
                    }
                }
            });
        } else {
            _pp.SubscribeToTopic(Pucket.Connection, message => {
                print(message);
                GameObject newPlayer = Instantiate(playerPrefab);
                newPlayer.AddComponent<Rigidbody>();
                newPlayer.AddComponent<MoveVehicle>();
                newPlayer.GetComponent<CubeClass>().Id = ++counter;
                Array.Resize(ref _cubes, _cubes.Length + 1);
                _cubes[_cubes.GetUpperBound(0)] = newPlayer;
                _cubesById[counter] = newPlayer;
                string data = counter.ToString() + ';';
                print(_cubesById.Keys);
                foreach(int key in _cubesById.Keys) {
                    data += key.ToString() + '-';
                }
                data = data.Remove(data.Length - 1);
                print(data);
                _pp.CreatePukcet(data, Pucket.Connected);
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
