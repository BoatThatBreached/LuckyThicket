using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Image img;
    public TMP_Text txt;
    private bool _showing;

    private const float MAXPassed = 0.5f;

    public void ShowMessage(string msg, Color col)
    {
        if (_showing)
            return;
        _showing = true;
        txt.text = msg;
        col.a = 0;
        img.color = col;
        txt.color = Color.black;
        StartCoroutine(FromFade());
    }

    private IEnumerator FromFade()
    {
        var passed = 0f;
        while (passed <= MAXPassed)
        {
            var ratio= passed / MAXPassed;
            var imgCol = img.color;
            var txtCol = txt.color;
            imgCol.a = ratio;
            txtCol.a = ratio;
            img.color = imgCol;
            txt.color = txtCol;
            var dt = Time.deltaTime;
            passed += dt;
            yield return new WaitForSeconds(dt);
        }

        yield return new WaitForSeconds(2);
        yield return Fade();
    }

    private IEnumerator Fade()
    {
        var passed = 0f;
        while (passed <= MAXPassed)
        {
            var ratio= 1-passed / MAXPassed;
            var imgCol = img.color;
            var txtCol = txt.color;
            imgCol.a = ratio;
            txtCol.a = ratio;
            img.color = imgCol;
            txt.color = txtCol;
            var dt = Time.deltaTime;
            passed += dt;
            yield return new WaitForSeconds(dt);
        }

        _showing = false;
    }
}