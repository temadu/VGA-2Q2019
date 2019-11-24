using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameWorld : MonoBehaviour {
    public static long Freim = 0L;
    public bool Server;
    private GameObject[] _cubes;
    private Dictionary<int, GameObject> _cubesById;
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    public GameObject playerPrefab;
    public GameObject otherPlayerPrefab;
    private int counter = 0;
    private int counterTest = 0;
    private Bafer bafer;
    private Boolean apdeitPoss
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
            bafer = new Bafer();
            // data de login? ip? name?
            _pp.SubscribeToTopic(Pucket.Logined, (message, order) => {
                Debug.Log("LOGINNEEEEDDDD");
                Freim = order;
                int id = int.Parse(message);
                GameObject newPlayer = Instantiate(playerPrefab);
                newPlayer.name = "CualquierCosa";
                newPlayer.GetComponent<CubeClass>().Id = id;
                newPlayer.AddComponent<ImputChandre>();
                newPlayer.transform.position = new Vector3(0,0,0);
                _cubesById[id] = newPlayer;
            });

            _pp.SubscribeToTopic(Pucket.Snapshot, (message, order) => {
                // agregar al buffer/interpolar el buffer en el update
                bafer.add(order, message);
                
                // guardame en el buffer
                // string[] cubes = message.Split('\n');
                // // dame el sig pack dle buffer

                // // for tiene q hacerse en un update
                // foreach (string c in cubes) {
                //     if (c.Length == 0) continue;
                //     string[] pos = c.Split(';');
                //     if (_cubesById.ContainsKey(int.Parse(pos[0]))) {
                //         _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3(float.Parse(pos[1]),
                //             float.Parse(pos[2]), float.Parse(pos[3]));
                //     }
                // }
            });
            
            _pp.SubscribeToTopic(Pucket.UpdatePlayersInfo, (message, order) => {
                string[] split = message.Split('-');
                string[] ids = split[0].Split(';');
                string[] names = split[1].Split(';');
                for (int i = 0; i < ids.Length; i++) {
                    if(ids.Length == 0) continue;
                    GameObject cube = null;
                    if (_cubesById.ContainsKey(int.Parse(ids[i])))
                    {
                        cube = _cubesById[int.Parse(ids[i])];
                    }
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
            _pp.SubscribeToTopic(Pucket.Login, (message, order) => {
                print("NEW USER");
                print(message);
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
                    ids += key.ToString() + ';';
                    names += _cubesById[key].GetComponent<CubeClass>().Name + ';';
                }

                ids = ids.Remove(ids.Length - 1);
                names = names.Remove(names.Length - 1);
                _pp.CreatePukcet(counter.ToString(), Pucket.Logined);
                _pp.CreatePukcet((ids + '-' + names), Pucket.UpdatePlayersInfo);
            });
        }
    }

    public void Update() {
        // print(Time.deltaTime);
        string positions = "";
        _pp.Update();
        if (Server && Freim % 100 == 0) {
            foreach (var cube in _cubes) {
                Vector3 pos = cube.gameObject.transform.position;
                Quaternion rot = cube.gameObject.transform.rotation;
                positions += cube.GetComponent<CubeClass>().Id + ";" + pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.w +
                             ";" + rot.x + ";" + rot.y + ";" + rot.z + "\n";
            }
            print(positions);
            _pp.CreatePukcet(positions,Pucket.Snapshot);
        } else {
            Bafer.Pocket lastPacket = bafer.peakEnd();
            if(lastPacket != null && lastPacket.horder > Freim + 60) {
                apdeitPoss = true;
                Bafer.Pocket first = bafer.peak();
                long interPolationSteps = first.horder - Freim;
                // interpolar basado en el buffer
                string[] cubes = first.possisions.Split('\n');
                // for tiene q hacerse en un update
                foreach (string c in cubes) {
                    if (c.Length == 0) continue;
                    string[] pos = c.Split(';');
                    if (_cubesById.ContainsKey(int.Parse(pos[0]))) {
                        //lerp
                        _cubesById[int.Parse(pos[0])].gameObject.transform.position = 
                            Vector3.Lerp(_cubesById[int.Parse(pos[0])].gameObject.transform.position, 
                            new Vector3(float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3])), 
                            1f/interPolationSteps);
                        _cubesById[int.Parse(pos[0])].gameObject.transform.rotation = 
                            Quaternion.Lerp(_cubesById[int.Parse(pos[0])].gameObject.transform.rotation, 
                            new Quaternion(float.Parse(pos[4]), float.Parse(pos[5]), float.Parse(pos[6]), float.Parse(pos[7])), 
                            1f/interPolationSteps);
                    }
                }
                if(interPolationSteps == 1){
                    bafer.removeFirst();
                }
            }
            // predecir a los demas
        }

        Freim++;        
    }
}
