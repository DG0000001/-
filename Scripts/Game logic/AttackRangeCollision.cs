using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeCollision : MonoBehaviour
{
    public GameObject Rote;

    void OnTriggerEnter(Collider collider)
    {
        Rote.GetComponent<Rote>().BitEnemies.Add(collider.gameObject);
   }    
    private void OnTriggerExit(Collider collider)
    {
        Rote.GetComponent<Rote>().BitEnemies.Remove(collider.gameObject);
    }
}
