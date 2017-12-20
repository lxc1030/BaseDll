using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class UIAlertItem : MonoBehaviour
{
    // Fields
    public static Vector3 defaultFromPos = new Vector3(0, 100, 0);
    public static Vector3 defaultToPos = new Vector3(0, 250, 0);
    public static Vector3 defaultEndPos = new Vector3(0, 400, 0);
    public static float defaultTime = 2f;
    private float delayTime;
    private Vector3 endPos;
    private Vector3 fromPos;
    private OnAlertTipFinish mEvt;
    private float time;
    private Text tipLabel;
    private string tipStr;
    private Vector3 toPos;

    private void Awake()
    {
        tipLabel = GetComponent<Text>();
    }
    // Use this for initialization
    void Start () {
	
	}
    public void showTip(string tips)
    {
        this.tipStr = tips;
        this.time = defaultTime;
        this.delayTime = 0f;
        this.fromPos = defaultFromPos;
        this.toPos = defaultToPos;
        this.endPos = defaultEndPos;
        base.StartCoroutine("startAnim");
    }
    public void showTip(string tips, float delayTime, float time, Vector3 fromPos, Vector3 toPos, Vector3 endPos, OnAlertTipFinish evt)
    {
        this.tipStr = tips;
        this.time = time;
        this.delayTime = delayTime;
        this.fromPos = fromPos;
        this.toPos = toPos;
        this.endPos = endPos;
        this.mEvt = evt;
        this.tipLabel.text = this.tipStr;
        this.tipLabel.color = new Color(tipLabel.color.r, tipLabel.color.g, tipLabel.color.b, 0);
        this.tipLabel.transform.localPosition = fromPos;
        base.gameObject.SetActive(true);
        base.StartCoroutine("startAnim");
    }

    private IEnumerator startAnim()
    {
        yield return new WaitForSeconds(this.delayTime);
        transform.DOLocalMove(toPos, time).SetEase(Ease.Linear);
        this.tipLabel.DOColor(new Color(tipLabel.color.r, tipLabel.color.g, tipLabel.color.b, 1), 0.5f).SetEase(Ease.Linear);
        //TweenPosition.Begin(this.get_gameObject(), this.time / 2f, this.toPos);
        //TweenAlpha.Begin(this.get_gameObject(), 0.5f, 1f);
        yield return new WaitForSeconds(time);
        if (this.mEvt != null)
        {
            this.mEvt(this, true);
        }
        transform.DOLocalMove(endPos, 0.3f).SetEase(Ease.Linear);
        this.tipLabel.DOColor(new Color(tipLabel.color.r, tipLabel.color.g, tipLabel.color.b, 0), 0.3f).SetEase(Ease.Linear);
        //TweenPosition.Begin(this.get_gameObject(), 0.3f, this.endPos);
        //TweenAlpha.Begin(this.get_gameObject(), 0.3f, 0f);
        yield return new WaitForSeconds(0.3f);
        if (this.mEvt != null)
        {
            this.mEvt(this, false);
        }
        gameObject.SetActive(false);
    }
}
