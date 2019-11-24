using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtificialLag : MonoBehaviour{

  private bool randomLag = false;
  private float msLag = 0;
  private float packetDropChance = 0;

  public UnityEngine.UI.Text msLagText;
  public UnityEngine.UI.Text packetDropText;
  public UnityEngine.UI.Slider msLagSlider;
  public UnityEngine.UI.Slider packetDropSlider;

  public void setRandomLag(bool randomLag){
    this.randomLag = randomLag;
  }
  public void setMsLag(float msLag){
    this.msLag = msLag;
    msLagText.text = string.Format("{0:0} ms", this.msLag);
  }
  public void setPacketDropChance(float packetDropChance){
    this.packetDropChance = packetDropChance;
    packetDropText.text = string.Format("{0:0.00}", this.packetDropChance);
  }

  public void applyChanges(){
    // Esta verga es xq no anda el dynamic float en unity 2019 
    setMsLag(msLagSlider.value);
    setPacketDropChance(packetDropSlider.value);
    // Apply cambios en el UdpConnection
  }

  private void Start(){
  }

  void Update(){
      
  }
}
