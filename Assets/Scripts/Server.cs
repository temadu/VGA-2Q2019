using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
  public static long Frame = 0L;
  private GameObject[] _cubes;
  private Dictionary<int, GameObject> _cubesById;
  private PacketProcessor _pp = PacketProcessor.Instance;
  public GameObject playerPrefab;
  private int counter = 0;
  public bool client;
  private void Start()
  {
  	_pp.initStreams(client);
    _cubesById = new Dictionary<int, GameObject>();
    
    _cubes = GameObject.FindGameObjectsWithTag("Cubo");
    counter = 0;

    foreach (GameObject cube in _cubes)
    {
      cube.AddComponent<Rigidbody>();
      _cubesById[cube.GetComponent<CubeClass>().Id] = cube;
      // cube.GetComponent<CubeClass>().Id = counter;
      counter++;
      print(counter);

    }
    

   
    _pp.SubscribeToTopic(Packet.Login, (message, order) =>
    {
      print("NEW USER");
      print(message);
	    string[] ipname = message.Split(';');
      GameObject newPlayer = Instantiate(playerPrefab);
      newPlayer.AddComponent<Rigidbody>();
      newPlayer.AddComponent<MoveVehicle>();
      newPlayer.GetComponent<CubeClass>().Id = ++counter;
      newPlayer.GetComponent<CubeClass>().Name = ipname[1];
      Array.Resize(ref _cubes, _cubes.Length + 1);
      _cubes[_cubes.GetUpperBound(0)] = newPlayer;
      _cubesById[counter] = newPlayer;
      string ids = "";
      string names = "";
      foreach (int key in _cubesById.Keys)
      {
        ids += key.ToString() + ';';
        names += _cubesById[key].GetComponent<CubeClass>().Name + ';';
      }

      ids = ids.Remove(ids.Length - 1);
      names = names.Remove(names.Length - 1);
	    _pp.AddIp(counter, ipname[0]);
      //Ack
      _pp.CreatePacket(order.ToString(), Packet.Login, counter, true);
      _pp.CreatePacket(counter.ToString(), Packet.Logined, counter);
      _pp.CreatePacket((ids + '-' + names), Packet.UpdatePlayersInfo);
      });
    
  }

  public void Update()
  {
    _pp.Update();
    
    if (Frame % 10 == 0) {
      string positions = "";
      foreach (var cube in _cubes) {
        Vector3 pos = cube.gameObject.transform.position;
        Quaternion rot = cube.gameObject.transform.rotation;
        positions += cube.GetComponent<CubeClass>().Id + ";" + pos.x + ";" + pos.y + ";" + pos.z + ";" + rot.w +
                      ";" + rot.x + ";" + rot.y + ";" + rot.z + "\n";
      }
      _pp.CreatePacket(positions, Packet.Snapshot);
    }
    Frame++;
    
  }
}
