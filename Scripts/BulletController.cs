using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BulletController : PoolableMono
{
    public override void Init()
    {
        _target.SetActive(false);
    }

    private Rigidbody2D _rb2d;
    private GameObject _target;
    private Sequence _seq;

    public Vector2 _dir = Vector2.zero;

    private Vector3 _mousePos = Vector3.zero;
    private Vector2 _mouseDir = Vector2.zero;

    private PlayerController _pc;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _target = transform.Find("Target").gameObject;
        _pc = GameManager.Instance.Player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (GameManager.Instance._stopEffect)
        {
            PoolManager.Instance.Push(this);
        }
    }


    private void OnEnable()
    {
        _rb2d.velocity = _dir;
        Invoke("Die", 2f);
    }

    public void Die()
    {
        CancelInvoke();
        PoolManager.Instance.Push(this);
    }


    private void OnMouseEnter()
    {
        if ((_pc.transform.position - Vector3.up * 0.2f - transform.position).magnitude < 3f)
        {
            _target.transform.localScale = Vector3.one * 0.15f;
            _target.transform.eulerAngles = Vector3.zero;
            _target.SetActive(true);
            if (_seq != null && _seq.IsActive()) _seq.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(_target.transform.DOScale(Vector3.one * 0.1f, _pc._slowScale * 0.4f).SetEase(Ease.InSine));
            _seq.Join(_target.transform.DORotate(new Vector3(0, 0, 360), _pc._slowScale * 0.4f, RotateMode.FastBeyond360).SetEase(Ease.OutSine));
        }
    }
    private void OnMouseExit()
    {
        _target.SetActive(false);
    }
    private void OnMouseDown()
    {
        if ((_pc.transform.position - Vector3.up * 0.2f - transform.position).magnitude < 2.5f)
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = 0;
            _mouseDir = (_mousePos - GameManager.Instance.Player.transform.position + Vector3.up * 0.2f);
            _mouseDir = _mouseDir.normalized;


            PoolManager.Instance.Pop("HitSlice", transform.position, Vector3.one, new Vector3(Random.Range(0f, 180f), 90, 0));
            PoolManager.Instance.Pop("DafaultSlice", new Vector3(_mouseDir.x, _mouseDir.y - 0.2f, 0),
                new Vector3(0.5f, 0.7f, 0.5f), new Vector3(-Mathf.Atan2(_mouseDir.y, _mouseDir.x) * Mathf.Rad2Deg, 90, 0), GameManager.Instance.Player);
            PoolManager.Instance.Pop("BulletSpark", transform.position, Vector3.one, Vector3.zero);
            CameraManager.Instance.CameraShake(2, 2, 0.2f);
            Die();
        }
    }
}
