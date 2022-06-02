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
    private bool _waitFading;
    public IEnumerator ShowMessage(string msg, Color col, bool fade = false)
    {
        if (_showing)
            yield break;
        _showing = true;
        txt.text = msg;
        col.a = 0;
        img.color = col;
        txt.color = Color.black;
        _waitFading = fade;
        yield return FromFade();
    }

    public IEnumerator Hide() => Fade();

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
        var imgCol2 = img.color;
        var txtCol2 = txt.color;
        imgCol2.a = 1;
        txtCol2.a = 1;
        img.color = imgCol2;
        txt.color = txtCol2;
        _showing = false;
        if(_waitFading)
        {
            _waitFading = false;
            yield return new WaitForSeconds(2);
            yield return Fade();
        }
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

        var imgCol2 = img.color;
        var txtCol2 = txt.color;
        imgCol2.a = 0;
        txtCol2.a = 0;
        img.color = imgCol2;
        txt.color = txtCol2;
        _showing = false;
    }
}