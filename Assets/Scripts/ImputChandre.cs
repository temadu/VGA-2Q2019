using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImputChandre : MonoBehaviour
{
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    
    private Dictionary<KeyCode, char> keyMappings = new Dictionary<KeyCode, char>() {
        {KeyCode.Space, ' '},
        {KeyCode.W, 'W'},
        {KeyCode.A, 'A'},
        {KeyCode.S, 'S'},
        {KeyCode.D, 'D'},
    };

    void Start(){
        
    }

    void Update() {
        string s = "";
        foreach(KeyValuePair<KeyCode, char> keyMapping in this.keyMappings) {
            if(Input.GetKey(keyMapping.Key)) {
                s = s + keyMapping.Value; 
            }
        }
        if(s != "") {
            _pp.CreatePukcet(s, Pucket.Input);
        }
    }
}
