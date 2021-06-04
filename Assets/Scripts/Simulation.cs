using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public struct Agent
{
    public Vector2 position;
    public float angle;
}

public class Simulation : MonoBehaviour
{
    // Public Stuff
    public ComputeShader shader;

    // Private stuff
    private int width = 1920;
    private int height = 1080;

    private RenderTexture renderTexture;
    private RenderTexture dissolveTexture;

    private Agent[] agents;
    private int dataSize;

    // Buffers
    ComputeBuffer dataBuffer;

    // Constants and extras
    public const FilterMode defaultFilterMode = FilterMode.Bilinear;
    public const GraphicsFormat defaultGraphicsFormat = GraphicsFormat.R16G16B16A16_SFloat; //GraphicsFormat.R8G8B8A8_UNorm;

    private void Start()
    {
        Init();
        GameObject.FindGameObjectWithTag("Display").GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
        Debug.Log("STARTED");
    }

    private void Init()
    {
        // Init trail texture
        renderTexture = CreateRenderTexture(width, height);

        ////// Init dissolve texture
        dissolveTexture = CreateRenderTexture(width, height);

        // Init Agents
        agents = new Agent[VariableManager.Instance.numAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            Vector2 startPos = Vector2.zero;

            if (VariableManager.Instance.startShape == 0)
            {
                startPos = new Vector2(width / 2, height / 2);
            }
            else if (VariableManager.Instance.startShape == 1)
            {
                startPos = Random.insideUnitCircle * (width / 4) + new Vector2(width / 2, height / 2);
            }
            else if (VariableManager.Instance.startShape == 2)
            {
                startPos = new Vector2(Random.Range(width / 10, 9 * width / 10), Random.Range(height / 10, 9 * height / 10));
            }
            
            float startAngle = Random.value * Mathf.PI * 2;

            agents[i] = new Agent() { position = startPos, angle = startAngle };
        }

        Debug.Log(agents.Length);

        // Set texture vars
        shader.SetInt("width", renderTexture.width);
        shader.SetInt("height", renderTexture.height);

        // Agents
        shader.SetFloat("numAgents", VariableManager.Instance.numAgents);

        // Create Buffer
        dataSize = sizeof(float) * 3;
        dataBuffer = new ComputeBuffer(agents.Length, dataSize);
        dataBuffer.SetData(agents);
        shader.SetBuffer(0, "agents", dataBuffer);
    }

    private void LateUpdate()
    {
        // Assign texture
        shader.SetTexture(0, "output", renderTexture);
        shader.SetTexture(1, "output", renderTexture);
        shader.SetTexture(1, "dissolve", dissolveTexture);

        // Assign times
        shader.SetFloat("deltaTime", Time.fixedDeltaTime);
        shader.SetFloat("time", Time.fixedTime);

        // Assign agent settings for on the fly alteration
        shader.SetFloat("moveSpeed", VariableManager.Instance.moveSpeed);
        shader.SetFloat("turnSpeed", VariableManager.Instance.turnSpeed);
        shader.SetFloat("decay", VariableManager.Instance.decay);
        shader.SetFloat("diffuse", VariableManager.Instance.diffuse);
        shader.SetFloat("trailWeight", VariableManager.Instance.trailWeight);
        shader.SetFloat("sensorWidth", VariableManager.Instance.sensorWidth);
        shader.SetFloat("sensorDist", VariableManager.Instance.sensorDist);
        shader.SetInt("sensorSize", VariableManager.Instance.sensorSize);

        shader.Dispatch(0, agents.Length / 16, 1, 1);
        shader.Dispatch(1, width / 8, height / 8, 1);

        Graphics.Blit(dissolveTexture, renderTexture);
    }

    void OnDestroy()
    {
        dataBuffer.Release();
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

}

