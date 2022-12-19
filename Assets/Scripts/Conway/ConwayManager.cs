using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class ConwayManager : MonoBehaviour
{
    [SerializeField] private ComputeShader ConwayShader;
    private RenderTexture renderTexture;

    // Modifyables
    [SerializeField] private bool draw = true;
    [SerializeField] private float brushSize = 1f;

    [Range(0.1f, 5f)]
    [SerializeField] private float timeScale = 1f;

    private bool _drawCache;
    private float _fixedDeltaTime;

    // Constants and extras
    private const FilterMode defaultFilterMode = FilterMode.Point;
    private const GraphicsFormat defaultGraphicsFormat = GraphicsFormat.R16G16B16A16_SFloat; //GraphicsFormat.R8G8B8A8_UNorm;

    public void Awake()
    {
        _fixedDeltaTime = Time.fixedDeltaTime;
    }

    // Start is called before the first frame update
    public void Start()
    {

        renderTexture = CreateRenderTexture(Screen.width, Screen.height);

        int[,] brushArray = new int[,] {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        };
        int[] brushShape = brushArray.Cast<int>().ToArray();

        string output = "";
        foreach (var x in brushShape)
        {
            output += x.ToString(); 
        }
        Debug.Log(output);

        int initKernel = ConwayShader.FindKernel("Init");
        ConwayShader.SetTexture(initKernel, "Conway", renderTexture);
        ConwayShader.SetInts("brushShape", brushShape);
        ConwayShader.SetInt("brushArrSize", 10) ;
        ConwayShader.SetInt("brushMax", brushShape.Length);
        ConwayShader.SetFloat("width", Screen.width);
        ConwayShader.SetFloat("height", Screen.height);
        // Set Draw to True and Cache it
        ConwayShader.SetBool("draw", draw);
        _drawCache = draw;
        ConwayShader.Dispatch(initKernel, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    // Create a Random Background textureunit
    public void Random()
    {
        int randKernel = ConwayShader.FindKernel("Random");
        ConwayShader.SetTexture(randKernel, "Conway", renderTexture);
        ConwayShader.Dispatch(randKernel, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int updateKernel = ConwayShader.FindKernel("Update");
        ConwayShader.SetTexture(updateKernel, "Conway", renderTexture);
        ConwayShader.SetFloat("time", Time.unscaledTime);
        // If mouse down and draw true
        if (Input.GetMouseButton(0) && draw)
        {
            // Send mouse co-ords and set to true
            ConwayShader.SetVector("mousePosition", Input.mousePosition);
            ConwayShader.SetBool("mouseDown", Input.GetMouseButton(0));
        }
        if (draw != _drawCache)
        {
            // Update draw if it changes
            ConwayShader.SetBool("draw", draw);
            _drawCache = draw;
        }
        ConwayShader.Dispatch(updateKernel, renderTexture.width / 8, renderTexture.height / 8, 1);

        Time.fixedDeltaTime = _fixedDeltaTime * timeScale;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // This renders directly to the camera (I think)
        Graphics.Blit(renderTexture, destination);
    }

    private RenderTexture CreateRenderTexture(int width, int height, FilterMode filterMode = defaultFilterMode, GraphicsFormat format = defaultGraphicsFormat)
    {
        RenderTexture texture = new RenderTexture(width, height, 0);
        texture.graphicsFormat = format;
        texture.enableRandomWrite = true;
        texture.autoGenerateMips = false;
        texture.Create();

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = filterMode;

        return texture;
    }

    public static Texture2D WriteArrayToTexture(float[,] arr)
    {
        Texture2D tex = new Texture2D(arr.GetLength(0), arr.GetLength(1));
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                tex.SetPixel(i, j, new Color(arr[i, j], 0f, 0f));


            }
        }
        tex.Apply();
        return tex;
    }

    public void Draw() { draw = true; }
    public void Play() { draw = false;  }
}
