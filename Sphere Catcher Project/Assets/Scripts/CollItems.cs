using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollItems : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        if(other.name == "Player"){
            other.GetComponent<CharController>().items++;

            Destroy(gameObject);
        }
    }
}
