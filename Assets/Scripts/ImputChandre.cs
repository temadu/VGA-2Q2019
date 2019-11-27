using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImputChandre : MonoBehaviour
{
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    private CubeClass me;

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
            ApplyMovement(inputs);
        }

        // predecir/simular mis movimientos
    }

  public void ApplyMovement(string message){
    char[] charArr = message.ToCharArray();
    Vector3 movement = Vector3.zero;
    foreach (char c in charArr)
    {
      // Debug.Log(c);
      movement = movement + keyToVector[c];
    }
    this.transform.position = this.transform.position + (movement * speed);
  }
}
