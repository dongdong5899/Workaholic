using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    public int _clearWall = 0;

    private SpriteRenderer _sr;
    private Sequence _seq;
    private int _num = 0;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        MyNum();
    }


    private void MyNum()
    {
        for (int i = 0; i < GameManager.Instance._spawnPoint.transform.childCount; i++)
        {
            if (GameManager.Instance._spawnPoint.transform.GetChild(i).gameObject == gameObject)
            {
                _num = i;
            }
        }
    }

    public void Save()
    {
        if (GameManager.Instance._spawnNum != _num)
        {
            GameManager.Instance.Hp = 3;
            GameManager.Instance._spawnNum = _num;
            PlayerPrefs.SetInt("SpawnPoint", _num);
        }
        _sr.transform.eulerAngles = Vector3.zero;
        if (_seq != null && _seq.IsActive()) _seq.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(_sr.transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360).SetEase(Ease.OutExpo));
    }
}
