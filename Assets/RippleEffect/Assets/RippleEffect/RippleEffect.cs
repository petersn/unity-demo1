using UnityEngine;
using System.Collections;

public class RippleEffect : MonoBehaviour
{
    public AnimationCurve waveform = new AnimationCurve(
        new Keyframe(0.00f, 0.50f, 0, 0),
        new Keyframe(0.05f, 1.00f, 0, 0),
        new Keyframe(0.15f, 0.10f, 0, 0),
        new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),
        new Keyframe(0.45f, 0.60f, 0, 0),
        new Keyframe(0.55f, 0.40f, 0, 0),
        new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),
        new Keyframe(0.85f, 0.52f, 0, 0),
        new Keyframe(0.99f, 0.50f, 0, 0)
    );

    [Range(0.01f, 1.0f)]
    public float refractionStrength = 0.5f;

    public Color reflectionColor = Color.gray;

    [Range(0.0f, 1.0f)]
    public float reflectionStrength = 0.7f;

    [Range(1.0f, 3.0f)]
    public float waveSpeed = 1.25f;

	public float scaleFactor = 1.0f;

    [SerializeField, HideInInspector]
    Shader shader;

	public Vector2 pinchCenter = new Vector2(0, 0);
	public float pinchPhase = 0;

    class Droplet
    {
        Vector3 position;
        float time;

        public Droplet()
        {
            time = 1000;
        }

        public void Reset(Vector3 _position)
        {
            position = _position;
            time = 0;
        }

        public void Update()
        {
            time += Time.deltaTime;
        }

        public Vector4 MakeShaderParameter(Camera c)
        {
        	// Our shader expects the x and y coordinates to be in the following units:
        	// x should vary in the interval [0, c.aspect] to cover the whole screen.
        	// y should vary in the interval [0, 1] to cover the whole screen.
        	// z should encode the time.
        	// We convert our real world position into this coordinate space:
        	Vector3 viewportCoordinates = c.WorldToViewportPoint(position);
            return new Vector4(
            	viewportCoordinates.x * c.aspect,
            	viewportCoordinates.y,
            	time,
            	0
            );
        }
    }

    Droplet[] droplets;
    Texture2D gradTexture;
    Material material;
    float timer;
    int dropCount;

	void UpdatePinchParameters() {
		var c = GetComponent<Camera>();

		float p1 = Mathf.Exp(-5.0f * pinchPhase);
		float p2 = 100.0f / (1f + Mathf.Exp(5.0f * pinchPhase)); //10f - phase * 9f;
		float effectScale = 1.0f - Mathf.Exp(-10f * pinchPhase); //1.0f;
		float vignetteScale = -pinchPhase * 2.0f;
		float vignetteOffset = 1.0f - Mathf.Exp(-10f * (1f - pinchPhase));

       	Vector3 viewportCoordinates = c.WorldToViewportPoint(pinchCenter);
		material.SetVector("_PinchParams1", new Vector4(
			viewportCoordinates.x * c.aspect,
			viewportCoordinates.y,
			p1,
			p2
		));
		material.SetVector("_PinchParams2", new Vector4(
			effectScale,
			vignetteScale,
			vignetteOffset,
			0
		));
	}

    void UpdateShaderParameters()
    {
        var c = GetComponent<Camera>();

        material.SetVector("_Drop1", droplets[0].MakeShaderParameter(c));
        material.SetVector("_Drop2", droplets[1].MakeShaderParameter(c));
        material.SetVector("_Drop3", droplets[2].MakeShaderParameter(c));

        material.SetColor("_Reflection", reflectionColor);
        material.SetVector("_Params1", new Vector4(c.aspect, 1, 1 / scaleFactor, 0));
        material.SetVector("_Params2", new Vector4(1, 1 / c.aspect, refractionStrength, reflectionStrength));

		UpdatePinchParameters();
	}

	void Awake()
	{
        droplets = new Droplet[3];
        droplets[0] = new Droplet();
        droplets[1] = new Droplet();
        droplets[2] = new Droplet();

        gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
        gradTexture.wrapMode = TextureWrapMode.Clamp;
        gradTexture.filterMode = FilterMode.Bilinear;
        for (var i = 0; i < gradTexture.width; i++)
        {
            var x = 1.0f / gradTexture.width * i;
            var a = waveform.Evaluate(x);
            gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }
        gradTexture.Apply();

        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        material.SetTexture("_GradTex", gradTexture);

        UpdateShaderParameters();
    }

    void Update()
    {
        foreach (var d in droplets) d.Update();

        UpdateShaderParameters();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    public void Emit(Vector3 position)
    {
        droplets[dropCount++ % droplets.Length].Reset(position);
    }

	public void SetPinch(Vector2 _pinchCenter, float _pinchPhase) {
		pinchCenter = _pinchCenter;
		pinchPhase = Mathf.Max(0, Mathf.Min(1, _pinchPhase));
	}
}
