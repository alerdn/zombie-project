using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyToLifetime : MonoBehaviour
{
    [SerializeField] private float _lifetime = 5f;
    void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}
