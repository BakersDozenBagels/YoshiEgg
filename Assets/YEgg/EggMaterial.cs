using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EggMaterial : MonoBehaviour
{
    [SerializeField]
    private Material[] _shaders;

    public enum Shape
    {
        Circle,
        Teardrop,
        Triangle,
        Square,
        Pentagon,
        Hexagon,
        Octagon,
        Star,
        Moon,
        Egg,
        Heart,
        Cross
    }

    private Shader Shader(Shape s)
    {
        return _shaders[(int)s].shader;
    }

    private static Texture Default
    {
        get
        {
            if (_default == null)
                _default = new Texture2D(1, 1);
            return _default;
        }
    }
    private static Texture _default;


    private static Material Eggshell
    {
        get
        {
            if (_eggshell == null)
                _eggshell = new Material(UnityEngine.Shader.Find("Unlit/Color")) { color = new Color(0.94f, 0.9f, 0.86f) };
            return _eggshell;
        }
    }
    private static Material _eggshell;

    private Renderer _rend;
    private RenderTexture _output;
    private const int TextureSize = 1024;

    private void Start()
    {
        _rend = GetComponent<Renderer>();
        _output = new RenderTexture(TextureSize, TextureSize, 0);
        SetEggshell();
        _rend.material.mainTexture = _output;
    }

    private void SetEggshell()
    {
        Graphics.Blit(Default, _output, Eggshell);
    }

    private void OnDestroy()
    {
        if (_output != null)
            Destroy(_output);
    }

    private void SetSDFColor(Shader sdf, Color c, float[] pts, float scale)
    {
        if (sdf == null)
            return;
        Material m = new Material(sdf)
        {
            color = c
        };
        m.SetFloatArray("_Points", pts);
        m.SetFloat("_Scale", scale);
        Graphics.Blit(Default, _output, m);
    }

    private IEnumerator AnimateSDF(Shader sdf, Color c)
    {
        float[] pts = new float[] { .35f, .25f, .35f, .5f, .35f, .75f, .65f, .25f, .65f, .5f, .65f, .75f }
            .Select(f => f + Random.Range(-0.1f, 0.1f))
            .ToArray();

        float t = Time.time;
        while (Time.time - t < 1f)
        {
            SetSDFColor(sdf, c, pts, 0.1f * (Time.time - t));
            yield return null;
        }
        t = Time.time;
        while (Time.time - t < 1f)
        {
            SetSDFColor(sdf, c, pts, 0.1f * (1f - Time.time + t));
            yield return null;
        }
        SetEggshell();
    }

    public void AnimateSDFColor(Shape shape, Color c)
    {
        StartCoroutine(AnimateSDF(Shader(shape), c));
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            Test();
    }

    static int ix = 0;
    public static void Test()
    {
        FindObjectOfType<EggMaterial>().AnimateSDFColor((Shape)(ix++), Color.green);
        ix %= 12;
    }
#endif
}
