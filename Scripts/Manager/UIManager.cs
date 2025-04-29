using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    static public UIManager Instance;


    [SerializeField]
    private GameObject _canvas;
    private Image _die;
    private Sequence _seq;

    private void Awake()
    {
        Instance = this;
        _die = _canvas.transform.Find("Die").GetComponent<Image>();
    }


    public void HpUIUpdate()
    {
        GameObject hpList = _canvas.transform.Find("HpPanel").gameObject;
        for (int i = 0; i < 3; i++)
        {
            if (i < GameManager.Instance.Hp) 
                hpList.transform.GetChild(i).gameObject.SetActive(true);
            else
                hpList.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Fade(float value, float time, Ease ease)
    {
        if (time == 0)
        {
            _die.color = new Color(0, 0, 0, value);
        }
        else
        {
            if (_seq != null && _seq.IsActive()) _seq.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(_die.DOFade(value, time).SetEase(ease));
        }
    }
}
