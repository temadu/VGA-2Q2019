using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour {
    public static long Frame = 0L;
    private Dictionary<int, GameObject> _cubesById;
    private PacketProcessor _pp = PacketProcessor.Instance;
    public GameObject playerPrefab;
    public Buffer buf;
    public bool client;
    private int clientID = -2;
    private InputHandler clientInputHandler = null;
    private void Start() {
    	_pp.initStreams(client);
        
        _cubesById = new Dictionary<int, GameObject>();

        buf = new Buffer();
        // data de login? ip? name?
        _pp.SubscribeToTopic(Packet.Logined, (message, order) => {
            Debug.Log("LOGINNEEEEDDDD");
            clientID = int.Parse(message);
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.name = "CualquierCosa";
            newPlayer.GetComponent<CubeClass>().Id = clientID;
            clientInputHandler = newPlayer.AddComponent<InputHandler>();
            newPlayer.transform.position = new Vector3(0,0,0);
            clientInputHandler.lastKnownPosition = new Vector3(0,0,0);
            _cubesById[clientID] = newPlayer;
            _pp.CreatePacket(order.ToString(), Packet.Logined, -1, true);
        });

        _pp.SubscribeToTopic(Packet.Snapshot, (message, order) => {
            // agregar al buffer/interpolar el buffer en el update
            buf.add(order, message);            
        });
        
        _pp.SubscribeToTopic(Packet.UpdatePlayersInfo, (message, order) => {
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
            _pp.CreatePacket(order.ToString(), Packet.UpdatePlayersInfo, -1, true);
        });
        
    }

    public void Update() {
        _pp.Update();
        
        Buffer.BufferElem first = buf.peak();
        if(first == null){
            Frame = 0;
            return;
        }

        if(Frame == 0){
            if(buf.peakEnd().order > first.order + 10){
                Frame = first.order;
                // Seteo in interpolar
                string[] cubes = first.positions.Split('\n');
                foreach (string c in cubes) {
                  if (c.Length == 0) continue;
                  string[] pos = c.Split(';');
                  // Puede ser que estos cubosById no esten? 
                  if (_cubesById.ContainsKey(int.Parse(pos[0]))) {
                    _cubesById[int.Parse(pos[0])].gameObject.transform.position = new Vector3(float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));
                    _cubesById[int.Parse(pos[0])].gameObject.transform.rotation = new Quaternion(float.Parse(pos[4]), float.Parse(pos[5]), float.Parse(pos[6]), float.Parse(pos[7]));
                  }
                }
                buf.removeFirst();
                // No se si aca va un frame++. Creo que no importa mucho igual.
            }
        } else {
            //Interpolo
            long interPolationSteps = first.order - Frame;
            // print(Frame);
            // print(interPolationSteps);
            // interpolar basado en el buffer
            string[] cubes = first.positions.Split('\n');
            // for tiene q hacerse en un update
            foreach (string c in cubes) {
                if (c.Length == 0) continue;
                string[] pos = c.Split(';');
                if (_cubesById.ContainsKey(int.Parse(pos[0]))) {
                    //lerp
                    if(int.Parse(pos[0]) == clientID){
                        clientInputHandler.applyRealMovement( Vector3.Lerp(_cubesById[int.Parse(pos[0])].gameObject.transform.position, 
                            new Vector3(float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3])), 
                            1f/interPolationSteps),
                            Quaternion.Lerp(_cubesById[int.Parse(pos[0])].gameObject.transform.rotation, 
                            new Quaternion(float.Parse(pos[4]), float.Parse(pos[5]), float.Parse(pos[6]), float.Parse(pos[7])), 
                            1f/interPolationSteps), Frame
                        );
                    } else {
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
            }
            if(interPolationSteps <= 1){
                buf.removeFirst();
            }
            Frame++;
        }
        // predecir a los demas
    }

}
