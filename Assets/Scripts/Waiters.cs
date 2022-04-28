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

    public static IEnumerator LoopWhile(Func<bool> predicate, Action process, Action finish)
    {
        var delta = 0.5f;
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
}