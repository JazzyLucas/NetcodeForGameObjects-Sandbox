using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorToWorldSpacePoint : MonoBehaviour
{
    [field: SerializeField] public Transform Target { get; private set; }
    [field: SerializeField] public RectTransform RectTransform { get; private set; }

    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Target != null)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(Target.position);

            RectTransform.position = screenPosition;
        }
    }
}
