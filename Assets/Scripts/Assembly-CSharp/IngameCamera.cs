using System.Collections;
using UnityEngine;

public class IngameCamera : WPFMonoBehaviour
{
	protected enum CameraState
	{
		Follow = 0,
		Pan = 1,
		Zoom = 2,
		Wait = 3,
		Building = 4,
		Preview = 5
	}

	private const float ShowNextTargetCameraDelay = 2f;

	private const bool ZoomToNextBoxWhenCompleted = false;

	protected Vector3 m_integratedVelocity;

	protected float m_zoomFactor;

	protected float m_speed = 1f;

	private LevelManager.CameraLimits m_cameraLimits;

	private CameraPreview m_cameraPreview;

	private Transform m_cameraTargetObj;

	private Vector3 currentPos;

	private float currentFOV;

	private float CAMERA_MAX_ZOOM = 7f;

	private float CAMERA_PREVIEW_ZOOM = 9f;

	protected CameraState m_state;

	private Vector3[] m_touches = new Vector3[2];

	private float m_transitionTimer;

	private void Start()
	{
		if (BuildCustomizationLoader.Instance.IsHDVersion)
		{
			CAMERA_MAX_ZOOM = 10f;
		}
		else
		{
			CAMERA_MAX_ZOOM = 7f;
		}
		base.GetComponent<Camera>().orthographic = true;
		m_cameraLimits = WPFMonoBehaviour.levelManager.m_cameraLimits;
		WPFMonoBehaviour.levelManager.m_cameraMaxZoom = Mathf.Min(m_cameraLimits.size.x, m_cameraLimits.size.y) / 2f;
		WPFMonoBehaviour.levelManager.m_cameraMinZoom = 5f;
		m_cameraPreview = GetComponent<CameraPreview>();
		if (m_cameraPreview == null && WPFMonoBehaviour.levelManager.GoalPosition != null)
		{
			m_cameraPreview = base.gameObject.AddComponent<CameraPreview>();
			CameraPreview.CameraControlPoint cameraControlPoint = new CameraPreview.CameraControlPoint();
			cameraControlPoint.position = WPFMonoBehaviour.levelManager.GoalPosition.transform.position + WPFMonoBehaviour.levelManager.m_previewOffset;
			cameraControlPoint.zoom = WPFMonoBehaviour.levelManager.m_previewOffset.z / 2f;
			m_cameraPreview.m_controlPoints.Add(cameraControlPoint);
			m_cameraPreview.m_controlPoints.Add(cameraControlPoint);
			CameraPreview.CameraControlPoint cameraControlPoint2 = new CameraPreview.CameraControlPoint();
			cameraControlPoint2.position = new Vector2(WPFMonoBehaviour.levelManager.m_cameraLimits.topLeft.x + WPFMonoBehaviour.levelManager.m_cameraLimits.size.x / 2f, WPFMonoBehaviour.levelManager.m_cameraLimits.topLeft.y - WPFMonoBehaviour.levelManager.m_cameraLimits.size.y / 2f);
			cameraControlPoint2.zoom = WPFMonoBehaviour.levelManager.m_cameraMaxZoom;
			m_cameraPreview.m_controlPoints.Add(cameraControlPoint2);
			CameraPreview.CameraControlPoint cameraControlPoint3 = new CameraPreview.CameraControlPoint();
			cameraControlPoint3.position = WPFMonoBehaviour.levelManager.StartingPosition.transform.position + WPFMonoBehaviour.levelManager.m_constructionOffset;
			cameraControlPoint3.zoom = WPFMonoBehaviour.levelManager.m_constructionOffset.z;
			m_cameraPreview.m_controlPoints.Add(cameraControlPoint3);
			m_cameraPreview.m_controlPoints.Add(cameraControlPoint3);
			m_cameraPreview.m_animationTime = WPFMonoBehaviour.levelManager.m_previewMoveTime;
			if (m_cameraPreview.m_animationTime < 1f)
			{
				m_cameraPreview.m_animationTime = 1f;
			}
		}
		if (m_cameraPreview != null)
		{
			WPFMonoBehaviour.levelManager.m_previewMoveTime = m_cameraPreview.m_animationTime;
		}
		base.gameObject.SetActiveRecursively(true);
	}

