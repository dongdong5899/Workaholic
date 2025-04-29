using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class WallController : MonoBehaviour
{
    private GameObject _objs;

    [SerializeField]
    private float _dieTime = 2f;
    [SerializeField]
    private WallController _before;
    [SerializeField]
    private float _spawnDelay = 0.05f;

    private Sequence _seq = null;
    private TextMeshProUGUI _tmp;
    public bool _clear = false;
    public bool _onEnemy = false;
    public bool _on = false;

    private void Awake()
    {
        _tmp = transform.Find("Canvas/01").GetComponent<TextMeshProUGUI>();
        _objs = transform.Find("Enemy").gameObject;
    }

    private void Start()
    {
        EnemySetting();
    }

    private void Update()
    {
        if (_clear && _onEnemy)
        {
            EnemySetting();
        }
        if ((_before == null ? _on : _before._clear) && !_onEnemy)
        {
            StartCoroutine("EnemySpawn");
        }
        IsDie();
    }

    private void IsDie()
    {
        if (_onEnemy)
        {
            for (int i = 0; i < _objs.transform.childCount; i++)
            {
                if (_objs.transform.GetChild(i).gameObject.activeSelf)
                {
                    return;
                }
            }
            if (_seq == null)
            {
                Fade(0, _dieTime);
            }
        }
    }

    public void EnemySetting()
    {
        _seq = null;
        for (int i = 0; i < _objs.transform.childCount; i++)
        {
            _objs.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    IEnumerator EnemySpawn()
    {
        _onEnemy = true;
        for (int i = 0; i < _objs.transform.childCount; i++)
        {
            GameObject obj = _objs.transform.GetChild(i).gameObject;
            obj.SetActive(true);
            PoolManager.Instance.Pop("SpawnEffect", obj.transform.position, 
                new Vector3(Mathf.Abs(obj.transform.localScale.x), Mathf.Abs(obj.transform.localScale.y), Mathf.Abs(obj.transform.localScale.z)) * 1.2f, 
                new Vector3(90, 0, 0));
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    public void Fade(float value, float time)
    {
        if (_seq != null && _seq.IsActive()) _seq.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(_tmp.DOFade(value, time).SetEase(Ease.InSine));
        if (value == 0)
            _seq.AppendCallback(() => Clear());
    }

    private void Clear()
    {
        gameObject.SetActive(false);
        _clear = true;
    }

    public void ReSet()
    {
        for (int i = 0; i < _objs.transform.childCount; i++)
        {
            _objs.transform.GetChild(i).GetComponent<EnemyController>().ReSet();
        }
    }
    public void Set()
    {
        EnemySetting();
        _on = transform.name == "Wall_1";
        _clear = false;
        _onEnemy = false;
        _tmp.color = Color.red;
    }
}
