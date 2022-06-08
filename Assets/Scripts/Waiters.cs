using System;
using System.Collections;
using UnityEngine;

public class Waiters
{
    public static IEnumerator LoopFor(float seconds, Action process, Action finish)
    {
        var left = seconds;
        const int delta = 1;
        while (true)
        {
            process();
            yield return new WaitForSeconds(delta);
            left -= delta;
            if (!(left < 0))
                continue;
            finish();
            yield break;
        }
    }

    public static IEnumerator LoopFor(float seconds, Action finish)
    {
        yield return new WaitForSeconds(seconds);
        finish();
    }


    public static IEnumerator LoopWhile(Func<bool> predicate, Action process, Action finish)
    {
        const float delta = 0.5f;
        while (true)
        {
            if (!predicate())
            {
                process();
                yield return new WaitForSeconds(delta);
            }
            else
            {
                finish();
                yield break;
            }
        }
    }

    public static IEnumerable Move(float seconds, GameObject target, Vector3 destination, bool downscaling)
    {
        var delta = destination - target.GetComponent<RectTransform>().position;
        var left = seconds;
        
        while (left > 0)
        {
            Debug.Log(left);
            var dt = Time.deltaTime;
            left -= dt;
            target.GetComponent<RectTransform>().localPosition += delta * (dt * seconds);
            if (downscaling)
                target.GetComponent<RectTransform>().localScale = Vector3.one * (left / seconds);
            Debug.Log(target.GetComponent<RectTransform>().localPosition);
            yield return new WaitForSeconds(dt);
        }

    }
}