using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : PoolableMono
{
    private ParticleSystem _particle;

    public GameObject _follow;

    public Vector3 _pos;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_follow != null)
        {
            transform.position = _follow.transform.position + _pos;
        }

        if (!_particle.isPlaying)
        {
            PoolManager.Instance.Push(this);
        }
    }

    public override void Init()
    {

    }
}