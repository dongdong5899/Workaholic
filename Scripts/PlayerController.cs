using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private float _jumpPower = 5;
    [SerializeField]
    public float _slowScale = 0.05f;
    [SerializeField]
    private int _SlowGauge = 5;

    public Rigidbody2D _rb2d;
    private SpriteRenderer _sr;
    private Animator _anime;
    private Light2D _spotLight;
    private Sequence _attackSeq;
    private Sequence _gaugeSeq;
    private Image _gaugeBar;
    private Image _gauge;

    private int _gaugeHeal = 1;

    private float _movX = 0;

    private bool _doubleJump = false;
    public bool _onGround = true;
    private bool _isAttack = false;
    public bool _onSlow = false;

    private float _jumpDelay = 0;
    private float _slowFadeTime = 0.2f;

    private Vector2 _mouseDir = Vector2.zero;

    public enum State
    {
        Idle,
        Move,
        Run
    }

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anime = GetComponent<Animator>();
        _spotLight = transform.Find("Spot").GetComponent<Light2D>();
        _gaugeBar = transform.Find("SlowGauge/GaugeBar").GetComponent<Image>();
        _gauge = _gaugeBar.transform.Find("Gauge").GetComponent<Image>();
    }

    void Start()
    {
        State currentState = State.Idle;

        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Move:
                break;
            case State.Run:
                break;
            default:
                break;
        }
    }


    void Update()
    {
        _mouseDir = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)) - transform.position);
        _mouseDir = _mouseDir.normalized;
        Move();
        Attack();
        TimeSlow();
    }

    public void ReSet()
    {
        _rb2d.velocity = Vector2.zero;
        GameManager.Instance.Hp = 3;
        CameraManager.Instance.CameraRotate(0, 0);
        _sr.color = Color.white;
        transform.position = GameManager.Instance._spawnPoint.transform.GetChild(GameManager.Instance._spawnNum).position + Vector3.up * 0.49f;
        transform.localScale = Vector3.one;
        _jumpDelay = 0;
        _anime.SetBool("Move", false);
    }

    private void Move()
    {
        _onGround = (Physics2D.BoxCast(transform.position - Vector3.up * 0.9f, new Vector2(0.59f, 0.2f), 0, Vector2.down, 0.1f, 1 << LayerMask.NameToLayer("Floor"))
            && _jumpDelay <= 0);

        if (GameManager.Instance._onMove)
        {
            if (_jumpDelay > 0)
            {
                _jumpDelay -= Time.deltaTime;
            }

            _movX = Input.GetAxisRaw("Horizontal");

            if (_movX != 0)
            {
                _anime.SetBool("Move", true);
                transform.localScale = new Vector3(_movX, 1, 1);
            }
            else
            {
                _anime.SetBool("Move", false);
            }

            _rb2d.velocity = new Vector3(_movX * _speed, _rb2d.velocity.y);




            if (_onGround)
            {
                _doubleJump = true;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpPower);
                    _jumpDelay = 0.2f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, -20);
                _doubleJump = false;
            }
            else if (_doubleJump)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpPower);
                    _doubleJump = false;
                }
            }
        }
        else
        {
            _movX = 0;
            _anime.SetBool("Move", false);
        }

        _anime.SetBool("OnGround", _onGround);
        _anime.SetFloat("VY", _rb2d.velocity.y);
    }

    private void Attack()
    {
        _isAttack = _anime.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack");

        if (Input.GetMouseButtonDown(0) && _onGround && GameManager.Instance._onMove)
        {

            if (!_isAttack && !_onSlow)
            {
                _anime.SetTrigger("Attack");
                GameManager.Instance.BackLightWink(0.7f, 0.2f);

                PoolManager.Instance.Pop("DafaultSlice", new Vector3(0.4f * transform.localScale.x, -0.47f, 0), new Vector3(1, transform.localScale.x, 1), new Vector3(90 - 90 * transform.localScale.x, 90, 0), gameObject);
                
                RaycastHit2D hit;
                if (hit = Physics2D.BoxCast(transform.position + new Vector3(transform.localScale.x * -0.3f, -0.4f, 0f),
                    new Vector3(0.2f, 1.2f, 1f), 0, Vector3.right * transform.localScale.x, 3f, 
                    (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Shield"))))
                {
                    //공격 라인 파티클
                    PoolManager.Instance.Pop("Slice", transform.position + Vector3.down * 0.47f, Vector3.one, new Vector3(90 - 90 * transform.localScale.x, 90, 0));

                    if (hit.transform.GetComponent<EnemyController>())
                    {
                        //타격 파티클
                        PoolManager.Instance.Pop("HitSlice", hit.transform.position, new Vector3(1, 1, 1), new Vector3(Random.Range(0f, 180f), 90, 0));
                        PoolManager.Instance.Pop("HitSlice", hit.transform.position, new Vector3(1, 1, 1), new Vector3(Random.Range(0f, 180f), 90, 0));
                        float dir = Mathf.Sign(transform.position.x - hit.transform.position.x);
                        PoolManager.Instance.Pop("Spark", hit.transform.position, new Vector3(1, 1, 1), new Vector3(0, 0, 90 - 90 * dir));

                        hit.transform.GetComponent<EnemyController>().Die();
                    }
                    else
                    {
                        float dir = Mathf.Sign(transform.position.x - hit.transform.position.x);
                        PoolManager.Instance.Pop("SparkS", hit.transform.position, new Vector3(1, 1, 1), new Vector3(0, 0, 90 - 90 * dir));
                    }
                    //카메라
                    CameraManager.Instance.CameraShake(8, 8, 0.3f);
                }
            }
        }
    }

    private void TimeSlow()
    {
        if (!ButtonManager.Instance._esc.activeSelf)
        {
            _gauge.fillAmount = Mathf.Clamp(_gauge.fillAmount + (Time.unscaledDeltaTime / _SlowGauge) * _gaugeHeal, 0, 1);
            if (_gauge.fillAmount == 1)
            {
                if (_gauge.color.a == 1)
                {
                    if (_gaugeSeq != null && _gaugeSeq.IsActive()) _gaugeSeq.Kill();
                    _gaugeSeq = DOTween.Sequence();
                    _gaugeSeq.Append(_gaugeBar.DOFade(0, 1).SetEase(Ease.InBounce));
                    _gaugeSeq.Join(_gauge.DOFade(0, 1).SetEase(Ease.InBounce));
                }
            }
            else
            {
                if (_gauge.color.a == 0)
                {
                    if (_gaugeSeq != null && _gaugeSeq.IsActive()) _gaugeSeq.Kill();
                    _gaugeSeq = DOTween.Sequence();
                    _gaugeSeq.Append(_gaugeBar.DOFade(1, _slowScale * 0.6f).SetEase(Ease.InSine));
                    _gaugeSeq.Join(_gauge.DOFade(1, _slowScale * 0.6f).SetEase(Ease.InSine));
                }
            }
            if (_gauge.fillAmount == 0)
            {
                OnSlow(false);
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnSlow(true);
            }
            if (Input.GetMouseButtonUp(1) || _gauge.fillAmount == 0)
            {
                OnSlow(false);
            }
        }
    }
    public void OnSlow(bool on)
    {
        if (on && !_onSlow)
        {
            _onSlow = true;
            _gaugeHeal = -1;
            TimeManager.Instance.TimeScale(_slowScale);
            GameManager.Instance.BackLightColor(new Color(0.3f, 0.4f, 0.8f), _slowScale * _slowFadeTime * 2);
            if (_attackSeq != null && _attackSeq.IsActive()) _attackSeq.Kill();
            _attackSeq = DOTween.Sequence();
            _attackSeq.Append(DOTween.To(() => _spotLight.intensity, value => _spotLight.intensity = value, 1, _slowScale * _slowFadeTime * 2));
        }
        else if (_onSlow)
        {
            _onSlow = false;
            _gaugeHeal = 1;
            TimeManager.Instance.TimeScale(1f);
            GameManager.Instance.BackLightColor(Color.white, _slowFadeTime);
            if (_attackSeq != null && _attackSeq.IsActive()) _attackSeq.Kill();
            _attackSeq = DOTween.Sequence();
            _attackSeq.Append(DOTween.To(() => _spotLight.intensity, value => _spotLight.intensity = value, 0, _slowFadeTime));
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.transform.tag)
        {
            case "Bullet":
                GameManager.Instance.Hp--;
                if (collision.GetComponent<BulletController>())
                {
                    collision.GetComponent<BulletController>().Die();
                }
                break;
            case "SpawnPoint":
                if (collision.GetComponent<SpawnPoint>())
                {
                    collision.GetComponent<SpawnPoint>().Save();
                }
                break;
            default:
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.transform.tag)
        {
            case "DamageFloor":
                GameManager.Instance.Hp--;
                break;
            default:
                break;
        }
    }
}
