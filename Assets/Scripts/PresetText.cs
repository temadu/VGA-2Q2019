using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetText : MonoBehaviour{
  public string text;
  private UnityEngine.UI.InputField textField;
  private void Start(){
    textField = GetComponent<UnityEngine.UI.InputField>();
    textField.text = text;
  }

  void Update(){
      
  }
}
