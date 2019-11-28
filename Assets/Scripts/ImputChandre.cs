using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImputChandre : MonoBehaviour
{
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    private CubeClass me;
    public SortedList<long,MuvmentMeid> movements = new SortedList<long,MuvmentMeid>();
    public Queue<MuvmentMeid> movementsMade = new Queue<MuvmentMeid>();
    public Queue<MuvmentMeid> posToCheck = new Queue<MuvmentMeid>();
    public Vector3 lastKnownPosition = Vector3.zero;    

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
        _pp.SubscribeToAckTopic(Pucket.Input, HandleAcknowledge);
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
            _pp.CreatePukcet(s + inputs, Pucket.Input);
            Vector3 newPos = ApplyMovement(inputs);
            this.movementsMade.Enqueue(new MuvmentMeid()
                .setMoves(inputs).setSlientOrder(Slient.Freim).setPos(newPos));
            this.movements.Add(Slient.Freim, new MuvmentMeid()
                .setMoves(inputs).setSlientOrder(Slient.Freim).setPos(newPos));
        }

        ApplyAllMovements();
    }

  public Vector3 ApplyMovement(string message){
    char[] charArr = message.ToCharArray();
    Vector3 movement = Vector3.zero;
    foreach (char c in charArr)
    {
      movement = movement + keyToVector[c];
    }
    return movement;
  }

  public void applyRealMovement(Vector3 pos, Quaternion rot, long freim){
      while(true){
        if(movements.Count == 0)
            break;
        
        MuvmentMeid m = movements.Values[0];
        if(m.cerverOrder <= freim){
            movements.RemoveAt(0);
        } else {
            break;
        }
      }
      this.lastKnownPosition = pos;
      this.ApplyAllMovements();

  }
  public Vector3 ApplyAllMovements(){
    Vector3 newPos = Vector3.zero;
    foreach( KeyValuePair<long, MuvmentMeid> kvp in movements )
    {
        newPos = newPos + ApplyMovement(kvp.Value.moves);
    }
    this.transform.position = lastKnownPosition + (newPos * speed);
    return this.transform.position;
  }

  public void HandleAcknowledge(long clOrder, long svOrder){
    // print("Input Acknowledged: client:" + clOrder + " server:" + svOrder);
    print("LAG: " + ((svOrder-clOrder)/60f));
    while(true){
        if(movementsMade.Count == 0 || movementsMade.Peek().slientOrder > clOrder){
            break;
        }
        MuvmentMeid m = movementsMade.Dequeue();
        if(m.slientOrder == clOrder){
            m.cerverOrder = svOrder;
            this.posToCheck.Enqueue(m);
            // print("Moviendo:" + m.slientOrder + "moves" + movementsMade.Count + "posses" + posToCheck.Count);
            break;
        }
    }
    if(movements.ContainsKey(clOrder)){
        movements[clOrder].cerverOrder = svOrder;
    }

  }

  public class MuvmentMeid{
        public long slientOrder;
        public long cerverOrder;
        public string moves;
        public Vector3 pos;

        public MuvmentMeid() {
        }

        public MuvmentMeid setSlientOrder(long q){
            this.slientOrder = q;
            return this;
        }
        public MuvmentMeid setCerverOrder(long q){
            this.cerverOrder = q;
            return this;
        }
        public MuvmentMeid setMoves(string m){
            this.moves = m;
            return this;
        }
        public MuvmentMeid setPos(Vector3 p){
            this.pos = p;
            return this;
        }
    }
}
