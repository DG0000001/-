using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterFactory : MonoBehaviour
{
    public abstract Character CreateCharacter(string name);
    public abstract GameObject DestoryCharacter(GameObject gameObject);
}

