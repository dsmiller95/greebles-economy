using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Billboard : MonoBehaviour
{
    public string cameraName;

    private GameObject cam;

    void Start()
    {
        this.cam = GameObject.FindGameObjectsWithTag("MainCamera").Where(gameObject => gameObject.name == this.cameraName).First();
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
