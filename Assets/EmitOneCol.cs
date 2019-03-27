﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitOneCol : MonoBehaviour
{

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("monster"))
        {
            GameManager2.level1Hp -= 1;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("monster"))
        {
            Destroy(col.gameObject);
        }
    }
}