	private void Update()
	{
		UpdatePosition();
		ProcessInput();
	}

	private IEnumerator EnablePreviewMode()
	{
		while ((double)(CAMERA_PREVIEW_ZOOM - currentFOV) > 0.1)
		{
			float ortoPreviewDelta = CAMERA_PREVIEW_ZOOM - currentFOV;
			currentFOV += ortoPreviewDelta * GameTime.RealTimeDelta * 4f;
			base.GetComponent<Camera>().orthographicSize = currentFOV;
			yield return new WaitForEndOfFrame();
		}
	}

	private void UpdateCameraPreview()
	{
		Vector3 vector = m_cameraPreview.m_controlPoints[0].position - (Vector2)currentPos;
		currentPos += vector * GameTime.RealTimeDelta / WPFMonoBehaviour.levelManager.m_previewMoveTime * 4f;
		float num = CAMERA_MAX_ZOOM - currentFOV;
		currentFOV += num * GameTime.RealTimeDelta * 2f;
		UpdateGameCamera(ref currentPos, ref currentFOV);
		IsCameraInLimits(ref currentPos, ref currentFOV);
		base.transform.position = currentPos;
		base.GetComponent<Camera>().orthographicSize = currentFOV;
	}

	private void ProcessInput()
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
		{
			if (m_state == CameraState.Wait)
			{
				if (m_transitionTimer > 0f)
				{
					m_transitionTimer -= GameTime.RealTimeDelta;
				}
				else
				{
					m_state = CameraState.Follow;
				}
			}
			DoPanning();
			if (Input.touchCount == 2 || Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				m_state = CameraState.Zoom;
			}
			if (((WPFMonoBehaviour.gameData.m_useTouchControls && Input.touchCount < 1) || (!Input.GetMouseButton(0) && Input.GetAxis("Mouse ScrollWheel") == 0f)) && m_state != 0)
			{
				m_state = CameraState.Wait;
			}
		}
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PreviewWhileBuilding || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PreviewWhileRunning)
		{
			DoPanning();
		}
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Building)
		{
			if (!DeviceInfo.Instance.UsesTouchInput)
			{
				if (Input.GetAxis("Mouse ScrollWheel") < -0.5f)
				{
					WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.PreviewWhileBuilding);
				}
			}
			else if (Input.touchCount >= 2)
			{
				Touch touch = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				Vector2 vector = touch.position - touch2.position;
				Vector2 vector2 = touch.position - touch.deltaPosition - (touch2.position - touch2.deltaPosition);
				float num = vector.magnitude - vector2.magnitude;
				if (num < -60f)
				{
					WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.PreviewWhileBuilding);
				}
			}
		}
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.PreviewWhileBuilding)
		{
			return;
		}
		if (!DeviceInfo.Instance.UsesTouchInput)
		{
			if (Input.GetAxis("Mouse ScrollWheel") > 0.5f)
			{
				StopCoroutine("EnablePreviewMode");
				WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
			}
		}
		else if (Input.touchCount >= 2)
		{
			Touch touch3 = Input.GetTouch(0);
			Touch touch4 = Input.GetTouch(1);
			Vector2 vector3 = touch3.position - touch4.position;
			Vector2 vector4 = touch3.position - touch3.deltaPosition - (touch4.position - touch4.deltaPosition);
			float num2 = vector3.magnitude - vector4.magnitude;
			if (num2 > 60f)
			{
				StopCoroutine("EnablePreviewMode");
				WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
			}
		}
	}

	private void UpdatePosition()
	{
		LevelManager.GameState gameState = LevelManager.GameState.Building;
		LevelManager levelManager = WPFMonoBehaviour.levelManager;
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			gameState = levelManager.gameState;
		}
		m_speed = 1f;
		currentPos = base.transform.position;
		currentFOV = base.GetComponent<Camera>().orthographicSize;
		switch (gameState)
		{
		case LevelManager.GameState.Running:
			if (!levelManager.contraptionRunning.m_cameraTarget || GameTime.RealTimeDelta == 0f)
			{
				return;
			}
			UpdateGameCamera(ref currentPos, ref currentFOV);
			currentFOV = Mathf.Clamp(currentFOV, 2f, CAMERA_MAX_ZOOM);
			break;
		case LevelManager.GameState.Completed:
			UpdateGameCamera(ref currentPos, ref currentFOV);
			currentFOV = Mathf.Clamp(currentFOV, 2f, CAMERA_MAX_ZOOM);
			break;
		case LevelManager.GameState.Preview:
			currentPos = m_cameraPreview.m_controlPoints[0].position;
			currentFOV = m_cameraPreview.m_controlPoints[0].zoom;
			break;
		case LevelManager.GameState.PreviewMoving:
			m_cameraPreview.UpdateCameraPreview(ref currentPos, ref currentFOV);
			if (m_cameraPreview.Done)
			{
				WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
			}
			break;
		case LevelManager.GameState.PreviewWhileBuilding:
			if (m_state == CameraState.Building)
			{
				m_state = CameraState.Preview;
				StartCoroutine("EnablePreviewMode");
			}
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.PreviewWhileRunning:
			m_state = CameraState.Preview;
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.Building:
			if (m_state != CameraState.Building)
			{
				StopCoroutine("EnablePreviewMode");
			}
			m_state = CameraState.Building;
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		}
		IsCameraInLimits(ref currentPos, ref currentFOV);
		base.transform.position = currentPos;
		base.GetComponent<Camera>().orthographicSize = currentFOV;
	}

	private void UpdateGameCamera(ref Vector3 currentPos, ref float currentFOV)
	{
		switch (m_state)
		{
		case CameraState.Building:
		{
			Vector3 vector4 = m_cameraPreview.m_controlPoints[m_cameraPreview.m_controlPoints.Count - 1].position - (Vector2)currentPos;
			currentPos += vector4 * GameTime.RealTimeDelta * 4f;
			float num3 = m_cameraPreview.m_controlPoints[m_cameraPreview.m_controlPoints.Count - 1].zoom - currentFOV;
			currentFOV += num3 * GameTime.RealTimeDelta * 4f;
			break;
		}
		case CameraState.Follow:
		{
			Vector3 vector3 = WPFMonoBehaviour.levelManager.contraptionRunning.m_cameraTarget.transform.position + Vector3.ClampMagnitude(WPFMonoBehaviour.levelManager.contraptionRunning.m_cameraTarget.GetComponent<Rigidbody>().velocity * 1.5f, 20f) - currentPos;
			currentPos += vector3 * GameTime.RealTimeDelta;
			float num2 = 8f + WPFMonoBehaviour.levelManager.contraptionRunning.m_cameraTarget.GetComponent<Rigidbody>().velocity.magnitude - currentFOV;
			currentFOV += num2 * GameTime.RealTimeDelta;
			break;
		}
		case CameraState.Pan:
		{
			Vector3 vector5 = currentPos;
			if (WPFMonoBehaviour.gameData.m_useTouchControls)
			{
				if (Input.touchCount < 1)
				{
					break;
				}
				currentPos += (Vector3)(-Input.GetTouch(0).deltaPosition) * GameTime.RealTimeDelta * (currentFOV / CAMERA_MAX_ZOOM);
			}
			else
			{
				currentPos += (WPFMonoBehaviour.ScreenToZ0(m_touches[0]) - WPFMonoBehaviour.ScreenToZ0(Input.mousePosition)) * 40f * GameTime.RealTimeDelta * (currentFOV / CAMERA_MAX_ZOOM);
				m_touches[0] = Input.mousePosition;
			}
			currentPos.z = vector5.z;
			break;
		}
		case CameraState.Zoom:
			if (WPFMonoBehaviour.gameData.m_useTouchControls)
			{
				if (Input.touchCount < 2)
				{
					break;
				}
				try
				{
					Touch touch = Input.GetTouch(0);
					Touch touch2 = Input.GetTouch(1);
					Vector2 vector = touch.position - touch2.position;
					Vector2 vector2 = touch.position - touch.deltaPosition - (touch2.position - touch2.deltaPosition);
					float num = vector.magnitude - vector2.magnitude;
					m_zoomFactor = (0f - num) * GameTime.RealTimeDelta;
				}
				catch
				{
				}
			}
			else
			{
				m_zoomFactor = (0f - Input.GetAxis("Mouse ScrollWheel")) * 48f * GameTime.RealTimeDelta;
			}
			m_speed = 100f;
			m_transitionTimer = 1f;
			currentFOV += m_zoomFactor;
			if (currentFOV > WPFMonoBehaviour.levelManager.m_cameraMaxZoom)
			{
				currentFOV = WPFMonoBehaviour.levelManager.m_cameraMaxZoom;
			}
			if (currentFOV < WPFMonoBehaviour.levelManager.m_cameraMinZoom)
			{
				currentFOV = WPFMonoBehaviour.levelManager.m_cameraMinZoom;
			}
			break;
		case CameraState.Wait:
			break;
		}
	}

	private void IsCameraInLimits(ref Vector3 newPos, ref float ortoSize)
	{
		bool flag = Screen.width > Screen.height;
		bool flag2 = m_cameraLimits.size.x > m_cameraLimits.size.y;
		float num = (float)Screen.width / (float)Screen.height;
		if (num > m_cameraLimits.size.x / m_cameraLimits.size.y)
		{
			num = m_cameraLimits.size.x / m_cameraLimits.size.y;
		}
		float num2 = 0f;
		num2 = (flag ? ((!flag2) ? (Mathf.Min(m_cameraLimits.size.x / 2f, m_cameraLimits.size.y / 2f) / num) : Mathf.Min(m_cameraLimits.size.x / 2f, m_cameraLimits.size.y / 2f)) : ((!flag2) ? (Mathf.Min(m_cameraLimits.size.x / 2f, m_cameraLimits.size.y / 2f) / num) : Mathf.Min(m_cameraLimits.size.x / 2f, m_cameraLimits.size.y / 2f)));
		if (ortoSize > num2)
		{
			ortoSize = num2;
		}
		float min = m_cameraLimits.topLeft.x + ortoSize * num;
		float max = m_cameraLimits.topLeft.x + m_cameraLimits.size.x - ortoSize * num;
		float max2 = m_cameraLimits.topLeft.y - base.GetComponent<Camera>().orthographicSize;
		float min2 = m_cameraLimits.topLeft.y - m_cameraLimits.size.y + ortoSize;
		newPos.x = Mathf.Clamp(newPos.x, min, max);
		newPos.y = Mathf.Clamp(newPos.y, min2, max2);
		newPos.z = -15f;
	}

	private void DoPanning()
	{
		if (WPFMonoBehaviour.gameData.m_useTouchControls)
		{
			if (Input.touchCount == 1)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began && m_state != CameraState.Preview)
				{
					m_touches[0] = Input.GetTouch(0).position;
				}
				if (Vector3.Distance(Input.GetTouch(0).position, m_touches[0]) > 30f)
				{
					m_transitionTimer = 1f;
					m_state = CameraState.Pan;
				}
				if (Input.GetTouch(0).phase == TouchPhase.Ended && m_state == CameraState.Pan)
				{
					m_state = CameraState.Wait;
				}
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_touches[0] = Input.mousePosition;
			}
			if (Vector3.Distance(Input.mousePosition, m_touches[0]) > 30f)
			{
				m_transitionTimer = 1f;
				m_state = CameraState.Pan;
			}
		}
		else if (Input.GetMouseButtonUp(0) && m_state == CameraState.Pan)
		{
			m_state = CameraState.Wait;
		}
	}
}
