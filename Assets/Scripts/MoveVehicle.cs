using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
public class MoveVehicle : MonoBehaviour
{
    [Range(1, 10)]
    public float accelerationForce = 10;

    [Range(1, 1000)]
    public float dragDividend = 500;

    public float speed = 0.05f;

    private PacketPrusecor _pp = PacketPrusecor.Instance;
    private CubeClass me;

    private Rigidbody rigidBody;
    private Dictionary<char, Vector3> keyMappings = new Dictionary<char, Vector3>() {
        {' ', new Vector3(0, 5.5f, 0)},
        {'W', new Vector3(0, 0, 1)},
        {'A', new Vector3(-1, 0, 0)},
        {'S', new Vector3(0, 0, -1)},
        {'D', new Vector3(1, 0, 0)},
    };

    public bool IsGrounded()
    { return Physics.Raycast(this.transform.position, -Vector3.up, 0.65f); }

    public void Start() { 
        this.rigidBody = GetComponent<Rigidbody>(); 
        _pp.SubscribeToTopic(Pucket.Input, HandleUpdate);
        me = this.GetComponent<CubeClass>();
    }

    public void HandleUpdate(string message, long order) {
        string[] split = message.Split(';');
        int id = int.Parse(split[0]);
        char[] charArr = split[1].ToCharArray();
        
        if(me.Id == id) {
            Vector3 movement = Vector3.zero;
            foreach(char c in charArr) {
                Debug.Log(c);
                movement = movement + keyMappings[c];

                // if(this.IsGrounded()) {
                //     Debug.Log("grounder");
                    
                //     Vector3 objectForce = keyMappings[c] * this.accelerationForce;
                //     this.rigidBody.drag = objectForce.sqrMagnitude / this.dragDividend;
                //     this.rigidBody.AddForce(objectForce);
                // }
            }
            this.transform.position = this.transform.position + (movement * speed);
        }
    }
}