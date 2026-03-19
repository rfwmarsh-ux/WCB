using UnityEngine;

public class LocationMarker : MonoBehaviour
{
public enum MarkerType
{
    Business,
    POI,
    MissionHub,
    Shop,
    OutfitShop,
    Church,
    Veterinary,
    BusStation,
    BusStop,
    MetroStation,
    MetroInterchange,
    TrainStation,
    Tunnel,
    PoliceStation,
    Bridge,
    Park,
    School,
    University,
    Residential,
    NatureReserve,
    Woodland,
    Industrial,
    Canal
}

    [SerializeField] public MarkerType Type;
    [SerializeField] public string LocationName;
    [SerializeField] public Color markerColor;

    public static void CreateMarker(Vector2 position, string name, MarkerType type, Color color)
    {
        GameObject marker = new GameObject($"Marker_{name}");
        marker.transform.position = (Vector3)position;

        SpriteRenderer sr = marker.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = color;
        sr.sortingOrder = 3;

        float size = type switch
        {
            MarkerType.MissionHub => 5f,
            MarkerType.OutfitShop => 6f,
            MarkerType.PoliceStation => 7f,
            MarkerType.Church => 8f,
            MarkerType.Bridge => 5f,
            MarkerType.Park => 10f,
            MarkerType.School => 8f,
            MarkerType.University => 12f,
            MarkerType.Residential => 15f,
            MarkerType.NatureReserve => 12f,
            MarkerType.Woodland => 10f,
            MarkerType.Industrial => 10f,
            _ => 4f
        };
        marker.transform.localScale = Vector3.one * size;

        CircleCollider2D collider = marker.AddComponent<CircleCollider2D>();
        collider.radius = size / 2;
        collider.isTrigger = true;
    }
}

public class MapMarkers : MonoBehaviour
{
    private void Start()
    {
        CreateAllMarkers();
    }

