using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Sprite : MonoBehaviour
{
	public const float DefaultCameraSize = 10f;

	public const float DefaultCameraHeight = 20f;

	public const float DefaultScreenHeight = 768f;

	[HideInInspector]
	public int m_textureWidth;

	[HideInInspector]
	public int m_textureHeight;

	public float m_scale = 1f;

	public int m_spriteWidth;

	public int m_spriteHeight;

	public int m_UVx;

	public int m_UVy;

	public int m_width = 16;

	public int m_height = 16;

	public int m_atlasGridSubdivisions = 16;

	public bool m_updateCollider = true;

	[HideInInspector]
	public Rect m_uvRect;

	protected Mesh m_spriteMesh;

	public Vector2 Size
	{
		get
		{
			return new Vector2(base.transform.localScale.x * (float)m_spriteWidth / 768f * 20f, base.transform.localScale.y * (float)m_spriteHeight / 768f * 20f);
		}
	}

	public Mesh SpriteMesh
	{
		get
		{
			return m_spriteMesh;
		}
	}

	private void Awake()
	{
		MeshFilter meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (!meshFilter.sharedMesh)
		{
			CreatePlane(meshFilter, m_spriteWidth, m_spriteHeight);
			MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
		}
	}

	public void RebuildMesh()
	{
		MeshFilter mf = GetComponent(typeof(MeshFilter)) as MeshFilter;
		CreatePlane(mf, m_spriteWidth, m_spriteHeight);
		MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
	}

	public void ResetSize()
	{
		m_textureWidth = m_width * base.GetComponent<Renderer>().sharedMaterial.mainTexture.width / m_atlasGridSubdivisions;
		m_textureHeight = m_height * base.GetComponent<Renderer>().sharedMaterial.mainTexture.height / m_atlasGridSubdivisions;
		m_spriteWidth = (int)(m_scale * (float)m_textureWidth);
		m_spriteHeight = (int)(m_scale * (float)m_textureHeight);
		RebuildMesh();
	}

	public void MapUVToTexture(int UVx, int UVy, int width, int height)
	{
		if (!(m_spriteMesh == null))
		{
			m_UVx = UVx;
			m_UVy = UVy;
			m_width = width;
			m_height = height;
			Vector3[] array = new Vector3[4]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			};
			Vector2[] array2 = new Vector2[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				float num = 0.5f * (1f / (float)m_atlasGridSubdivisions / (float)(1024 / m_atlasGridSubdivisions)) * array[i].x;
				float num2 = 0.5f * (1f / (float)m_atlasGridSubdivisions / (float)(1024 / m_atlasGridSubdivisions)) * array[i].y;
				float x = Mathf.Clamp(array[i].x, 0f, 1f) * (1f / (float)m_atlasGridSubdivisions) * (float)m_width + (float)m_UVx * (1f / (float)m_atlasGridSubdivisions) - num;
				float y = Mathf.Clamp(array[i].y, 0f, 1f) * (1f / (float)m_atlasGridSubdivisions) * (float)m_height + (float)m_UVy * (1f / (float)m_atlasGridSubdivisions) - num2;
				array2[i] = new Vector2(x, y);
			}
			m_uvRect = new Rect(array2[0].x, array2[0].y, array2[2].x - array2[0].x, array2[2].y - array2[0].y);
			if ((bool)m_spriteMesh)
			{
				m_spriteMesh.uv = array2;
			}
		}
	}

	public void SetUVs(Vector2[] newUVs)
	{
		m_spriteMesh.uv = newUVs;
	}

	public void CreatePlane(MeshFilter mf, int width, int height)
	{
		float num = (float)width * 10f / 768f;
		float num2 = (float)height * 10f / 768f;
		Mesh mesh = new Mesh();
		mesh.name = "GeneratedMesh_" + width + "x" + height;
		Vector3[] array = new Vector3[4];
		int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		array[0] = new Vector3(0f - num, 0f - num2, 0f);
		array[1] = new Vector3(0f - num, num2, 0f);
		array[2] = new Vector3(num, num2, 0f);
		array[3] = new Vector3(num, 0f - num2, 0f);
		mesh.vertices = array;
		mesh.triangles = triangles;
		mf.sharedMesh = mesh;
		m_spriteMesh = mf.sharedMesh;
		mf.sharedMesh.RecalculateNormals();
		mf.sharedMesh.RecalculateBounds();
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}
		MeshFilter component = base.transform.GetComponent<MeshFilter>();
		if ((bool)component && (bool)base.GetComponent<Renderer>().sharedMaterial && (bool)base.GetComponent<Renderer>().sharedMaterial.mainTexture)
		{
			m_textureWidth = m_width * base.GetComponent<Renderer>().sharedMaterial.mainTexture.width / m_atlasGridSubdivisions;
			m_textureHeight = m_height * base.GetComponent<Renderer>().sharedMaterial.mainTexture.height / m_atlasGridSubdivisions;
			if (m_spriteWidth == 0)
			{
				m_spriteWidth = (int)(m_scale * (float)m_textureWidth);
			}
			if (m_spriteHeight == 0)
			{
				m_spriteHeight = (int)(m_scale * (float)m_textureHeight);
			}
			if (!component.sharedMesh)
			{
				CreatePlane(component, m_spriteWidth, m_spriteHeight);
				MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
			}
		}
	}
}
