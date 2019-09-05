using DefaultNamespace;
using UnityEngine;

public class GameWorld : MonoBehaviour,StrimObserver{
    public bool Server;

    private GameObject[] _cubes;
    private GameObject[] _cubesById;

    private PacketPrusecor _pp;

    private void Start() {
        
        
        _pp.SubscribeToStrim(Pucket.Snapshot,this);
        _cubes = GameObject.FindGameObjectsWithTag("Cubo");
        _cubesById = new GameObject[10];
        int counter = 0;
			
        foreach (GameObject cube in _cubes) {
            if (Server) {
                cube.AddComponent<Rigidbody>();
            }
            cube.GetComponent<CubeClass>().Id = counter;
            _cubesById[counter] = cube;
            counter++;
            print(counter);
			
        }
    }

    public void Update() {
        string positions = "";

//		if (Input.GetKeyDown(KeyCode.Return)) {
        if (Server) {
            foreach (var cube in _cubes) {
                Vector3 pos = cube.gameObject.transform.position;
                Quaternion rot = cube.gameObject.transform.rotation;
                positions += cube.GetComponent<CubeClass>().Id + "," + pos.x + "," + pos.y + "," + pos.z + "," + rot.w +
                             "," + rot.x + "," + rot.y + "," + rot.z + "\n";
            }

        }
        _pp.CreatePukcet(positions,Pucket.Snapshot);
        
    }


    public void HandleUpdate(string message) {
        Debug.Log(message);
        string[] cubes = message.Split('\n');
        foreach (string c in cubes) {
            string[] newPos = c.Split(',');
            _cubesById[int.Parse(newPos[0])].gameObject.transform.position = new Vector3 (float.Parse(newPos[1]), float.Parse(newPos[2]), float.Parse(newPos[3]));
        }
    }
}
