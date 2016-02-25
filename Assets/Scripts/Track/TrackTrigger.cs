using UnityEngine;
using System.Collections.Generic;

public class TrackTrigger : MonoBehaviour
{
    public Transform PointA, PointB;
    public BoxCollider TriggerCollider;
    public float Depth, Height;
    public bool Visible;
    public Color OnTriggerEnterColor, OnTriggerExitColor;

    [SerializeField]
    List<MeshRenderer> _meshRenderers;
    [SerializeField]
    List<string> _tagsToIgnore;
    bool _oldVisible;

    void Start()
    {
        _meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());

        var delta = (PointB.position - PointA.position);
        TriggerCollider.size = new Vector3(delta.magnitude, Height, Depth);
        TriggerCollider.transform.position = Vector3.Lerp(PointA.position, PointB.position, 0.5f);
        TriggerCollider.transform.rotation = Quaternion.LookRotation(Vector3.Cross(delta, Vector3.up), Vector3.up);

        _oldVisible = Visible;
        SetColor(OnTriggerExitColor);
    }

    void Update()
    {
        if (_oldVisible != Visible)
        {
            foreach (var renderer in _meshRenderers)
            {
                renderer.enabled = Visible;
            }
            _oldVisible = Visible;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_tagsToIgnore.Contains(other.tag))
            SetColor(OnTriggerEnterColor);
    }

    void OnTriggerExit(Collider other)
    {
        SetColor(OnTriggerExitColor);
    }

    void SetColor(Color newColor)
    {
        foreach (var renderer in _meshRenderers)
        {
            renderer.material.color = newColor;
        }
    }
}
