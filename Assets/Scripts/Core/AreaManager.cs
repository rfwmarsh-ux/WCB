using UnityEngine;
using System.Collections.Generic;

public enum AreaType
{
    Park,
    NatureReserve,
    Woodland,
    School,
    UniversityCampus,
    Residential,
    Industrial,
    Canal,
    GolfCourse,
    Cemetery,
    SportsGround,
    Allotment,
    PlayingFields,
    OpenSpace
}

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance { get; private set; }

    private List<Area> areas = new List<Area>();

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
        CreateAllAreas();
    }

    private void CreateAllAreas()
    {
        CreateParksAndGardens();
        CreateNatureReserves();
        CreateWoodlands();
        CreateSchools();
        CreateUniversityCampus();
        CreateResidentialAreas();
        CreateIndustrialAreas();
        CreateCanals();
        CreateGolfCourses();
        CreateCemeteries();
        CreateSportsGrounds();
        CreateAllotments();
        CreateCanalAreas();

        Debug.Log($"Created {areas.Count} areas");
    }

    private void CreateParksAndGardens()
    {
        CreateArea("West Park", new Vector2(400, 550), AreaType.Park, 80f, 60f);
        CreateArea("Bantock Park", new Vector2(350, 600), AreaType.Park, 70f, 55f);
        CreateArea("East Park", new Vector2(600, 480), AreaType.Park, 75f, 55f);
        CreateArea("North Park", new Vector2(500, 750), AreaType.Park, 60f, 50f);
        CreateArea("St George's Park", new Vector2(520, 460), AreaType.Park, 40f, 35f);
        CreateArea("Graiseley Green", new Vector2(550, 560), AreaType.Park, 35f, 30f);
        CreateArea("Aldersley Park", new Vector2(180, 430), AreaType.Park, 45f, 40f);
        CreateArea("Castlecroft Park", new Vector2(250, 580), AreaType.Park, 40f, 35f);
        CreateArea("Finchfield Park", new Vector2(300, 520), AreaType.Park, 35f, 30f);
        CreateArea("Whitmore Reans Park", new Vector2(380, 480), AreaType.Park, 30f, 25f);
        CreateArea("Goldthorn Park", new Vector2(320, 400), AreaType.Park, 45f, 40f);
        CreateArea("Phoenix Park", new Vector2(280, 360), AreaType.Park, 50f, 40f);
        CreateArea("Fowlers Park", new Vector2(650, 450), AreaType.Park, 40f, 35f);
        CreateArea("Mount Pleasant Green", new Vector2(620, 700), AreaType.Park, 30f, 25f);
        CreateArea("Ashmore Park", new Vector2(280, 450), AreaType.Park, 55f, 45f);
        CreateArea("Graiseley Park", new Vector2(550, 560), AreaType.Park, 35f, 30f);
        CreateArea("Merry Hill Park", new Vector2(260, 290), AreaType.Park, 40f, 35f);
        CreateArea("Tettenhall Pool", new Vector2(630, 715), AreaType.Park, 25f, 20f);
        CreateArea("Wolverhampton Park", new Vector2(495, 505), AreaType.Park, 30f, 25f);
        CreateArea("Birchfield Park", new Vector2(700, 650), AreaType.Park, 35f, 30f);
        CreateArea("Moxley Park", new Vector2(200, 350), AreaType.Park, 30f, 25f);
        CreateArea("Dudley Road Park", new Vector2(360, 380), AreaType.Park, 35f, 30f);
        CreateArea(" Ettingshall Park", new Vector2(370, 340), AreaType.Park, 30f, 25f);
    }

    private void CreateNatureReserves()
    {
        CreateArea("Pendeford Mill Nature Reserve", new Vector2(780, 620), AreaType.NatureReserve, 60f, 50f);
        CreateArea("Smestow Valley Nature Reserve", new Vector2(380, 350), AreaType.NatureReserve, 120f, 35f);
        CreateArea("Bradmore Nature Area", new Vector2(700, 550), AreaType.NatureReserve, 40f, 35f);
        CreateArea("Ettingshall Marsh", new Vector2(380, 320), AreaType.NatureReserve, 40f, 35f);
        CreateArea("Wolverhampton Wetland Reserve", new Vector2(450, 400), AreaType.NatureReserve, 35f, 30f);
        CreateArea(" Bilston Brook Reserve", new Vector2(290, 360), AreaType.NatureReserve, 30f, 25f);
        CreateArea("Smestow Brook Reserve", new Vector2(350, 370), AreaType.NatureReserve, 25f, 20f);
        CreateArea("Horseley Fields Meadows", new Vector2(300, 390), AreaType.NatureReserve, 30f, 25f);
        CreateArea("Dunstall Park Nature", new Vector2(420, 430), AreaType.NatureReserve, 25f, 20f);
    }

    private void CreateWoodlands()
    {
        CreateArea("Peascroft Wood", new Vector2(420, 340), AreaType.Woodland, 45f, 40f);
        CreateArea("Horseley Fields Wood", new Vector2(300, 380), AreaType.Woodland, 35f, 30f);
        CreateArea("Gorsebrook Wood", new Vector2(550, 600), AreaType.Woodland, 40f, 35f);
        CreateArea("Dunstall Park Woods", new Vector2(420, 420), AreaType.Woodland, 30f, 25f);
        CreateArea("Wergs Wood", new Vector2(680, 720), AreaType.Woodland, 35f, 30f);
        CreateArea("Bradmore Woods", new Vector2(720, 580), AreaType.Woodland, 40f, 35f);
        CreateArea("Tettenhall Wood", new Vector2(630, 730), AreaType.Woodland, 45f, 40f);
        CreateArea("Aldersley Wood", new Vector2(170, 400), AreaType.Woodland, 35f, 30f);
    }

    private void CreateSchools()
    {
        CreateArea("West Park Primary School", new Vector2(410, 560), AreaType.School, 20f, 18f);
        CreateArea("St Bartholomew's Primary", new Vector2(480, 560), AreaType.School, 18f, 15f);
        CreateArea("Holy Trinity Catholic Primary", new Vector2(520, 560), AreaType.School, 18f, 15f);
        CreateArea("Spring Vale Primary School", new Vector2(450, 620), AreaType.School, 20f, 18f);
        CreateArea("East Park Academy", new Vector2(580, 500), AreaType.School, 22f, 18f);
        CreateArea("Hill Avenue Academy", new Vector2(550, 630), AreaType.School, 20f, 16f);
        CreateArea("Whitgreave Primary School", new Vector2(500, 430), AreaType.School, 18f, 15f);
        CreateArea("Lanesfield Primary School", new Vector2(550, 680), AreaType.School, 18f, 15f);
        CreateArea("Bushbury Hill Primary School", new Vector2(195, 640), AreaType.School, 20f, 16f);
        CreateArea("Bushbury Lane Academy", new Vector2(210, 670), AreaType.School, 22f, 18f);
        CreateArea("Castlecroft Primary School", new Vector2(260, 590), AreaType.School, 18f, 15f);
        CreateArea("Claregate Primary School", new Vector2(600, 710), AreaType.School, 18f, 15f);
        CreateArea("Christ Church Infants", new Vector2(640, 740), AreaType.School, 16f, 14f);
        CreateArea("Bantock Primary School", new Vector2(360, 610), AreaType.School, 18f, 15f);
        CreateArea("Dunstall Hill Primary School", new Vector2(390, 380), AreaType.School, 18f, 15f);
        CreateArea("St Mary's Primary School", new Vector2(310, 500), AreaType.School, 16f, 14f);
        CreateArea("Oak Meadow Primary School", new Vector2(400, 650), AreaType.School, 20f, 16f);
        CreateArea("Uplands Junior School", new Vector2(520, 600), AreaType.School, 18f, 15f);
        CreateArea("Penn Hall School", new Vector2(450, 680), AreaType.School, 25f, 20f);
        CreateArea("Berrybrook Primary School", new Vector2(350, 650), AreaType.School, 18f, 15f);
        CreateArea("Moseley Park", new Vector2(280, 340), AreaType.School, 30f, 25f);
        CreateArea("Colton Hills Community School", new Vector2(310, 410), AreaType.School, 28f, 22f);
        CreateArea("Aldersley High School", new Vector2(160, 450), AreaType.School, 30f, 25f);
        CreateArea("Coppice Performing Arts School", new Vector2(450, 370), AreaType.School, 25f, 20f);
        CreateArea("Ashmore Park Nursery", new Vector2(270, 460), AreaType.School, 15f, 12f);
        CreateArea("Bilston Church of England Primary", new Vector2(265, 330), AreaType.School, 18f, 15f);
        CreateArea("Broadmeadow Special School", new Vector2(370, 490), AreaType.School, 20f, 18f);
        CreateArea("Aurora Cedars School", new Vector2(690, 760), AreaType.School, 22f, 18f);
    }

    private void CreateUniversityCampus()
    {
        CreateArea("Wolverhampton University Main Campus", new Vector2(800, 530), AreaType.UniversityCampus, 60f, 50f);
        CreateArea("University Halls of Residence", new Vector2(815, 550), AreaType.UniversityCampus, 25f, 20f);
        CreateArea("City Campus", new Vector2(470, 530), AreaType.UniversityCampus, 35f, 30f);
        CreateArea("University Sports Ground", new Vector2(780, 570), AreaType.SportsGround, 30f, 25f);
    }

    private void CreateResidentialAreas()
    {
        CreateArea("Parkfield", new Vector2(440, 620), AreaType.Residential, 50f, 45f);
        CreateArea("Penn Fields", new Vector2(360, 620), AreaType.Residential, 45f, 40f);
        CreateArea("Underhill Estate", new Vector2(340, 660), AreaType.Residential, 40f, 35f);
        CreateArea("Bradmore", new Vector2(700, 560), AreaType.Residential, 50f, 45f);
        CreateArea("Tettenhall Wood", new Vector2(640, 750), AreaType.Residential, 55f, 50f);
        CreateArea("Tettenhall", new Vector2(610, 720), AreaType.Residential, 45f, 40f);
        CreateArea("Compton", new Vector2(700, 750), AreaType.Residential, 60f, 50f);
        CreateArea("Wergs", new Vector2(680, 730), AreaType.Residential, 40f, 35f);
        CreateArea("Codsall", new Vector2(150, 760), AreaType.Residential, 50f, 45f);
        CreateArea("Bilston South", new Vector2(290, 360), AreaType.Residential, 45f, 40f);
        CreateArea("Ettingshall", new Vector2(360, 340), AreaType.Residential, 40f, 35f);
        CreateArea("Coseley", new Vector2(320, 360), AreaType.Residential, 50f, 45f);
        CreateArea("Dudley Road Area", new Vector2(360, 380), AreaType.Residential, 40f, 35f);
        CreateArea("Chapel Ash", new Vector2(520, 570), AreaType.Residential, 35f, 30f);
        CreateArea("Lanesfield", new Vector2(560, 690), AreaType.Residential, 35f, 30f);
        CreateArea("Whitmore Reans", new Vector2(380, 500), AreaType.Residential, 45f, 40f);
        CreateArea("Finchfield", new Vector2(300, 530), AreaType.Residential, 40f, 35f);
        CreateArea("Castlecroft", new Vector2(250, 580), AreaType.Residential, 40f, 35f);
        CreateArea("Windy", new Vector2(380, 280), AreaType.Residential, 35f, 30f);
        CreateArea("Bushbury", new Vector2(200, 660), AreaType.Residential, 50f, 45f);
        CreateArea("Low Hill", new Vector2(400, 650), AreaType.Residential, 45f, 40f);
        CreateArea("Heath Town", new Vector2(540, 750), AreaType.Residential, 40f, 35f);
        CreateArea("Newbridge", new Vector2(580, 720), AreaType.Residential, 35f, 30f);
        CreateArea("Pendeford", new Vector2(750, 610), AreaType.Residential, 50f, 45f);
        CreateArea("Claverley", new Vector2(100, 400), AreaType.Residential, 30f, 25f);
        CreateArea("Bobville", new Vector2(150, 380), AreaType.Residential, 25f, 20f);
    }

    private void CreateIndustrialAreas()
    {
        CreateArea("Bilston Industrial Estate", new Vector2(270, 350), AreaType.Industrial, 45f, 35f);
        CreateArea("Wolverhampton Science Park", new Vector2(400, 420), AreaType.Industrial, 40f, 30f);
        CreateArea("Crossgate North", new Vector2(550, 420), AreaType.Industrial, 35f, 30f);
        CreateArea("M53 Business Park", new Vector2(250, 420), AreaType.Industrial, 30f, 25f);
        CreateArea("Stafford Road Industrial", new Vector2(350, 460), AreaType.Industrial, 35f, 30f);
    }

    private void CreateCanals()
    {
        CreateArea("Staffordshire Worcestershire Canal", new Vector2(350, 360), AreaType.Canal, 15f, 120f);
        CreateArea("Birmingham Canal Navigations", new Vector2(320, 340), AreaType.Canal, 12f, 80f);
        CreateArea("Wyrley Essington Canal", new Vector2(280, 370), AreaType.Canal, 10f, 60f);
    }

    private void CreateGolfCourses()
    {
        CreateArea("Wolverhampton Golf Club", new Vector2(250, 550), AreaType.GolfCourse, 45f, 40f);
        CreateArea("Oxley Golf Club", new Vector2(370, 400), AreaType.GolfCourse, 40f, 35f);
        CreateArea("South Staffordshire Golf Club", new Vector2(200, 350), AreaType.GolfCourse, 50f, 45f);
        CreateArea("Himley Park Golf", new Vector2(180, 300), AreaType.GolfCourse, 45f, 40f);
    }

    private void CreateCemeteries()
    {
        CreateArea("Wolverhampton Cemetery", new Vector2(520, 420), AreaType.Cemetery, 35f, 30f);
        CreateArea("Bushbury Cemetery", new Vector2(200, 630), AreaType.Cemetery, 30f, 25f);
        CreateArea("St Mary's Cemetery", new Vector2(330, 520), AreaType.Cemetery, 25f, 20f);
        CreateArea("Tettenhall Cemetery", new Vector2(600, 700), AreaType.Cemetery, 25f, 20f);
    }

    private void CreateSportsGrounds()
    {
        CreateArea("Molineux Stadium", new Vector2(480, 560), AreaType.SportsGround, 25f, 22f);
        CreateArea("Wolverhampton Racecourse", new Vector2(600, 580), AreaType.SportsGround, 60f, 50f);
        CreateArea("Aldersley Stadium", new Vector2(175, 440), AreaType.SportsGround, 35f, 30f);
        CreateArea("Crystal Gardens Sports", new Vector2(450, 480), AreaType.SportsGround, 25f, 20f);
    }

    private void CreateAllotments()
    {
        CreateArea("Parkfield Allotments", new Vector2(430, 610), AreaType.Allotment, 20f, 18f);
        CreateArea("Bilston Allotments", new Vector2(260, 320), AreaType.Allotment, 18f, 15f);
        CreateArea("Tettenhall Allotments", new Vector2(600, 690), AreaType.Allotment, 18f, 15f);
        CreateArea("Wednesfield Allotments", new Vector2(710, 440), AreaType.Allotment, 20f, 16f);
        CreateArea("Low Hill Allotments", new Vector2(400, 640), AreaType.Allotment, 18f, 15f);
        CreateArea("Pendeford Allotments", new Vector2(750, 600), AreaType.Allotment, 16f, 14f);
    }
    
    private void CreateCanalAreas()
    {
        CreateArea("Wyrley and Essington Canal", new Vector2(350, 360), AreaType.Canal, 15f, 200f);
        CreateArea("Birmingham Canal Main Line", new Vector2(320, 340), AreaType.Canal, 12f, 150f);
        CreateArea("Shropshire Canal", new Vector2(280, 370), AreaType.Canal, 10f, 80f);
        CreateArea("Staffordshire Canal", new Vector2(400, 340), AreaType.Canal, 12f, 100f);
        CreateArea("Canal Reservoir", new Vector2(430, 350), AreaType.Canal, 25f, 20f);
        CreateArea("Bilston Canal Basin", new Vector2(290, 360), AreaType.Canal, 20f, 15f);
        CreateArea("Ettingshall Canal", new Vector2(380, 330), AreaType.Canal, 15f, 60f);
    }

    private void CreateArea(string name, Vector2 position, AreaType type, float width, float height)
    {
        GameObject areaGO = new GameObject($"Area_{name}");
        areaGO.transform.position = (Vector3)position;

        Area area = areaGO.AddComponent<Area>();
        area.Initialize(name, position, type, width, height);

        areaGO.transform.parent = transform;
        areas.Add(area);
    }

    public List<Area> GetAllAreas() => areas;
    public List<Area> GetAreasOfType(AreaType type)
    {
        List<Area> result = new List<Area>();
        foreach (var area in areas)
        {
            if (area.AreaType == type)
                result.Add(area);
        }
        return result;
    }
}

