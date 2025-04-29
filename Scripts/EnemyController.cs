using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public enum EnemyTag
{
    Circle,
    Box,
}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyTag enemyTag;

    [SerializeField]
    private float _AttackDelay = 2f;
    [SerializeField]
    private float _startAttackDelay = 1f;
    private float _AttackTime = 1f;

    private Vector3 _dir = Vector3.zero;
    private Vector3 _dis = Vector3.zero;

    private GameObject _line;
    private GameObject _inLine;
    private GameObject _rail;
    private SpriteRenderer _lineSR;

    [SerializeField]
    private float _lineMaxTime = 2;
    private float _lineTime = 0;

    private Sequence _seq;

    private void Awake()
    {
        _line = transform.Find("Line").gameObject;
        if (enemyTag == EnemyTag.Box)
        {
            _inLine = _line.transform.Find("InLine").gameObject;
            _rail = transform.Find("Rail").gameObject;
        }
        _lineSR = _line.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        switch (enemyTag)
        {
            case EnemyTag.Circle:
                Circle();
                break;
            case EnemyTag.Box:
                if (GameManager.Instance._stopEffect)
                {
                    if (_seq != null && _seq.IsActive()) _seq.Kill();
                    _line.SetActive(false);
                    _rail.SetActive(false);
                }
                Box();
                break;
            default:
                break;
        }
    }

    public void ReSet()
    {
        if (_seq != null && _seq.IsActive()) _seq.Kill();
        StopAllCoroutines();
        _AttackTime = _startAttackDelay;
        _lineTime = 0;
        switch (enemyTag)
        {
            case EnemyTag.Circle:
                _lineSR.color = new Color(1, 0, 0, 0f);
                break;
            case EnemyTag.Box:
                _line.SetActive(false);
                _rail.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void Circle()
    {
        _dis = GameManager.Instance.Player.transform.position - transform.position;
        _dir = (_dis - Vector3.up * 0.2f).normalized;

        if (_lineTime < _lineMaxTime)
        {
            _lineTime += Time.deltaTime;
            _line.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg + 90 - 90 * transform.localScale.x);
        }
        if (_dis.magnitude < 15f)
        {
            if (_AttackTime > 0)
                _AttackTime -= Time.deltaTime;
            else
            {
                _AttackTime = 100;

                StartCoroutine("CircleFire");
            }
        }
    }


    private void Box()
    {
        _dis = GameManager.Instance.Player.transform.position - transform.position;
        _dir = (_dis - Vector3.up * 0.2f).normalized;

        if (_dis.magnitude < 20f)
        {
            if (_AttackTime > 0)
                _AttackTime -= Time.deltaTime;
            else
            {
                _AttackTime = 100;

                BoxSeq();
            }
        }
    }

    public void Die()
    {
        if (_seq.IsActive()) _seq.Kill();
        gameObject.SetActive(false);
    }


    IEnumerator CircleFire()
    {
        _line.SetActive(true);
        _lineTime = 0;
        _lineSR.color = new Color(1, 0, 0, 0.12f);
        while (_lineTime < _lineMaxTime)
        {
            _seq = DOTween.Sequence();
            _seq.Append(_lineSR.DOFade(_lineSR.color.a == 0.04f ? 0.12f : 0.04f, (_lineMaxTime - _lineTime) / (_lineMaxTime * 4)));

            yield return new WaitForSeconds((_lineMaxTime - _lineTime) / (_lineMaxTime * 4));
        }
        Vector2 dir = (GameManager.Instance.Player.transform.position - Vector3.up * 0.2f - transform.position).normalized;

        _seq = DOTween.Sequence();
        _seq.Append(_lineSR.DOFade(0.12f, 0.1f));
        _seq.AppendInterval(0.3f);
        _seq.Append(_lineSR.DOFade(0, 1f));
        _seq.AppendCallback(() =>
        {
            _line.SetActive(false);
            _AttackTime = _AttackDelay;
        });

        for (int i = 0; i < 3; i++)
        {
            PoolManager.Instance.Pop("Bullet", transform.position, Vector3.one, Vector3.zero, dir, 20f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    private void BoxSeq()
    {
        if (_seq != null && _seq.IsActive()) _seq.Kill();
        _seq = DOTween.Sequence();
        _seq.AppendCallback(() =>
        {
            _line.SetActive(true);
            _line.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg + 90 - 90 * transform.localScale.x);
            _rail.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg + 90 - 90 * transform.localScale.x);

            _inLine.transform.localScale = new Vector3(1, 0, 1);
        });
        _seq.Append(_inLine.transform.DOScaleY(1, _lineMaxTime).SetEase(Ease.InSine));
        _seq.AppendCallback(() =>
        {
            _line.SetActive(false);
            _rail.SetActive(true);
            _rail.transform.localScale = new Vector3(100, 1, 1);
            CameraManager.Instance.CameraShake(7, 7, 1);
        });
        _seq.Append(_rail.transform.DOScaleY(0, 0.5f).SetEase(Ease.InSine));
        _seq.AppendCallback(() =>
        {
            _rail.SetActive(false);
            _AttackTime = _AttackDelay;
        });


    }
}
