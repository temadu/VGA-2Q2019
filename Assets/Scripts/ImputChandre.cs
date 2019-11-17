﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImputChandre : MonoBehaviour
{
    private PacketPrusecor _pp = PacketPrusecor.Instance;
    private CubeClass me;

    private Dictionary<KeyCode, char> keyMappings = new Dictionary<KeyCode, char>() {
        {KeyCode.Space, ' '},
        {KeyCode.W, 'W'},
        {KeyCode.A, 'A'},
        {KeyCode.S, 'S'},
        {KeyCode.D, 'D'},
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
        }

        // predecir/simular mis movimientos
    }
}
