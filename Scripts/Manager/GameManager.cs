using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;

    public GameObject Player;
    public PlayerController _pc;
    private SpriteRenderer _sr;
    public Light2D Global;
    private Sequence _lightIntensitySeq;
    private Sequence _lightColorSeq;
    [SerializeField]
    public GameObject _spawnPoint;
    [SerializeField]
    public GameObject _wall;
    [SerializeField]
    private CinemachineBoss _CBoss;

    private float _inv = 1;
    public float _invTime = 0;

    private int _hp = 3;
    public int Hp
    {
        get => _hp;
        set
        {
            if (value < _hp)
            {
                if (_invTime > 0)
                {
                    return;
                }
                CameraManager.Instance.CameraShake(5, 3, _inv);
                _hp = Mathf.Clamp(value, 0, 3);
                if (value == 0) Die();
                _invTime = _inv;
            }
            else
            {
                _hp = Mathf.Clamp(value, 0, 3);
            }
            UIManager.Instance.HpUIUpdate();
        }
    }

    public int _spawnNum = 0;

    public bool _stopEffect = false;
    public bool _onMove = true;

    private void Awake()
    {
        Instance = this;
        Player = GameObject.Find("Player");
        _sr = Player.GetComponent<SpriteRenderer>();
        _pc = Player.GetComponent<PlayerController>();
    }

    private void Start()
    {
        _spawnNum = PlayerPrefs.GetInt("SpawnPoint");
        Player.transform.position = _spawnPoint.transform.GetChild(_spawnNum).position;
        StartCoroutine("ReSpawn");
    }

    private void Update()
    {
        if (_invTime > 0)
        {
            _invTime -= Time.deltaTime;
            _sr.color = Color.Lerp(Color.white, Color.red, _invTime / _inv);
        }
    }

    public void BackLightWink(float minValue, float time)
    {
        if (_lightIntensitySeq != null && _lightIntensitySeq.IsActive()) _lightIntensitySeq.Kill();
        _lightIntensitySeq = DOTween.Sequence();
        _lightIntensitySeq.Append(DOTween.To(() => minValue, value => Global.intensity = value, 0.9f, time));
    }
    public void BackLightColor(Color color, float time)
    {
        if (_lightColorSeq != null && _lightColorSeq.IsActive()) _lightColorSeq.Kill();
        _lightColorSeq = DOTween.Sequence();
        _lightColorSeq.Append(DOTween.To(() => Global.color, value => Global.color = value, color, time));
    }

    private void Die()
    {
        UIManager.Instance.Fade(1, 0.2f, Ease.InQuad);
        TimeManager.Instance.TimeScale(0.2f);
        _onMove = false;
        StartCoroutine("ReSpawn");
    }

    IEnumerator ReSpawn()
    {
        yield return new WaitForSeconds(0.2f);
        _invTime = 0;
        _pc._rb2d.velocity = Vector2.zero;
        _pc.ReSet();
        TimeManager.Instance.TimeScale(1f);
        for (int i = 0; i < _wall.transform.childCount; i++)
        {
            WallController _wc = _wall.transform.GetChild(i).GetComponent<WallController>();
            _wc.ReSet();
            _wc.gameObject.SetActive(true);
            if (i < _spawnPoint.transform.GetChild(_spawnNum).GetComponent<SpawnPoint>()._clearWall)
            {
                _wc._clear = true;
                _wc.gameObject.SetActive(false);
            }
            else
            {
                _wc.Set();
                if (i == 11)
                {
                    _CBoss.Set();
                }
            }
        }
        _stopEffect = true;
        yield return new WaitForSeconds(1f);
        _stopEffect = false;
        UIManager.Instance.Fade(0, 1.5f, Ease.InQuad);
        yield return new WaitForSeconds(1.5f);
        _onMove = true;
    }
}
