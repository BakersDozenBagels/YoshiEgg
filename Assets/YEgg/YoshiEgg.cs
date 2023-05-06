using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class YoshiEgg : MonoBehaviour
{
    private EggMaterial.Shape[] _shapes;
    private bool _isSolved;
    private int _id = ++_idc, _answer;
    private static int _idc;
    private EggMaterial _egg;
    private static Dictionary<EggMaterial.Shape, Color> _colors;
    private Dictionary<EggMaterial.Shape, int> _values;
    private static Dictionary<EggMaterial.Shape, string> _colorNames;

    private void Start()
    {
        if (_colors == null)
        {
            _colors = new Dictionary<EggMaterial.Shape, Color>()
            {
                { EggMaterial.Shape.Circle, new Color(0f, 1f, 1f) },
                { EggMaterial.Shape.Teardrop, new Color(0f, 0f, 1f) },
                { EggMaterial.Shape.Triangle, new Color32(139, 69, 19, 255) },
                { EggMaterial.Shape.Square, new Color(0f, 1f, 0f) },
                { EggMaterial.Shape.Pentagon, new Color(1f, 0f, 0f) },
                { EggMaterial.Shape.Hexagon, new Color(0.5f, 0.5f, 0.5f) },
                { EggMaterial.Shape.Octagon, new Color32(222, 184, 135, 255) },
                { EggMaterial.Shape.Star, new Color(1f, 1f, 0f) },
                { EggMaterial.Shape.Moon, new Color32(138, 43, 226, 255) },
                { EggMaterial.Shape.Egg, new Color32(255, 165, 0, 255) },
                { EggMaterial.Shape.Heart, new Color32(255, 105, 180, 255) },
                { EggMaterial.Shape.Cross, new Color(0f, 0f, 0f) }
            };
        }
        if (_colorNames == null)
        {
            _colorNames = new Dictionary<EggMaterial.Shape, string>()
            {
                { EggMaterial.Shape.Circle, "Cyan" },
                { EggMaterial.Shape.Teardrop, "Blue" },
                { EggMaterial.Shape.Triangle, "Brown" },
                { EggMaterial.Shape.Square, "Green" },
                { EggMaterial.Shape.Pentagon, "Red" },
                { EggMaterial.Shape.Hexagon, "Gray" },
                { EggMaterial.Shape.Octagon, "Tan" },
                { EggMaterial.Shape.Star, "Yellow" },
                { EggMaterial.Shape.Moon, "Purple" },
                { EggMaterial.Shape.Egg, "Orange" },
                { EggMaterial.Shape.Heart, "Pink" },
                { EggMaterial.Shape.Cross, "Black" }
            };
        }
        _values = new Dictionary<EggMaterial.Shape, int>()
        {
            { EggMaterial.Shape.Circle, 6 },
            { EggMaterial.Shape.Teardrop, 3 },
            { EggMaterial.Shape.Triangle, 11 },
            { EggMaterial.Shape.Square, 1 },
            { EggMaterial.Shape.Pentagon, 2 },
            { EggMaterial.Shape.Hexagon, 10 },
            { EggMaterial.Shape.Octagon, 12 },
            { EggMaterial.Shape.Star, 4 },
            { EggMaterial.Shape.Moon, 7 },
            { EggMaterial.Shape.Egg, 8 },
            { EggMaterial.Shape.Heart, 5 },
            { EggMaterial.Shape.Cross, 9 }
        };
        MonoRandom rs = GetComponent<KMRuleSeedable>().GetRNG();
        if(rs.Seed != 1)
        {
            Log("Using ruleseed " + rs.Seed + ".");
            List<KeyValuePair<EggMaterial.Shape, int>> l = _values.ToList();
            rs.ShuffleFisherYates(l);
            _values = l.ToDictionary(k => k.Key, k => k.Value);
        }

        _shapes = Enumerable
            .Repeat(0, 7)
            .Select(_ => (EggMaterial.Shape)Random.Range(0, 12))
            .ToArray();
        _egg = GetComponentInChildren<EggMaterial>();
        int ans = _shapes.Select(s => _values[s]).Sum();
        _answer = ans % 60;
        StartCoroutine(Animate());
        Log("Colors shown: " + _shapes.Select(s => _colorNames[s]).Join(" "));
        Log("Expected answer: " + _answer + " (" + ans + ")");

        GetComponent<KMSelectable>().Children[0].OnInteract += () => { Egged(); return false; };
    }

    private void Egged()
    {
        if (_isSolved)
            return;
        int f = (int)GetComponent<KMBombInfo>().GetTime() % 60;
        Log("Pressed the egg when the timer reads " + f + ".");
        if (f == _answer)
        {
            Log("Correggct. Module solved!");
            GetComponent<KMAudio>().PlaySoundAtTransform("Solve", transform);
            GetComponent<KMBombModule>().HandlePass();
            _isSolved = true;
            StartCoroutine(FlyAway());
        }
        else
        {
            Log("Incorreggct. Strike!");
            GetComponent<KMAudio>().PlaySoundAtTransform("Strike", transform);
            GetComponent<KMBombModule>().HandleStrike();
        }
    }

    private IEnumerator FlyAway()
    {
        float t = Time.time;
        while (Time.time - t < 6f)
        {
            _egg.transform.localPosition = new Vector3(0f, Mathf.Lerp(0.06f, 3f, (Time.time - t) / 6f), -0.01f);
            yield return null;
        }
        _egg.transform.localScale = Vector3.zero;
        yield break;
    }

    private IEnumerator Animate()
    {
        int ix = 0;
        while (!_isSolved)
        {
            _egg.AnimateSDFColor(_shapes[ix], _colors[_shapes[ix]]);
            yield return new WaitForSeconds(2.4f);
            ix++;
            if (ix >= 7)
            {
                ix %= 7;
                yield return new WaitForSeconds(1.2f);
            }
        }
    }

    private void Log(string s)
    {
        Debug.Log("[Yoshi Egg #" + _id + "] " + s);
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} yoshi 37 [Presses the egg when there are 37 seconds on the timer]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        Match m = Regex.Match(command.Trim(), @"^\s*yoshi\s*([0-5]?\d)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (m.Success)
        {
            yield return null;

            int v = int.Parse(m.Groups[1].Value);
            while ((int)GetComponent<KMBombInfo>().GetTime() % 60 != v)
                yield return true;

            GetComponent<KMSelectable>().Children[0].OnInteract();
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        IEnumerator c = ProcessTwitchCommand("yoshi " + _answer);
        while (c.MoveNext())
            yield return c.Current;
    }
}
