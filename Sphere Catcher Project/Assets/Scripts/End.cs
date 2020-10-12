using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class End : MonoBehaviour
{
    private bool win = false;
   public int winCheck;

   public RectTransform PanelGo;

   public Text Text;

    void Start(){
        Invoke("WinScreen", 5);
    }
    
    void Update(){
        winCheck = FindObjectOfType<CharController>().items;
        
    }

    private void OnTriggerEnter(Collider Meta){
        if(Meta.name == "Player" && winCheck == 3){
            win = true;
            PanelGo.gameObject.SetActive(true);

        }
    }
    public void WinScreen(){
        if (win == true){
            Application.Quit();
        }
    }
}
