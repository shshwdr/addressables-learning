using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseHandleOnDestroy : MonoBehaviour
{
    public void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }

    public event Action OnDestroyEvent;
}