    private void CreateAllMarkers()
    {
        // Outfit Shops (Gold)
        LocationMarker.CreateMarker(new Vector2(200, 400), "Fashion House", LocationMarker.MarkerType.OutfitShop, new Color(0.9f, 0.6f, 0.2f, 1f));
        LocationMarker.CreateMarker(new Vector2(750, 650), "Style Studio", LocationMarker.MarkerType.OutfitShop, new Color(0.9f, 0.6f, 0.2f, 1f));

        // Mission Hubs (Purple)
        LocationMarker.CreateMarker(new Vector2(150, 480), "The Factory", LocationMarker.MarkerType.MissionHub, new Color(0.6f, 0.2f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(530, 530), "Crimson Cab", LocationMarker.MarkerType.MissionHub, new Color(0.8f, 0.2f, 0.4f, 1f));

        // Shops (Green)
        LocationMarker.CreateMarker(new Vector2(480, 520), "Chrome Wheels", LocationMarker.MarkerType.Business, new Color(0.3f, 0.7f, 0.3f, 1f));
        LocationMarker.CreateMarker(new Vector2(520, 480), "Neon Nights", LocationMarker.MarkerType.Business, new Color(0.8f, 0.2f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(780, 490), "Iron Fist Pawn", LocationMarker.MarkerType.Business, new Color(0.6f, 0.5f, 0.3f, 1f));
        LocationMarker.CreateMarker(new Vector2(180, 520), "Lucky's Electronics", LocationMarker.MarkerType.Business, new Color(0.3f, 0.3f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(480, 230), "Black Market", LocationMarker.MarkerType.Business, new Color(0.4f, 0.3f, 0.2f, 1f));

        // Church (White)
        LocationMarker.CreateMarker(new Vector2(500, 500), "Wolverhampton Central Church", LocationMarker.MarkerType.Church, new Color(0.9f, 0.85f, 0.7f, 1f));

        // Veterinary Centres (Light Green)
        LocationMarker.CreateMarker(new Vector2(500, 500), "Midland Veterinary Clinic", LocationMarker.MarkerType.Veterinary, new Color(0.5f, 0.9f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(800, 500), "Eastside Animal Hospital", LocationMarker.MarkerType.Veterinary, new Color(0.5f, 0.9f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(200, 500), "West End Pet Care", LocationMarker.MarkerType.Veterinary, new Color(0.5f, 0.9f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(500, 800), "Northside Emergency Vets", LocationMarker.MarkerType.Veterinary, new Color(0.5f, 0.9f, 0.5f, 1f));

        // Bus Station & Stops (Blue)
        LocationMarker.CreateMarker(new Vector2(480, 520), "Wolverhampton Bus Station", LocationMarker.MarkerType.BusStation, new Color(0.2f, 0.4f, 0.9f, 1f));
        LocationMarker.CreateMarker(new Vector2(520, 480), "City Centre Stop", LocationMarker.MarkerType.BusStop, new Color(0.3f, 0.5f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(350, 400), "Dudley Road", LocationMarker.MarkerType.BusStop, new Color(0.3f, 0.5f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(700, 400), "Wednesfield", LocationMarker.MarkerType.BusStop, new Color(0.3f, 0.5f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(600, 700), "Tettenhall", LocationMarker.MarkerType.BusStop, new Color(0.3f, 0.5f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(750, 600), "Pendeford", LocationMarker.MarkerType.BusStop, new Color(0.3f, 0.5f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(200, 650), "Bushbury", LocationMarker.MarkerType.BusStop, new Color(0.3f, 0.5f, 0.8f, 1f));

        // Metro Stations (Cyan/Teal)
        LocationMarker.CreateMarker(new Vector2(470, 540), "Wolverhampton Station", LocationMarker.MarkerType.MetroStation, new Color(0.8f, 0.2f, 0.2f, 1f));
        LocationMarker.CreateMarker(new Vector2(490, 510), "Pipers Row", LocationMarker.MarkerType.MetroStation, new Color(0.2f, 0.8f, 0.4f, 1f));
        LocationMarker.CreateMarker(new Vector2(510, 480), "Wolverhampton St George's", LocationMarker.MarkerType.MetroInterchange, new Color(0.2f, 0.6f, 0.9f, 1f));

        // Train Station (Brown)
        LocationMarker.CreateMarker(new Vector2(460, 550), "Wolverhampton Railway Station", LocationMarker.MarkerType.TrainStation, new Color(0.6f, 0.3f, 0.1f, 1f));

        // Points of Interest (Cyan)
        LocationMarker.CreateMarker(new Vector2(800, 530), "Wolverhampton Uni", LocationMarker.MarkerType.POI, new Color(0.2f, 0.8f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(180, 530), "West End High School", LocationMarker.MarkerType.POI, new Color(0.2f, 0.8f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(470, 790), "Primary School", LocationMarker.MarkerType.POI, new Color(0.2f, 0.8f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(770, 510), "Library", LocationMarker.MarkerType.POI, new Color(0.2f, 0.6f, 0.8f, 1f));
        LocationMarker.CreateMarker(new Vector2(450, 470), "City Centre Park", LocationMarker.MarkerType.POI, new Color(0.3f, 0.8f, 0.3f, 1f));
        LocationMarker.CreateMarker(new Vector2(200, 550), "West End Park", LocationMarker.MarkerType.POI, new Color(0.3f, 0.8f, 0.3f, 1f));

        // Police Stations (Dark Blue)
        LocationMarker.CreateMarker(new Vector2(480, 510), "Wolverhampton Central Police", LocationMarker.MarkerType.PoliceStation, new Color(0.1f, 0.2f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(720, 420), "Wednesfield Police Station", LocationMarker.MarkerType.PoliceStation, new Color(0.1f, 0.2f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(280, 320), "Bilston Police Station", LocationMarker.MarkerType.PoliceStation, new Color(0.1f, 0.2f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(620, 720), "Tettenhall Police Station", LocationMarker.MarkerType.PoliceStation, new Color(0.1f, 0.2f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(550, 750), "Heath Town Police Station", LocationMarker.MarkerType.PoliceStation, new Color(0.1f, 0.2f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(400, 650), "Low Hill Police Station", LocationMarker.MarkerType.PoliceStation, new Color(0.1f, 0.2f, 0.5f, 1f));

        // Bridges (Brown)
        LocationMarker.CreateMarker(new Vector2(380, 420), "Oxley Viaduct", LocationMarker.MarkerType.Bridge, new Color(0.4f, 0.25f, 0.15f, 1f));
        LocationMarker.CreateMarker(new Vector2(420, 380), "Dunstall Hill Viaduct", LocationMarker.MarkerType.Bridge, new Color(0.4f, 0.25f, 0.15f, 1f));
        LocationMarker.CreateMarker(new Vector2(300, 380), "Goldthorn Hill Viaduct", LocationMarker.MarkerType.Bridge, new Color(0.4f, 0.25f, 0.15f, 1f));
        LocationMarker.CreateMarker(new Vector2(180, 420), "Aldersley Railway Bridge", LocationMarker.MarkerType.Bridge, new Color(0.4f, 0.25f, 0.15f, 1f));
        LocationMarker.CreateMarker(new Vector2(480, 540), "Ring Road Bridge", LocationMarker.MarkerType.Bridge, new Color(0.5f, 0.5f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(490, 530), "Railway Drive Bridge", LocationMarker.MarkerType.Bridge, new Color(0.5f, 0.5f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(350, 320), "Wolverhampton Road West", LocationMarker.MarkerType.Bridge, new Color(0.5f, 0.5f, 0.5f, 1f));
        LocationMarker.CreateMarker(new Vector2(470, 545), "Station Footbridge", LocationMarker.MarkerType.Bridge, new Color(0.6f, 0.3f, 0.2f, 1f));
        LocationMarker.CreateMarker(new Vector2(650, 380), "Bentley Canal Bridge", LocationMarker.MarkerType.Bridge, new Color(0.5f, 0.35f, 0.2f, 1f));

        // Parks (Green)
        LocationMarker.CreateMarker(new Vector2(400, 550), "West Park", LocationMarker.MarkerType.Park, new Color(0.25f, 0.6f, 0.25f, 1f));
        LocationMarker.CreateMarker(new Vector2(600, 480), "East Park", LocationMarker.MarkerType.Park, new Color(0.25f, 0.6f, 0.25f, 1f));
        LocationMarker.CreateMarker(new Vector2(500, 750), "North Park", LocationMarker.MarkerType.Park, new Color(0.25f, 0.6f, 0.25f, 1f));
        LocationMarker.CreateMarker(new Vector2(350, 600), "Bantock Park", LocationMarker.MarkerType.Park, new Color(0.25f, 0.6f, 0.25f, 1f));
        LocationMarker.CreateMarker(new Vector2(280, 360), "Phoenix Park", LocationMarker.MarkerType.Park, new Color(0.25f, 0.6f, 0.25f, 1f));

        // Nature Reserves (Dark Green)
        LocationMarker.CreateMarker(new Vector2(380, 350), "Smestow Valley", LocationMarker.MarkerType.NatureReserve, new Color(0.15f, 0.45f, 0.15f, 1f));
        LocationMarker.CreateMarker(new Vector2(780, 620), "Pendeford Mill", LocationMarker.MarkerType.NatureReserve, new Color(0.15f, 0.45f, 0.15f, 1f));

        // Woodlands (Forest Green)
        LocationMarker.CreateMarker(new Vector2(420, 340), "Peascroft Wood", LocationMarker.MarkerType.Woodland, new Color(0.1f, 0.35f, 0.1f, 1f));
        LocationMarker.CreateMarker(new Vector2(630, 730), "Tettenhall Wood", LocationMarker.MarkerType.Woodland, new Color(0.1f, 0.35f, 0.1f, 1f));

        // University (Purple)
        LocationMarker.CreateMarker(new Vector2(800, 530), "Wolverhampton University", LocationMarker.MarkerType.University, new Color(0.4f, 0.3f, 0.6f, 1f));

        // Schools (Mauve)
        LocationMarker.CreateMarker(new Vector2(280, 450), "Ashmore Park School", LocationMarker.MarkerType.School, new Color(0.5f, 0.4f, 0.6f, 1f));
        LocationMarker.CreateMarker(new Vector2(640, 740), "Tettenhall Wood School", LocationMarker.MarkerType.School, new Color(0.5f, 0.4f, 0.6f, 1f));

        Debug.Log("Created location markers");
    }
}