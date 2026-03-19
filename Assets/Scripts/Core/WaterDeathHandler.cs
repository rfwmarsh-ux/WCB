using UnityEngine;
using System.Collections;

public class WaterDeathHandler : MonoBehaviour
{
    public static WaterDeathHandler Instance { get; private set; }

    private GameObject wtfMessage;
    private bool isShowingMessage = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateWTFMessage();
    }

    private void CreateWTFMessage()
    {
        wtfMessage = new GameObject("WTF_Message");
        wtfMessage.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        wtfMessage.transform.SetParent(FindObjectOfType<Canvas>()?.transform);
        
        SpriteRenderer sr = wtfMessage.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = Color.white;
        wtfMessage.transform.localScale = new Vector3(200f, 100f, 1f);
        
        wtfMessage.SetActive(false);
    }

    public void CheckWaterDeath(Vector2 playerPosition, int playerId)
    {
        if (WaterManager.Instance == null) return;

        if (WaterManager.Instance.IsOverWater(playerPosition, out WaterBody water))
        {
            if (!water.IsSafeCanal())
            {
                Debug.Log($"Player {playerId} fell in {water.WaterName}!");
                HandleWaterDeath(playerId);
            }
        }
    }

    private void HandleWaterDeath(int playerId)
    {
        if (isShowingMessage) return;
        StartCoroutine(ShowWTFAndDie(playerId));
    }

    private IEnumerator ShowWTFAndDie(int playerId)
    {
        isShowingMessage = true;

        if (wtfMessage != null)
        {
            wtfMessage.SetActive(true);
        }

        Debug.Log("W T F !");

        yield return new WaitForSeconds(5f);

        if (wtfMessage != null)
        {
            wtfMessage.SetActive(false);
        }

        if (playerId == 1)
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            if (pm != null)
            {
                pm.Die();
            }
        }
        else
        {
            Player2Manager p2m = Player2Manager.Instance;
            if (p2m != null)
            {
                p2m.Die();
            }
        }

        isShowingMessage = false;
    }

    public void ForceShowWTF(Vector3 worldPosition)
    {
        if (wtfMessage != null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
                wtfMessage.transform.position = screenPos;
            }
            wtfMessage.SetActive(true);
        }
    }

    public void HideWTF()
    {
        if (wtfMessage != null)
        {
            wtfMessage.SetActive(false);
        }
    }
}
