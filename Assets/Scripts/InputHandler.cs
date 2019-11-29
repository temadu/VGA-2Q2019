using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PacketProcessor _pp = PacketProcessor.Instance;
    private CubeClass me;
    public SortedList<long,MovementMade> movements = new SortedList<long,MovementMade>();
    public Queue<MovementMade> movementsMade = new Queue<MovementMade>();
    public Queue<MovementMade> posToCheck = new Queue<MovementMade>();
    public Vector3 lastKnownPosition = Vector3.zero;    
    public long lastKnownFrame = 0;    

    public float speed = 0.05f; 

    private Dictionary<KeyCode, char> keyMappings = new Dictionary<KeyCode, char>() {
        {KeyCode.Space, ' '},
        {KeyCode.W, 'W'},
        {KeyCode.A, 'A'},
        {KeyCode.S, 'S'},
        {KeyCode.D, 'D'},
    };
    
    private Dictionary<char, Vector3> keyToVector = new Dictionary<char, Vector3>() {
        {' ', new Vector3(0, 5.5f, 0)},
        {'W', new Vector3(0, 0, 1)},
        {'A', new Vector3(-1, 0, 0)},
        {'S', new Vector3(0, 0, -1)},
        {'D', new Vector3(1, 0, 0)},
    };

    void Start(){
        me = this.GetComponent<CubeClass>();
        _pp.SubscribeToAckTopic(Packet.Input, HandleAcknowledge);
  }

    void Update() {
        string s = me.Id + ";";
        string inputs = "";
        foreach(KeyValuePair<KeyCode, char> keyMapping in this.keyMappings) {
            if(Input.GetKey(keyMapping.Key)) {
                inputs = inputs + keyMapping.Value; 
            }
        }
        if(inputs != "") {
            Debug.Log(s + inputs);
            _pp.CreatePacket(s + inputs, Packet.Input);
            Vector3 newPos = ApplyMovementFromMessage(inputs);
            // this.movementsMade.Enqueue(new MovementMade()
            //     .setMoves(inputs).setClientOrder(Client.Frame).setPos(newPos));
            if(!movements.ContainsKey(Client.Frame)){
                this.movements.Add(Client.Frame, new MovementMade()
                    .setMoves(inputs).setClientOrder(Client.Frame).setPos(newPos));
            }
        }

        // ApplyAllMovements();
        // Debug.Log(this.movements.Count);
    }

  public Vector3 ApplyMovementFromMessage(string message){
    char[] charArr = message.ToCharArray();
    Vector3 movement = Vector3.zero;
    foreach (char c in charArr) {
      movement = movement + keyToVector[c];
    }
    return movement;
  }

  public void applyRealMovement(Vector3 pos, Quaternion rot, long freim){
      MovementMade m = null;
      while(true){
        if(movements.Count == 0)
            break;
        
        m = movements.Values[0];
        if(m.serverOrder <= freim){
            movements.RemoveAt(0);
        } else {
            break;
        }
      }
      this.lastKnownPosition = pos;
      this.lastKnownFrame = freim;
      this.transform.position = pos;
    //   print("CURRENT" + m.pos + " "+ freim);
    //   print("NEXT" + movements.Values[0].pos + " "+ movements.Values[0].serverOrder);
    //   print(this.movements.Count);
    //   this.ApplyAllMovements();

  }
  public void ApplyAllMovements(){
    Vector3 newPos = Vector3.zero;
    foreach( KeyValuePair<long, MovementMade> kvp in movements ) {
        // kvp.Value.pos = lastKnownPosition + newPos + ApplyMovementFromMessage(kvp.Value.moves);
        // newPos = newPos + ApplyMovement(kvp.Value.moves);
        newPos = newPos + kvp.Value.pos;
    }
    // print(newPos);
    this.transform.position = lastKnownPosition + (newPos * speed);
  }

  public void HandleAcknowledge(long clOrder, long svOrder){
    // print("Input Acknowledged: client:" + clOrder + " server:" + svOrder);
    print("LAG: " + ((svOrder-clOrder)/60f));
    // while(true){
    //     if(movementsMade.Count == 0 || movementsMade.Peek().clientOrder > clOrder){
    //         break;
    //     }
    //     MovementMade m = movementsMade.Dequeue();
    //     if(m.clientOrder == clOrder){
    //         m.serverOrder = svOrder;
    //         this.posToCheck.Enqueue(m);
    //         // print("Moviendo:" + m.clientOrder + "moves" + movementsMade.Count + "posses" + posToCheck.Count);
    //         break;
    //     }
    // }
    if(movements.ContainsKey(clOrder)){
        movements[clOrder].serverOrder = svOrder;
    }

  }

  public class MovementMade{
        public long clientOrder;
        public long serverOrder;
        public string moves;
        public Vector3 pos;

        public MovementMade() {
        }

        public MovementMade setClientOrder(long q){
            this.clientOrder = q;
            return this;
        }
        public MovementMade setServerOrder(long q){
            this.serverOrder = q;
            return this;
        }
        public MovementMade setMoves(string m){
            this.moves = m;
            return this;
        }
        public MovementMade setPos(Vector3 p){
            this.pos = p;
            return this;
        }

        public override string ToString(){
            return clientOrder + " " + serverOrder + " " + moves + " " + pos.ToString();
        }
    }
}
