using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public struct PatternSet
{
    public WallController _inWalls;
    public WallController _outWalls;
    public float _rotate;
    public float _circleRotate;
}

public class CinemachineBoss : MonoBehaviour
{
    [SerializeField]
    private PatternSet[] _ps;
    /*private WallController[] _inWalls;
    [SerializeField]
    private WallController[] _outWalls;*/
    private bool[] _pattern = new bool[6];
    private GameObject _body;
    private GameObject _circle;
    private SpriteRenderer _bsr;
    private SpriteRenderer _csr;
    private Sequence _seq;

    private bool _on = false;

    private void Awake()
    {
        _body = transform.Find("CameraEnemyBody").gameObject;
        _circle = transform.Find("CameraEnemy").gameObject;
        _bsr = _body.GetComponent<SpriteRenderer>();
        _csr = _circle.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Set();
    }

    public void Set()
    {
        _circle.transform.eulerAngles = Vector3.zero;
        _body.transform.eulerAngles = Vector3.zero;
        _circle.transform.localPosition = Vector2.zero;
        _body.transform.localPosition = Vector2.zero;
        _circle.SetActive(true);
        _body.SetActive(true);
        _on = false;
        for (int i = 0; i < _pattern.Length; i++)
        {
            _pattern[i] = false;
        }
        _bsr.color = new Color(1, 1, 1, 0);
        _csr.color = new Color(1, 1, 1, 0);
        _seq.Kill();
    }

    void Update()
    {
        if ((GameManager.Instance.Player.transform.position - transform.position).magnitude < 21 && !_on)
        {
            if (GameManager.Instance._onMove) GameManager.Instance._onMove = false;
            if (GameManager.Instance.Player.GetComponent<PlayerController>()._onGround) On();
            GameManager.Instance._pc._rb2d.AddForce(Vector2.down * 10);
        }
        for (int i = 0; i < _ps.Length; i++)
        {
            if (_ps[i]._inWalls._clear && !_pattern[i])
            {
                _pattern[i] = true;
                Pattern(i);
            }
        }
    }

    private void On()
    {
        _on = true;
        GameManager.Instance._onMove = false;
        if (_seq != null && _seq.IsActive()) _seq.Kill();
        _seq = DOTween.Sequence();
        _seq.AppendCallback(() => 
        {
            UIManager.Instance.Fade(1, 0, Ease.InQuad);
        });
        _seq.Append(_bsr.DOFade(1, 1f).SetEase(Ease.InQuad));
        _seq.Join(_csr.DOFade(1, 1f).SetEase(Ease.InQuad));
        _seq.AppendCallback(() =>
        {
            UIManager.Instance.Fade(0, 7, Ease.InOutSine);
            CameraManager.Instance.CameraZoom(8, 7, Ease.InOutSine);
        });
        _seq.AppendInterval(8);
        _seq.Append(_circle.transform.DORotate(new Vector3(0, 0, 720), 2.3f, RotateMode.FastBeyond360).SetEase(Ease.InOutElastic));
        _seq.AppendInterval(1);
        _seq.AppendCallback(() => 
        {
            GameManager.Instance._onMove = true;
            CameraManager.Instance.CameraZoom(6, 1, Ease.OutExpo);
            _ps[0]._inWalls._on = true;
        }); 
    }

    private void Pattern(int num)
    {
        if (_seq != null && _seq.IsActive()) _seq.Kill();
        _seq = DOTween.Sequence();
        if (num == 3)
        {
            if (GameManager.Instance._spawnNum != 4)
            {
                GameManager.Instance.Hp = 3;
                CameraManager.Instance.CameraShake(10, 10, 1);
                for (int i = 0; i < 5; i++)
                {
                    _seq.AppendCallback(() =>
                    {
                        PoolManager.Instance.Pop("BulletSpark", transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0),
                            Vector3.one * 3, Vector3.zero);
                    });
                    _seq.AppendInterval(0.1f);
                }
                _seq.Join(_body.transform.DOMoveY(-44f, 2.5f).SetEase(Ease.InOutBack));
                _seq.Join(_body.transform.DORotate(new Vector3(0, 0, 120), 2.5f).SetEase(Ease.OutCirc));
                _seq.Insert(1f, _circle.transform.DORotate(new Vector3(0, 0, -_ps[num]._circleRotate), 1, RotateMode.FastBeyond360).SetEase(Ease.InBack));
                _seq.InsertCallback(2, () =>
                {
                    CameraManager.Instance.CameraRotate(_ps[num]._rotate, 1, Ease.OutElastic);
                });
                _seq.AppendCallback(() =>
                {
                    _body.SetActive(false);
                });
            }
        }
        else if (num == 5)
        {
            if (GameManager.Instance._spawnNum != 4)
            {
                CameraManager.Instance.CameraShake(10, 10, 1);
                for (int i = 1; i <= 50; i++)
                {
                    if (i % 10 == 0)
                    {
                        _seq.AppendCallback(() =>
                        {
                            CameraManager.Instance.CameraShake(10, 10, 1);
                            PoolManager.Instance.Pop("BulletSpark", transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0),
                                Vector3.one * 7, Vector3.zero);
                        });
                    }
                    _seq.AppendCallback(() =>
                    {
                        PoolManager.Instance.Pop("BulletSpark", transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0),
                            Vector3.one * 2, Vector3.zero);
                    });

                    _seq.AppendInterval(0.1f);
                }
                _seq.InsertCallback(0, () =>
                {
                    CameraManager.Instance.CameraRotate(_ps[num]._rotate, 2, Ease.InOutSine);
                });
                _seq.Append(_circle.transform.DOMoveY(-24f, 0.25f).SetEase(Ease.OutElastic));
                _seq.Append(_circle.transform.DOMoveY(-50f, 2f).SetEase(Ease.InQuint));
                _seq.Join(_circle.transform.DORotate(new Vector3(0, 0, 120), 2f).SetEase(Ease.InOutSine));
                _seq.AppendCallback(() =>
                {
                    CameraManager.Instance.CameraShake(5, 5, 3.5f);
                    _circle.SetActive(false);
                });
            }
        }
        else
        {
            _seq.Append(_circle.transform.DORotate(new Vector3(0, 0, -_ps[num]._circleRotate), 1, RotateMode.FastBeyond360).SetEase(Ease.InBack));
            _seq.AppendCallback(() =>
            {
                CameraManager.Instance.CameraRotate(_ps[num]._rotate, 1, Ease.OutElastic);
            });
            _seq.AppendInterval(1);
        }
        _seq.AppendCallback(() =>
        {
            _ps[num]._outWalls._on = true;
        });
    }
}
