using UnityEngine;

public class OutfitShop : MonoBehaviour
{
    public static OutfitShop Instance { get; private set; }

    private string[] outfitNames = new string[]
    {
        "Casual",
        "Business",
        "Gang Member",
        "Police Uniform",
        "Construction Worker",
        "Taxi Driver",
        "Rich Suit",
        "Homeless",
        "Athletic",
        "Rave Outfit"
    };

    private Color[] outfitColors = new Color[]
    {
        new Color(0.5f, 0.5f, 0.5f, 1f),    // Casual - Gray
        new Color(0.2f, 0.2f, 0.4f, 1f),    // Business - Navy
        new Color(0.6f, 0f, 0f, 1f),        // Gang Member - Red
        new Color(0f, 0f, 0.5f, 1f),         // Police - Blue
        new Color(0.8f, 0.6f, 0.2f, 1f),     // Construction - Orange
        new Color(1f, 1f, 0f, 1f),           // Taxi Driver - Yellow
        new Color(0.1f, 0.1f, 0.1f, 1f),     // Rich Suit - Black
        new Color(0.4f, 0.3f, 0.2f, 1f),     // Homeless - Brown
        new Color(0f, 0.8f, 0.4f, 1f),       // Athletic - Green
        new Color(1f, 0f, 1f, 1f)            // Rave - Magenta
    };

    private int currentOutfitIndex = 0;
    private float serviceCost = 200f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        currentOutfitIndex = 0;
    }

    public void ChangeOutfit(int outfitIndex, int playerId = 1)
    {
        if (outfitIndex < 0 || outfitIndex >= outfitNames.Length)
        {
            Debug.Log("Invalid outfit selection");
            return;
        }

        GameManager gm = GameManager.Instance;
        
        if (gm.CanAfford(serviceCost))
        {
            gm.AddMoney(-serviceCost);
            currentOutfitIndex = outfitIndex;
            
            if (playerId == 1)
            {
                PlayerController pc = PlayerController.Instance;
                if (pc != null)
                {
                    SpriteRenderer sr = pc.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = outfitColors[outfitIndex];
                    }
                }
            }
            else
            {
                Player2Controller p2c = Player2Controller.Instance;
                if (p2c != null)
                {
                    SpriteRenderer sr = p2c.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = outfitColors[outfitIndex];
                    }
                }
            }

            ClearWantedLevel(playerId);
            Debug.Log($"Player {playerId} changed outfit to {outfitNames[outfitIndex]} - Wanted level cleared! Cost: ${serviceCost}");
        }
        else
        {
            Debug.Log($"Cannot afford outfit change. Need: ${serviceCost}, Have: ${gm.Money}");
        }
    }

    public void ClearWantedLevel(int playerId = 1)
    {
        if (WantedLevelManager.Instance != null)
        {
            WantedLevelManager.Instance.ClearWantedLevel(playerId);
            Debug.Log($"Player {playerId} wanted level cleared!");
        }
        else
        {
            GameManager.Instance.WantedLevel = 0;
            Debug.Log("Wanted level cleared!");
        }
    }

    public string GetCurrentOutfit() => outfitNames[currentOutfitIndex];
    public string[] GetAllOutfits() => outfitNames;
    public Color GetCurrentOutfitColor() => outfitColors[currentOutfitIndex];
    public float GetServiceCost() => serviceCost;

    public void ShowOutfitMenu()
    {
        Debug.Log("=== OUTFIT SHOP ===");
        Debug.Log($"Cost: ${serviceCost}");
        Debug.Log("Enter outfit number to change (clears wanted level):");
        
        for (int i = 0; i < outfitNames.Length; i++)
        {
            string marker = (i == currentOutfitIndex) ? " [CURRENT]" : "";
            Debug.Log($"{i + 1}. {outfitNames[i]}{marker}");
        }
        Debug.Log("Press number key to select outfit");
    }
}