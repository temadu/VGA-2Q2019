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
    private int counter = 0;

    private void Start() {
        _pp.SubscribeToTopic(Pucket.Snapshot, HandleUpdate);
        _cubes = GameObject.FindGameObjectsWithTag("Cubo");
        _cubesById = new Dictionary<int, GameObject>();
        counter = 0;
			
        foreach (GameObject cube in _cubes) {
            if (Server) {
                cube.AddComponent<Rigidbody>();
                cube.AddComponent<MoveVehicle>();
            }
            // cube.GetComponent<CubeClass>().Id = counter;
            _cubesById[cube.GetComponent<CubeClass>().Id] = cube;
            counter++;
            print(counter);
        }

        if(!Server) {
            // data de login? ip? name?
            _pp.CreatePukcet("neim", Pucket.Connection);
            _pp.SubscribeToTopic(Pucket.Connected, message => {
                GameObject newPlayer = Instantiate(playerPrefab);
                int id = int.Parse(message);
                newPlayer.GetComponent<CubeClass>().Id = id;
                Array.Resize(ref _cubes, _cubes.Length + 1);
                _cubes[_cubes.GetUpperBound(0)] = newPlayer;	
                _cubesById[counter] = newPlayer;
            });
        } else {
            _pp.SubscribeToTopic(Pucket.Connection, message => {
                print(message);
                GameObject newPlayer = Instantiate(playerPrefab);
                newPlayer.AddComponent<Rigidbody>();
                newPlayer.GetComponent<CubeClass>().Id = counter++;
                Array.Resize(ref _cubes, _cubes.Length + 1);
                _cubes[_cubes.GetUpperBound(0)] = newPlayer;
                _cubesById[counter] = newPlayer;
                _pp.CreatePukcet(counter.ToString(), Pucket.Connected);
            });
        }
    }

    public void Update() {
        // print(Time.deltaTime);
        string positions = "";
        _pp.Update();
        if (Server) {
            foreach (var cube in _cubes) {
                Vector3 pos = cube.gameObject.transform.position;
                Quaternion rot = cube.gameObject.transform.rotation;
                positions += cube.GetComponent<CubeClass>().Id + ";" + pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.w +
                             ";" + rot.x + ";" + rot.y + ";" + rot.z + "\n";
            }
            _pp.CreatePukcet(positions,Pucket.Snapshot);
        }

    }


    public void HandleUpdate(string message) {
        if (!Server) {
            Debug.Log(message);
            string[] cubes = message.Split('\n');
            foreach (string c in cubes) {
                if(c.Length == 0) continue;
                string[] pos = c.Split(';');
                _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3 (float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));
            }
        }
    }
}
