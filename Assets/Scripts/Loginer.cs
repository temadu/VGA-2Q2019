using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class Loginer : MonoBehaviour
{
  private PacketPrusecor _pp = PacketPrusecor.Instance;
  public UnityEngine.UI.InputField serverIP;
  public string serverIPPreset = null;
  public UnityEngine.UI.InputField personalIP;
  public UnityEngine.UI.InputField username;

  void Start(){
    // this.personalIP.text = IPManager.GetIP(ADDRESSFAM.IPv4);
    // this.personalIP.text = NetworkManager.singleton.networkAddress;
    if(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()){
      var host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (var ip in host.AddressList) {
        print(ip.ToString());
        if (ip.AddressFamily == AddressFamily.InterNetwork) {
          this.personalIP.text = ip.ToString();
          break;
        }
      }
    }
    if(serverIPPreset != null){
      this.serverIP.text = this.serverIPPreset;
    }
  }


  public void Login(){
    Debug.Log("Sending LOGIN");
    _pp.CreatePukcet(personalIP.text+";"+username.text, Pucket.Login);
  }
}
