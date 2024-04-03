
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.EventSystems;

public class ConwayManager : MonoBehaviour
{
    [SerializeField] private ComputeShader ConwayShader;
    private RenderTexture renderTexture;

    [SerializeField] private GameObject drawArray;
    private DrawArrayManager drawArrayManager;

    // Modifiables
    private bool draw = true;

    private float timeScale {get; set;}

    private bool _drawCache;
    private float _fixedDeltaTime;

    private int[] brushShape;

    private RenderTexture InputTexture;

    // Constants and extras
    private const FilterMode defaultFilterMode = FilterMode.Point;
    private const GraphicsFormat defaultGraphicsFormat = GraphicsFormat.R16G16B16A16_SFloat; //GraphicsFormat.R8G8B8A8_UNorm;

    public void Awake()
    {
        _fixedDeltaTime = Time.fixedDeltaTime;
        SetTimeScale(1f);
    }

    // Start is called before the first frame update
    public void Start()
    {
        drawArrayManager = drawArray.GetComponent<DrawArrayManager>();
        renderTexture = CreateRenderTexture(Screen.width, Screen.height);
        InputTexture = CreateRenderTexture(Screen.width, Screen.height);

        brushShape = drawArrayManager.GetDefaultBrush();

        int initKernel = ConwayShader.FindKernel("Init");
        ConwayShader.SetTexture(initKernel, "Conway", renderTexture);
        ConwayShader.SetInt("brushArrSize", 10);
        ConwayShader.SetInts("brushShape", brushShape);
        ConwayShader.SetInt("brushMax", brushShape.Length);
        ConwayShader.SetFloat("width", Screen.width);
        ConwayShader.SetFloat("height", Screen.height);
        // Set Draw to True and Cache it
        ConwayShader.SetBool("draw", draw);
        _drawCache = draw;
        ConwayShader.Dispatch(initKernel, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        int updateKernel = ConwayShader.FindKernel("Update");
        ConwayShader.SetTexture(updateKernel, "Conway", renderTexture);
        ConwayShader.SetFloat("time", Time.unscaledTime);
        ConwayShader.SetInts("brushShape", brushShape);
        
        // If mouse down and draw true
        if (!pointerOverUI && Input.GetMouseButton(0) && draw)
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

    // Create a Random Background texture
    public void Random()
    {
        int randKernel = ConwayShader.FindKernel("Random");
        ConwayShader.SetTexture(randKernel, "Conway", renderTexture);
        ConwayShader.Dispatch(randKernel, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    public void Draw() { draw = true; }
    public void Play() { draw = false;  }
    public void SetTimeScale(float sliderValue) { 
        timeScale = sliderValue; 
    }

    public void SetBrush(int[] brush) {
        brushShape = brush;
    } 
} 