public class Area : MonoBehaviour
{
    public string AreaName { get; private set; }
    public Vector2 Position { get; private set; }
    public AreaType AreaType { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    private SpriteRenderer spriteRenderer;

    public void Initialize(string name, Vector2 position, AreaType type, float width, float height)
    {
        AreaName = name;
        Position = position;
        AreaType = type;
        Width = width;
        Height = height;

        CreateAreaVisuals();
    }

    private void CreateAreaVisuals()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteHelper.GetDefaultSprite();
        spriteRenderer.sortingOrder = -5;

        transform.localScale = new Vector3(Width, Height, 1f);

        Color areaColor = GetAreaColor();
        spriteRenderer.color = areaColor;
    }

    private Color GetAreaColor()
    {
        return AreaType switch
        {
            AreaType.Park => new Color(0.25f, 0.55f, 0.25f, 0.85f),
            AreaType.NatureReserve => new Color(0.2f, 0.45f, 0.2f, 0.8f),
            AreaType.Woodland => new Color(0.15f, 0.35f, 0.15f, 0.9f),
            AreaType.School => new Color(0.4f, 0.35f, 0.5f, 0.7f),
            AreaType.UniversityCampus => new Color(0.35f, 0.3f, 0.55f, 0.75f),
            AreaType.Residential => new Color(0.35f, 0.32f, 0.28f, 0.6f),
            AreaType.Industrial => new Color(0.4f, 0.38f, 0.35f, 0.65f),
            AreaType.Canal => new Color(0.2f, 0.4f, 0.5f, 0.8f),
            AreaType.GolfCourse => new Color(0.3f, 0.5f, 0.25f, 0.85f),
            AreaType.Cemetery => new Color(0.25f, 0.3f, 0.25f, 0.7f),
            AreaType.SportsGround => new Color(0.3f, 0.5f, 0.3f, 0.75f),
            AreaType.Allotment => new Color(0.35f, 0.45f, 0.25f, 0.7f),
            AreaType.PlayingFields => new Color(0.28f, 0.5f, 0.28f, 0.75f),
            AreaType.OpenSpace => new Color(0.3f, 0.45f, 0.25f, 0.5f),
            _ => new Color(0.3f, 0.4f, 0.3f, 0.5f)
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 1f));
    }
}
