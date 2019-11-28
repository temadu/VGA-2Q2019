using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Slient : MonoBehaviour {
    public static long Freim = 0L;
    private Dictionary<int, GameObject> _cubesById;
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    public GameObject playerPrefab;
    private Bafer bafer;
    public bool client;
    private void Start() {
    	_pp.initStrims(client);
        
        _cubesById = new Dictionary<int, GameObject>();

        bafer = new Bafer();
        // data de login? ip? name?
        _pp.SubscribeToTopic(Pucket.Logined, (message, order) => {
            Debug.Log("LOGINNEEEEDDDD");
            int id = int.Parse(message);
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.name = "CualquierCosa";
            newPlayer.GetComponent<CubeClass>().Id = id;
            newPlayer.AddComponent<ImputChandre>();
            newPlayer.transform.position = new Vector3(0,0,0);
            _cubesById[id] = newPlayer;
            _pp.CreatePukcet(order+";"+Slient.Freim.ToString(), Pucket.Logined, -1, true);
        });

        _pp.SubscribeToTopic(Pucket.Snapshot, (message, order) => {
            // agregar al buffer/interpolar el buffer en el update
            bafer.add(order, message);
            
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
                    cube = Instantiate(playerPrefab);
                    cube.transform.position = new Vector3(0,0,0);
                } 
                cube.GetComponent<CubeClass>().Id = int.Parse(ids[i]);
                cube.GetComponent<CubeClass>().Name = names[i];
                _cubesById[int.Parse(ids[i])] = cube;
            }
            _pp.CreatePukcet(order+";"+Slient.Freim.ToString(), Pucket.UpdatePlayersInfo, -1, true);
        });
        
    }

    public void Update() {
        _pp.Update();
        
        Bafer.Pocket first = bafer.peak();
        if(first == null){
            Freim = 0;
            return;
        }

        if(Freim == 0){
            if(bafer.peakEnd().horder > first.horder + 120){
                Freim = first.horder;
                // Seteo in interpolar
                string[] cubes = first.possisions.Split('\n');
                foreach (string c in cubes) {
                  if (c.Length == 0) continue;
                  string[] pos = c.Split(';');
                  // Puede ser que estos cubosById no esten? 
                  if (_cubesById.ContainsKey(int.Parse(pos[0]))) {
                    _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3(float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));
                    _cubesById[int.Parse(pos[0])].gameObject.transform.rotation = new Quaternion(float.Parse(pos[4]), float.Parse(pos[5]), float.Parse(pos[6]), float.Parse(pos[7]));
                  }
                }
                bafer.removeFirst();
                // No se si aca va un frame++. Creo que no importa mucho igual.
            }
        } else {
            //Interpolo
            long interPolationSteps = first.horder - Freim;
            print(Freim);
            print(interPolationSteps);
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
            if(interPolationSteps <= 1){
                bafer.removeFirst();
            }
            Freim++;
        }
        // predecir a los demas
    }

}
