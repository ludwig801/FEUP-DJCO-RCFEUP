using UnityEngine;
using System.Collections.Generic;

public class TrackTrigger : MonoBehaviour
{
    public Transform PointA, PointB, Banner;
    public BoxCollider TriggerCollider;
    public float DeltaY, Depth, Height;
    [Range(0, 2)]
    public float ScaleY;
    public Color OnTriggerEnterColor, OnTriggerExitColor;
    public bool Visible;

    [SerializeField]
    List<MeshRenderer> _meshRenderers;
    [SerializeField]
    List<string> _tagsToIgnore;
    bool _oldVisible;
    Vector3 _aOrigin, _bOrigin, _bannerOrigin;

    void Start()
    {
        _meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());

        var delta = (PointB.position - PointA.position);
        TriggerCollider.size = new Vector3(delta.magnitude, Height, Depth);
        TriggerCollider.transform.position = Vector3.Lerp(PointA.position, PointB.position, 0.5f);
        TriggerCollider.transform.rotation = Quaternion.LookRotation(Vector3.Cross(delta, Vector3.up), Vector3.up);

        _oldVisible = Visible;
        SetColor(OnTriggerExitColor);

        _aOrigin = PointA.position;
        _bOrigin = PointB.position;
        _bannerOrigin = Vector3.Lerp(_aOrigin, _bOrigin, 0.5f);
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

        PointA.position = _aOrigin + new Vector3(0, DeltaY, 0);
        PointB.position = _bOrigin + new Vector3(0, DeltaY, 0);
        PointA.localScale = new Vector3(1, ScaleY, 1);
        PointB.localScale = PointA.localScale;
        //var delta = (PointB.position - PointA.position);
        //Banner.localScale = new Vector3(1, ScaleY, delta.magnitude * 0.06f);
        //Banner.position = _bannerOrigin + new Vector3(0, Height, 0);
        //Banner.rotation = Quaternion.LookRotation(delta, Vector3.up);
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
            var mats = renderer.materials;
            foreach (var material in mats)
            {
                material.color = newColor;
            }
        }
    }
}
