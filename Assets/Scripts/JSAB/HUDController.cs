using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image[] hpDots;
    public Image progressFill;
    public Image checkpointMark;
    public Text centerText;
    public Text subText;

    Coroutine flashRoutine;

    public void SetHp(int hp, int maxHp)
    {
        for (int i = 0; i < hpDots.Length; i++)
            if (hpDots[i] != null)
                hpDots[i].enabled = i < hp;
    }

    public void SetProgress(float t)
    {
        if (progressFill != null)
            progressFill.fillAmount = Mathf.Clamp01(t);
    }

    public void SetCheckpointMark(float t)
    {
        if (checkpointMark == null)
            return;
        RectTransform parent = checkpointMark.transform.parent as RectTransform;
        if (parent == null)
            return;
        float width = parent.rect.width;
        checkpointMark.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-width * 0.5f, width * 0.5f, t), 0f);
        checkpointMark.enabled = true;
    }

    public void FlashMessage(string message)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(message));
    }

    IEnumerator FlashRoutine(string message)
    {
        centerText.text = message;
        centerText.color = new Color(0.13f, 0.78f, 0.97f);
        yield return new WaitForSeconds(1.2f);
        centerText.text = "";
    }

    public void ShowEnd(bool win)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
        centerText.text = win ? "DESPERTASTE" : "EL SUEÑO TE ABSORBIÓ";
        centerText.color = win ? new Color(0.13f, 0.78f, 0.97f) : new Color(1f, 0.16f, 0.54f);
        subText.text = "PRESIONÁ R PARA REINTENTAR";
    }
}
