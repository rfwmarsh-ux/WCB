using UnityEngine;
using System.Collections.Generic;

public class BusinessManager : MonoBehaviour
{
    public static BusinessManager Instance { get; private set; }

    [SerializeField] private float shopRadius = 8f;

    private List<ShopData> allShops = new List<ShopData>();

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
        InitializeShops();
    }

    private void InitializeShops()
    {
        // === SUPERMARKETS (Wolverhampton Real Locations) ===
        CreateShop("Tesco Superstore", new Vector2(550, 470), ShopType.Supermarket);
        CreateShop("Tesco Express", new Vector2(490, 500), ShopType.Supermarket);
        CreateShop("Asda Superstore", new Vector2(450, 540), ShopType.Supermarket);
        CreateShop("Morrisons", new Vector2(530, 520), ShopType.Supermarket);
        CreateShop("Morrisons Daily", new Vector2(485, 485), ShopType.Supermarket);
        CreateShop("Sainsbury's", new Vector2(495, 495), ShopType.Supermarket);
        CreateShop("Sainsbury's Local", new Vector2(505, 510), ShopType.Supermarket);
        CreateShop("Aldi", new Vector2(420, 480), ShopType.Supermarket);
        CreateShop("Aldi Bilston", new Vector2(300, 320), ShopType.Supermarket);
        CreateShop("Lidl", new Vector2(320, 350), ShopType.Supermarket);
        CreateShop("Lidl Wednesfield", new Vector2(700, 410), ShopType.Supermarket);
        CreateShop("Co-op Food", new Vector2(510, 515), ShopType.ConvenienceStore);
        CreateShop("Co-op Bilston", new Vector2(280, 305), ShopType.ConvenienceStore);
        CreateShop("Iceland", new Vector2(515, 505), ShopType.Shop);
        CreateShop("Home Bargains", new Vector2(470, 530), ShopType.Shop);
        CreateShop("B&M Bargains", new Vector2(350, 360), ShopType.Shop);
        CreateShop("Poundland", new Vector2(290, 320), ShopType.Shop);
        CreateShop("Wilko", new Vector2(480, 495), ShopType.Shop);
        CreateShop("The Range", new Vector2(400, 450), ShopType.Shop);
        
        // === SPORTS CLUBS & VENUES ===
        CreateShop("Molineux Stadium", new Vector2(480, 560), ShopType.Stadium);
        CreateShop("Wolverhampton Racecourse", new Vector2(600, 580), ShopType.RacingTrack);
        CreateShop("Wolverhampton & Bilston Greyhound Stadium", new Vector2(280, 340), ShopType.RacingTrack);
        CreateShop("Wolverhampton Golf Club", new Vector2(250, 550), ShopType.GolfCourse);
        CreateShop("Oxley Golf Club", new Vector2(370, 400), ShopType.GolfCourse);
        CreateShop("South Staffordshire Golf Club", new Vector2(200, 350), ShopType.GolfCourse);
        CreateShop("Aldersley Stadium", new Vector2(175, 440), ShopType.SportsClub);
        CreateShop("Crystal Gardens Sports Centre", new Vector2(450, 480), ShopType.SportsClub);
        CreateShop("Wolverhampton Swimming Centre", new Vector2(500, 580), ShopType.SwimmingPool);
        CreateShop("West Park Athletics", new Vector2(400, 555), ShopType.SportsClub);
        CreateShop("Wolverhampton Tennis Centre", new Vector2(200, 550), ShopType.TennisClub);
        CreateShop("Birchfield Sports Club", new Vector2(700, 650), ShopType.SportsClub);
        CreateShop("Dunstall Sports Ground", new Vector2(420, 420), ShopType.FootballPitch);
        CreateShop("City of Wolverhampton Rowing Club", new Vector2(350, 360), ShopType.SportsClub);
        
        // === SOCIAL CLUBS ===
        CreateShop("Wolverhampton Wanderers Social Club", new Vector2(485, 565), ShopType.SocialClub);
        CreateShop("Bilston Working Men's Club", new Vector2(265, 315), ShopType.SocialClub);
        CreateShop("Wednesfield Social Club", new Vector2(705, 425), ShopType.SocialClub);
        CreateShop("Tettenhall Working Men's Club", new Vector2(615, 705), ShopType.SocialClub);
        CreateShop("Penn Fields Social Club", new Vector2(360, 630), ShopType.SocialClub);
        CreateShop("Ettingshall Social Club", new Vector2(370, 340), ShopType.SocialClub);
        CreateShop("Bushbury Social Club", new Vector2(195, 665), ShopType.SocialClub);
        CreateShop("Merry Hill Bowls Club", new Vector2(260, 290), ShopType.SocialClub);
        
        // === CITY CENTRE SHOPS ===
        CreateShop("The Wulfrun Centre", new Vector2(500, 500), ShopType.ShoppingCentre);
        CreateShop("Dudley Street Shops", new Vector2(510, 490), ShopType.Shop);
        CreateShop("Mander Centre", new Vector2(495, 505), ShopType.ShoppingCentre);
        CreateShop("Primark", new Vector2(500, 495), ShopType.OutfitShop);
        CreateShop("New Look", new Vector2(495, 505), ShopType.OutfitShop);
        CreateShop("H&M", new Vector2(520, 510), ShopType.OutfitShop);
        CreateShop("Sports Direct", new Vector2(505, 490), ShopType.OutfitShop);
        CreateShop("Currys", new Vector2(530, 540), ShopType.Electronics);
        CreateShop("Argos", new Vector2(505, 480), ShopType.Electronics);
        CreateShop("Game", new Vector2(495, 510), ShopType.Electronics);
        
        // === RESTAURANTS & FAST FOOD ===
        CreateShop("Nando's", new Vector2(520, 530), ShopType.Restaurant);
        CreateShop("Pizza Express", new Vector2(480, 510), ShopType.Restaurant);
        CreateShop("Bella Italia", new Vector2(505, 495), ShopType.Restaurant);
        CreateShop("Café Rouge", new Vector2(490, 520), ShopType.Restaurant);
        CreateShop("McDonald's", new Vector2(485, 530), ShopType.FastFood);
        CreateShop("KFC", new Vector2(470, 505), ShopType.FastFood);
        CreateShop("Burger King", new Vector2(530, 475), ShopType.FastFood);
        CreateShop("Wendy's", new Vector2(525, 495), ShopType.FastFood);
        CreateShop("Grease Pit Diner", new Vector2(500, 540), ShopType.Restaurant);
        
        // === BARS & CLUBS ===
        CreateShop("Neon Nights Club", new Vector2(520, 480), ShopType.Club);
        CreateShop("Bank Nightclub", new Vector2(495, 485), ShopType.Club);
        CreateShop("Liquid", new Vector2(515, 520), ShopType.Club);
        CreateShop("Sky Bar", new Vector2(500, 510), ShopType.Bar);
        CreateShop("The Velvet Underground", new Vector2(800, 550), ShopType.Club);
        CreateShop("The Factory", new Vector2(150, 480), ShopType.Bar);
        CreateShop("Apex Club", new Vector2(530, 800), ShopType.Club);
        CreateShop("The Diamond", new Vector2(530, 220), ShopType.Club);
        CreateShop("Club V", new Vector2(490, 545), ShopType.Club);
        
        // === PUBS ===
        CreateShop("The Royal London", new Vector2(485, 515), ShopType.Pub);
        CreateShop("The Mount Tavern", new Vector2(520, 505), ShopType.Pub);
        CreateShop("The Old Still", new Vector2(475, 485), ShopType.Pub);
        CreateShop("The Wheatsheaf", new Vector2(505, 475), ShopType.Pub);
        CreateShop("The Newhampton", new Vector2(530, 515), ShopType.Pub);
        CreateShop("The Castle", new Vector2(460, 510), ShopType.Pub);
        CreateShop("The Bilston", new Vector2(270, 330), ShopType.Pub);
        CreateShop("The Red Lion", new Vector2(260, 300), ShopType.Pub);
        CreateShop("Cross Keys", new Vector2(290, 340), ShopType.Pub);
        CreateShop("The Elephant & Castle", new Vector2(690, 410), ShopType.Pub);
        CreateShop("The Mount", new Vector2(630, 720), ShopType.Pub);
        CreateShop("The Summerhouse", new Vector2(600, 690), ShopType.Pub);
        CreateShop("The Fox's Den", new Vector2(540, 750), ShopType.Pub);
        CreateShop("The Pendeford", new Vector2(760, 610), ShopType.Pub);
        CreateShop("The Bushbury", new Vector2(210, 660), ShopType.Pub);
        
        // === HOTELS ===
        CreateShop("The Wolverhampton Hotel", new Vector2(470, 520), ShopType.Hotel);
        CreateShop("Premier Inn", new Vector2(540, 480), ShopType.Hotel);
        CreateShop("Holiday Inn Express", new Vector2(460, 490), ShopType.Hotel);
        CreateShop("Mount Hotel", new Vector2(530, 550), ShopType.Hotel);
        
        // === BANKS ===
        CreateShop("HSBC", new Vector2(495, 480), ShopType.Bank);
        CreateShop("Lloyds Bank", new Vector2(510, 520), ShopType.Bank);
        CreateShop("NatWest", new Vector2(475, 495), ShopType.Bank);
        CreateShop("Barclays", new Vector2(485, 505), ShopType.Bank);
        CreateShop("Santander", new Vector2(505, 495), ShopType.Bank);
        
        // === CINEMAS & THEATRES ===
        CreateShop("Cineworld", new Vector2(540, 500), ShopType.Cinema);
        CreateShop("The Light Cinema", new Vector2(515, 485), ShopType.Cinema);
        CreateShop("Grand Theatre", new Vector2(490, 515), ShopType.Theatre);
        CreateShop("Wolverhampton Music Venue", new Vector2(505, 530), ShopType.Theatre);
        
        // === GARAGES & CAR DEALERS ===
        CreateShop("Chrome Wheels Auto", new Vector2(480, 520), ShopType.Garage);
        CreateShop("Crimson Cab Station", new Vector2(530, 530), ShopType.Garage);
        CreateShop("Rusty's Chop Shop", new Vector2(820, 510), ShopType.Garage);
        CreateShop("Speed Demons Motors", new Vector2(220, 490), ShopType.Garage);
        CreateShop("Midnight Garage", new Vector2(490, 820), ShopType.Garage);
        CreateShop("Thunder's Garage", new Vector2(490, 180), ShopType.Garage);
        CreateShop("Quick Fix Motors", new Vector2(350, 400), ShopType.Garage);
        CreateShop("Elite Auto Services", new Vector2(600, 700), ShopType.Garage);
        CreateShop("Wolverhampton Motors", new Vector2(450, 450), ShopType.CarDealer);
        CreateShop("Premium Cars Ltd", new Vector2(380, 420), ShopType.CarDealer);
        
        // === PETROL STATIONS ===
        CreateShop("Shell", new Vector2(350, 450), ShopType.PetrolStation);
        CreateShop("BP", new Vector2(320, 380), ShopType.PetrolStation);
        CreateShop("Tesco Petrol", new Vector2(540, 465), ShopType.PetrolStation);
        CreateShop("Sainsbury's Petrol", new Vector2(495, 495), ShopType.PetrolStation);
        CreateShop("Asda Petrol", new Vector2(445, 535), ShopType.PetrolStation);
        CreateShop("Morrisons Petrol", new Vector2(525, 525), ShopType.PetrolStation);
        CreateShop("Esso", new Vector2(280, 450), ShopType.PetrolStation);
        
        // === PHARMACIES ===
        CreateShop("Boots Pharmacy", new Vector2(500, 495), ShopType.Pharmacy);
        CreateShop("Superdrug", new Vector2(490, 505), ShopType.Pharmacy);
        CreateShop("Lloyds Pharmacy", new Vector2(520, 490), ShopType.Pharmacy);
        
        // === HAIR SALONS ===
        CreateShop("Supercuts", new Vector2(485, 520), ShopType.HairSalon);
        CreateShop(" Toni & Guy", new Vector2(510, 500), ShopType.HairSalon);
        
        // === GYMS ===
        CreateShop("PureGym", new Vector2(540, 510), ShopType.Gym);
        CreateShop("The Gym", new Vector2(495, 525), ShopType.Gym);
        CreateShop("Fit4Less", new Vector2(460, 500), ShopType.Gym);
        
        // === DIY STORES ===
        CreateShop("B&Q", new Vector2(400, 380), ShopType.DIY);
        CreateShop("Screwfix", new Vector2(450, 470), ShopType.DIY);
        
        // === POLICE & FIRE ===
        CreateShop("Wolverhampton Police Station", new Vector2(500, 545), ShopType.Police);
        CreateShop("Bilston Police Station", new Vector2(270, 325), ShopType.Police);
        CreateShop("Wolverhampton Fire Station", new Vector2(470, 530), ShopType.FireStation);
        CreateShop("Bilston Fire Station", new Vector2(270, 340), ShopType.FireStation);
        CreateShop("Wednesfield Fire Station", new Vector2(700, 440), ShopType.FireStation);
        
        // === HOSPITALS ===
        CreateShop("New Cross Hospital", new Vector2(580, 620), ShopType.Hospital);
        CreateShop("Royal Wolverhampton Hospital", new Vector2(520, 570), ShopType.Hospital);
        CreateShop("Bilston Medical Centre", new Vector2(285, 325), ShopType.Hospital);
        CreateShop("Wednesfield Medical", new Vector2(705, 405), ShopType.Hospital);
        
        // === VETERINARY CENTRES ===
        CreateShop("Wolverhampton Vet Centre", new Vector2(350, 480), ShopType.Vet);
        CreateShop("Bilston Vets", new Vector2(260, 300), ShopType.Vet);
        CreateShop("Wednesfield Vets", new Vector2(710, 415), ShopType.Vet);
        
        // === POST OFFICES ===
        CreateShop("Wolverhampton Post Office", new Vector2(495, 500), ShopType.PostOffice);
        CreateShop("Bilston Post Office", new Vector2(280, 305), ShopType.PostOffice);
        CreateShop("Wednesfield Post Office", new Vector2(705, 410), ShopType.PostOffice);
        CreateShop("Tettenhall Post Office", new Vector2(615, 705), ShopType.PostOffice);
        
        // === LIBRARIES ===
        CreateShop("Wolverhampton Central Library", new Vector2(770, 510), ShopType.Library);
        CreateShop("Bilston Library", new Vector2(275, 315), ShopType.Library);
        CreateShop("Wednesfield Library", new Vector2(710, 395), ShopType.Library);
        
        // === CHURCHES ===
        CreateShop("St George's Church", new Vector2(500, 520), ShopType.Church);
        CreateShop("St Mary's Church", new Vector2(310, 500), ShopType.Church);
        CreateShop("St Albans Church", new Vector2(260, 580), ShopType.Church);
        CreateShop("Holy Trinity Church", new Vector2(450, 420), ShopType.Church);
        CreateShop("Bilston Methodist Church", new Vector2(265, 320), ShopType.Church);
        
        // === SCHOOLS & EDUCATION ===
        CreateShop("West Park Primary", new Vector2(410, 560), ShopType.Education);
        CreateShop("St Bartholomew's Primary", new Vector2(480, 560), ShopType.Education);
        CreateShop("Bushbury Primary", new Vector2(195, 670), ShopType.Education);
        CreateShop("Castlecroft Primary", new Vector2(260, 590), ShopType.Education);
        CreateShop("Aldersley High", new Vector2(160, 450), ShopType.Education);
        CreateShop("City of Wolverhampton College", new Vector2(450, 520), ShopType.Education);
        CreateShop("Wolverhampton University", new Vector2(800, 530), ShopType.Education);
        CreateShop("Tettenhall Wood School", new Vector2(640, 730), ShopType.Education);
        
        // === MARKETS ===
        CreateShop("Wolverhampton Market", new Vector2(505, 510), ShopType.Market);
        CreateShop("Bilston Market", new Vector2(280, 310), ShopType.Market);
        CreateShop("Wednesfield Market", new Vector2(700, 400), ShopType.Market);
        
        // === CIVIC BUILDINGS ===
        CreateShop("Wolverhampton Civic Centre", new Vector2(520, 530), ShopType.Civic);
        CreateShop("Wolverhampton Crown Court", new Vector2(500, 545), ShopType.Civic);
        
        // === RAILWAY STATION ===
        CreateShop("Wolverhampton Railway Station", new Vector2(500, 575), ShopType.RailwayStation);
        CreateShop("Bilston Station", new Vector2(280, 370), ShopType.RailwayStation);
        
        // === METRO STOPS ===
        CreateShop("Wolverhampton Metro", new Vector2(495, 580), ShopType.MetroStop);
        CreateShop("Pipers Row Metro", new Vector2(490, 560), ShopType.MetroStop);
        CreateShop("St George's Metro", new Vector2(510, 545), ShopType.MetroStop);
        CreateShop("The Royal Metro", new Vector2(520, 530), ShopType.MetroStop);
        CreateShop("Priestfield Metro", new Vector2(530, 510), ShopType.MetroStop);
        CreateShop("Bilston Central Metro", new Vector2(300, 350), ShopType.MetroStop);
        CreateShop("Ettingshall Metro", new Vector2(370, 380), ShopType.MetroStop);
        CreateShop(" Mondeo Metro", new Vector2(400, 420), ShopType.MetroStop);
        
        // === CANAL BASINS & WHARFS ===
        CreateShop("Canal Wharf", new Vector2(420, 420), ShopType.CanalBasin);
        CreateShop("Chillington Basin", new Vector2(380, 360), ShopType.CanalBasin);
        CreateShop("Shrubbery Basin", new Vector2(400, 340), ShopType.CanalBasin);
        CreateShop("Commercial Wharf", new Vector2(360, 380), ShopType.CanalBasin);
        CreateShop("Pickford Wharf", new Vector2(340, 400), ShopType.CanalBasin);
        CreateShop("Canal Basin", new Vector2(450, 380), ShopType.CanalBasin);
        
        // === HISTORIC LANDMARKS ===
        CreateShop("St Peter's Church", new Vector2(505, 505), ShopType.Landmark);
        CreateShop("Old Town Hall", new Vector2(495, 520), ShopType.Landmark);
        CreateShop("Wolverhampton Art Gallery", new Vector2(490, 515), ShopType.Landmark);
        CreateShop("St Mary's Church", new Vector2(310, 500), ShopType.Church);
        CreateShop("St Albans Church", new Vector2(260, 580), ShopType.Church);
        CreateShop("Holy Trinity Church", new Vector2(450, 420), ShopType.Church);
        CreateShop("Bilston Methodist Church", new Vector2(265, 320), ShopType.Church);
        CreateShop("Tettenhall Church", new Vector2(620, 720), ShopType.Church);
        
        // === DEPARTMENT STORES ===
        CreateShop("Beatties", new Vector2(495, 510), ShopType.DepartmentStore);
        CreateShop("Frasers", new Vector2(500, 495), ShopType.DepartmentStore);
        CreateShop("Mander Centre", new Vector2(500, 500), ShopType.ShoppingCentre);
        
        // === SCIENCE PARK ===
        CreateShop("Wolverhampton Science Park", new Vector2(400, 420), ShopType.SciencePark);
        CreateShop("Innovation Centre", new Vector2(410, 430), ShopType.Industrial);
        
        // === ARENA THEATRE ===
        CreateShop("Arena Theatre", new Vector2(800, 520), ShopType.Theatre);
        CreateShop("The Light Cinema", new Vector2(515, 485), ShopType.Cinema);
        
        // === BOWLING CLUBS ===
        CreateShop("Merry Hill Bowls Club", new Vector2(260, 290), ShopType.BowlingClub);
        CreateShop("Tettenhall Bowling Club", new Vector2(625, 715), ShopType.BowlingClub);
        CreateShop("Bushbury Bowls Club", new Vector2(200, 650), ShopType.BowlingClub);
        CreateShop("Wolverhampton Bowls Centre", new Vector2(400, 555), ShopType.BowlingClub);
        
        // === MORE GOLF COURSES ===
        CreateShop("Wergs Golf Club", new Vector2(680, 720), ShopType.GolfCourse);
        CreateShop("Penn Golf Club", new Vector2(450, 680), ShopType.GolfCourse);
        CreateShop("Himley Park Golf", new Vector2(180, 300), ShopType.GolfCourse);
        
        // === TENNIS CLUBS ===
        CreateShop("Wolverhampton Tennis Club", new Vector2(200, 550), ShopType.TennisClub);
        CreateShop("Tettenhall Tennis", new Vector2(630, 725), ShopType.TennisClub);
        CreateShop("West Park Tennis", new Vector2(395, 560), ShopType.TennisClub);
        
        // === INDUSTRIAL AREAS ===
        CreateShop("Bilston Industrial Estate", new Vector2(270, 350), ShopType.Industrial);
        CreateShop("M53 Business Park", new Vector2(250, 420), ShopType.Industrial);
        CreateShop("Stafford Road Industrial", new Vector2(350, 460), ShopType.Industrial);
        CreateShop("Crossgate Industrial", new Vector2(550, 420), ShopType.Industrial);
        
        // === FARMS (Rural Edge) ===
        CreateShop("Hill Farm", new Vector2(100, 300), ShopType.Farm);
        CreateShop("Marsh Farm", new Vector2(150, 250), ShopType.Farm);
        CreateShop("Green Farm", new Vector2(200, 200), ShopType.Farm);
        CreateShop("Oak Farm", new Vector2(850, 300), ShopType.Farm);
        CreateShop("Bridge Farm", new Vector2(880, 400), ShopType.Farm);
        CreateShop("Park Farm", new Vector2(820, 200), ShopType.Farm);
        CreateShop("Wood Farm", new Vector2(100, 800), ShopType.Farm);
        CreateShop("Valley Farm", new Vector2(200, 850), ShopType.Farm);
        CreateShop("Ridge Farm", new Vector2(850, 800), ShopType.Farm);

        Debug.Log($"Initialized {allShops.Count} named businesses");
    }

    private void CreateShop(string name, Vector2 position, ShopType type)
    {
        CreateShop(name, position, type, true);
    }

    private void CreateShop(string name, Vector2 position, ShopType type, bool hasUtility)
    {
        GameObject shopGO = new GameObject(name);
        shopGO.transform.position = (Vector3)position;

        SpriteRenderer sr = shopGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 5;

        CircleCollider2D collider = shopGO.AddComponent<CircleCollider2D>();
        collider.radius = shopRadius;
        collider.isTrigger = true;

        ShopData shop = shopGO.AddComponent<ShopData>();
        shop.Initialize(name, position, type, hasUtility);

        switch (type)
        {
            case ShopType.House:
                sr.color = new Color(0.75f, 0.55f, 0.45f, 1f);
                shopGO.transform.localScale = new Vector3(8f, 10f, 1f);
                break;
            case ShopType.Flat:
                sr.color = new Color(0.6f, 0.6f, 0.65f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 25f, 1f);
                break;
            case ShopType.Supermarket:
                sr.color = new Color(0.25f, 0.45f, 0.25f, 1f);
                shopGO.transform.localScale = new Vector3(30f, 22f, 1f);
                break;
            case ShopType.OutfitShop:
                sr.color = new Color(0.9f, 0.6f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(15f, 12f, 1f);
                break;
            case ShopType.Garage:
                sr.color = new Color(0.35f, 0.35f, 0.7f, 1f);
                shopGO.transform.localScale = new Vector3(20f, 15f, 1f);
                break;
            case ShopType.Club:
                sr.color = new Color(0.8f, 0.2f, 0.8f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 15f, 1f);
                break;
            case ShopType.SocialClub:
                sr.color = new Color(0.3f, 0.5f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 15f, 1f);
                break;
            case ShopType.SportsClub:
                sr.color = new Color(0.2f, 0.55f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 20f, 1f);
                break;
            case ShopType.Bar:
                sr.color = new Color(0.6f, 0.4f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(14f, 12f, 1f);
                break;
            case ShopType.Shop:
            case ShopType.ShoppingCentre:
                sr.color = new Color(0.5f, 0.6f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(15f, 12f, 1f);
                break;
            case ShopType.Restaurant:
                sr.color = new Color(0.9f, 0.5f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(16f, 14f, 1f);
                break;
            case ShopType.FastFood:
                sr.color = new Color(0.9f, 0.3f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(12f, 10f, 1f);
                break;
            case ShopType.PawnShop:
                sr.color = new Color(0.7f, 0.5f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(12f, 10f, 1f);
                break;
            case ShopType.Park:
                sr.color = new Color(0.25f, 0.55f, 0.25f, 1f);
                shopGO.transform.localScale = new Vector3(40f, 35f, 1f);
                break;
            case ShopType.Pub:
                sr.color = new Color(0.5f, 0.25f, 0.1f, 1f);
                shopGO.transform.localScale = new Vector3(14f, 12f, 1f);
                break;
            case ShopType.Hotel:
                sr.color = new Color(0.3f, 0.4f, 0.7f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 20f, 1f);
                break;
            case ShopType.Bank:
                sr.color = new Color(0.2f, 0.4f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 16f, 1f);
                break;
            case ShopType.Theatre:
                sr.color = new Color(0.7f, 0.2f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(22f, 18f, 1f);
                break;
            case ShopType.Cinema:
                sr.color = new Color(0.6f, 0.15f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(28f, 20f, 1f);
                break;
            case ShopType.Hospital:
                sr.color = new Color(0.9f, 0.9f, 0.9f, 1f);
                shopGO.transform.localScale = new Vector3(35f, 30f, 1f);
                break;
            case ShopType.Pharmacy:
                sr.color = new Color(0.4f, 0.8f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(12f, 10f, 1f);
                break;
            case ShopType.Vet:
                sr.color = new Color(0.5f, 0.7f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(14f, 12f, 1f);
                break;
            case ShopType.Police:
                sr.color = new Color(0.2f, 0.3f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(20f, 16f, 1f);
                break;
            case ShopType.FireStation:
                sr.color = new Color(0.8f, 0.2f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(22f, 18f, 1f);
                break;
            case ShopType.ConvenienceStore:
                sr.color = new Color(0.6f, 0.7f, 0.4f, 1f);
                shopGO.transform.localScale = new Vector3(10f, 8f, 1f);
                break;
            case ShopType.DIY:
                sr.color = new Color(0.7f, 0.4f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 18f, 1f);
                break;
            case ShopType.CarDealer:
                sr.color = new Color(0.4f, 0.4f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(28f, 18f, 1f);
                break;
            case ShopType.HairSalon:
                sr.color = new Color(0.8f, 0.5f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(10f, 8f, 1f);
                break;
            case ShopType.Gym:
                sr.color = new Color(0.5f, 0.3f, 0.4f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 14f, 1f);
                break;
            case ShopType.PostOffice:
                sr.color = new Color(0.3f, 0.3f, 0.7f, 1f);
                shopGO.transform.localScale = new Vector3(14f, 12f, 1f);
                break;
            case ShopType.PetrolStation:
                sr.color = new Color(0.8f, 0.5f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(20f, 15f, 1f);
                break;
            case ShopType.Civic:
                sr.color = new Color(0.4f, 0.5f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 20f, 1f);
                break;
            case ShopType.Stadium:
                sr.color = new Color(0.85f, 0.75f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(35f, 30f, 1f);
                break;
            case ShopType.RacingTrack:
                sr.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(50f, 40f, 1f);
                break;
            case ShopType.CricketGround:
                sr.color = new Color(0.4f, 0.5f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(30f, 30f, 1f);
                break;
            case ShopType.FootballPitch:
                sr.color = new Color(0.2f, 0.55f, 0.2f, 1f);
                shopGO.transform.localScale = new Vector3(30f, 20f, 1f);
                break;
            case ShopType.TennisClub:
                sr.color = new Color(0.3f, 0.6f, 0.4f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 20f, 1f);
                break;
            case ShopType.GolfCourse:
                sr.color = new Color(0.3f, 0.5f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(45f, 40f, 1f);
                break;
            case ShopType.SwimmingPool:
                sr.color = new Color(0.2f, 0.5f, 0.7f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 18f, 1f);
                break;
            case ShopType.Electronics:
                sr.color = new Color(0.3f, 0.5f, 0.8f, 1f);
                shopGO.transform.localScale = new Vector3(15f, 12f, 1f);
                break;
            case ShopType.Library:
                sr.color = new Color(0.5f, 0.4f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 15f, 1f);
                break;
            case ShopType.Education:
                sr.color = new Color(0.4f, 0.3f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(22f, 18f, 1f);
                break;
            case ShopType.Church:
                sr.color = new Color(0.6f, 0.6f, 0.65f, 1f);
                shopGO.transform.localScale = new Vector3(16f, 20f, 1f);
                break;
            case ShopType.Market:
                sr.color = new Color(0.7f, 0.6f, 0.4f, 1f);
                shopGO.transform.localScale = new Vector3(30f, 25f, 1f);
                break;
            case ShopType.Industrial:
                sr.color = new Color(0.45f, 0.4f, 0.38f, 1f);
                shopGO.transform.localScale = new Vector3(35f, 25f, 1f);
                break;
            case ShopType.Residential:
                sr.color = new Color(0.7f, 0.6f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(12f, 12f, 1f);
                break;
            case ShopType.Farm:
                sr.color = new Color(0.6f, 0.5f, 0.35f, 1f);
                shopGO.transform.localScale = new Vector3(25f, 20f, 1f);
                break;
            case ShopType.RailwayStation:
                sr.color = new Color(0.35f, 0.3f, 0.3f, 1f);
                shopGO.transform.localScale = new Vector3(40f, 25f, 1f);
                break;
            case ShopType.MetroStop:
                sr.color = new Color(0.8f, 0.6f, 0.1f, 1f);
                shopGO.transform.localScale = new Vector3(15f, 10f, 1f);
                break;
            case ShopType.CanalBasin:
                sr.color = new Color(0.2f, 0.4f, 0.5f, 1f);
                shopGO.transform.localScale = new Vector3(20f, 15f, 1f);
                break;
            case ShopType.DepartmentStore:
                sr.color = new Color(0.5f, 0.35f, 0.35f, 1f);
                shopGO.transform.localScale = new Vector3(30f, 25f, 1f);
                break;
            case ShopType.BowlingClub:
                sr.color = new Color(0.5f, 0.4f, 0.35f, 1f);
                shopGO.transform.localScale = new Vector3(18f, 15f, 1f);
                break;
            case ShopType.SciencePark:
                sr.color = new Color(0.4f, 0.5f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(35f, 25f, 1f);
                break;
            case ShopType.Landmark:
                sr.color = new Color(0.7f, 0.65f, 0.6f, 1f);
                shopGO.transform.localScale = new Vector3(20f, 22f, 1f);
                break;
            case ShopType.None:
                sr.color = Color.gray;
                shopGO.transform.localScale = new Vector3(10f, 10f, 1f);
                break;
            default:
                sr.color = Color.white;
                shopGO.transform.localScale = new Vector3(12f, 12f, 1f);
                break;
        }

        shopGO.transform.parent = transform;
        allShops.Add(shop);
    }

    public ShopData GetNearestShop(Vector2 position)
    {
        ShopData nearest = null;
        float minDist = float.MaxValue;

        foreach (var shop in allShops)
        {
            float dist = Vector2.Distance(position, shop.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = shop;
            }
        }

        return nearest;
    }

    public List<ShopData> GetAllShops() => allShops;
    public List<ShopData> GetShopsOfType(ShopType type)
    {
        return allShops.FindAll(s => s.Type == type);
    }
}

public enum ShopType
{
    House,
    Flat,
    Shop,
    Supermarket,
    FastFood,
    Restaurant,
    Pub,
    Bar,
    Club,
    SocialClub,
    SportsClub,
    Gym,
    Hotel,
    Bank,
    Theatre,
    Cinema,
    Hospital,
    Pharmacy,
    Vet,
    Police,
    FireStation,
    Garage,
    CarDealer,
    PetrolStation,
    OutfitShop,
    Electronics,
    PawnShop,
    ConvenienceStore,
    DIY,
    HairSalon,
    PostOffice,
    Library,
    Education,
    Church,
    Stadium,
    RacingTrack,
    CricketGround,
    FootballPitch,
    TennisClub,
    GolfCourse,
    SwimmingPool,
    Civic,
    Market,
    Park,
    Canal,
    Industrial,
    Residential,
    Farm,
    RailwayStation,
    MetroStop,
    CanalBasin,
    DepartmentStore,
    BowlingClub,
    SciencePark,
    Landmark,
    None
}

public class ShopData : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector2 Position { get; private set; }
    public ShopType Type { get; private set; }
    public bool HasUtility { get; private set; }

    public void Initialize(string name, Vector2 position, ShopType type, bool hasUtility = false)
    {
        Name = name;
        Position = position;
        Type = type;
        HasUtility = hasUtility;
        
        if (!hasUtility)
        {
            var label = GetComponentInChildren<UnityEngine.UI.Text>();
            if (label != null)
                label.enabled = false;
        }
    }
}

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    
    private List<GameObject> unnamedBuildings = new List<GameObject>();
    
    private float residentialRoadWidth = 16f;
    private float majorRoadWidth = 28f;
    private float dualCarriagewayWidth = 42f;
    private float pavementWidth = 4f;
    private float shoulderWidth = 2f;
    private float grassStripWidth = 5f;
    
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
        CreateAllBuildings();
    }
    
    private void CreateAllBuildings()
    {
        CreateResidentialBuildings();
        CreateShopBuildings();
        CreateSportsVenues();
        CreateSocialClubs();
        CreateIndustrialBuildings();
        CreateFarmBuildings();
        
        VerifyBuildingPlacement();
        
        Debug.Log($"Created {unnamedBuildings.Count} unnamed buildings");
    }
    
    private void VerifyBuildingPlacement()
    {
        int blockedCount = 0;
        foreach (GameObject building in unnamedBuildings)
        {
            if (!IsPositionClearOfRoads(building.transform.position, 8f))
            {
                blockedCount++;
            }
        }
        if (blockedCount > 0)
        {
            Debug.LogWarning($"Warning: {blockedCount} buildings may be too close to roads");
        }
    }
    
    private float GetBuildingOffsetFromRoad(float roadCenter, float roadWidth, bool above)
    {
        float halfRoad = roadWidth / 2f;
        float pavementEnd = halfRoad + pavementWidth;
        float grassEnd = pavementEnd + grassStripWidth;
        float buildingStart = grassEnd + 5f;
        return above ? buildingStart : -buildingStart;
    }
    
    private bool IsPositionClearOfRoads(Vector2 position, float bufferDistance = 10f)
    {
        if (MapSystem.Instance != null)
        {
            foreach (Rect road in MapSystem.Instance.GetRoadSegments())
            {
                Rect bufferedRoad = new Rect(
                    road.x - bufferDistance,
                    road.y - bufferDistance,
                    road.width + bufferDistance * 2,
                    road.height + bufferDistance * 2
                );
                if (bufferedRoad.Contains(position))
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    private void CreateResidentialBuildings()
    {
        float roadY;
        
        roadY = 150f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, true);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.Terraced);
        }
        
        roadY = 250f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, false);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.Terraced);
        }
        
        roadY = 350f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, true);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.Terraced);
        }
        
        roadY = 450f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, false);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.SemiDetached);
        }
        
        roadY = 550f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, true);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.SemiDetached);
        }
        
        roadY = 650f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, false);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.Terraced);
        }
        
        roadY = 750f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, true);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.Terraced);
        }
        
        roadY = 850f;
        for (int i = 0; i < 30; i++)
        {
            float x = 100f + i * 25f;
            float yOffset = GetBuildingOffsetFromRoad(roadY, residentialRoadWidth, false);
            CreateHouse(new Vector2(x, roadY + yOffset), HouseType.Detached);
        }
        
        float roadX;
        roadX = 100f;
        for (int i = 0; i < 25; i++)
        {
            float y = 150f + i * 30f;
            float xOffset = GetBuildingOffsetFromRoad(roadX, residentialRoadWidth, true);
            CreateHouse(new Vector2(roadX + xOffset, y), HouseType.Terraced);
        }
        
        roadX = 200f;
        for (int i = 0; i < 25; i++)
        {
            float y = 150f + i * 30f;
            float xOffset = GetBuildingOffsetFromRoad(roadX, residentialRoadWidth, false);
            CreateHouse(new Vector2(roadX + xOffset, y), HouseType.Terraced);
        }
        
        roadX = 400f;
        for (int i = 0; i < 25; i++)
        {
            float y = 150f + i * 30f;
            float xOffset = GetBuildingOffsetFromRoad(roadX, residentialRoadWidth, true);
            CreateHouse(new Vector2(roadX + xOffset, y), HouseType.SemiDetached);
        }
        
        roadX = 600f;
        for (int i = 0; i < 25; i++)
        {
            float y = 150f + i * 30f;
            float xOffset = GetBuildingOffsetFromRoad(roadX, residentialRoadWidth, false);
            CreateHouse(new Vector2(roadX + xOffset, y), HouseType.SemiDetached);
        }
        
        roadX = 800f;
        for (int i = 0; i < 25; i++)
        {
            float y = 150f + i * 30f;
            float xOffset = GetBuildingOffsetFromRoad(roadX, residentialRoadWidth, true);
            CreateHouse(new Vector2(roadX + xOffset, y), HouseType.Terraced);
        }
        
        CreateFlatBlocksNearMajorRoads();
    }
    
    private void CreateFlatBlocksNearMajorRoads()
    {
        float majorY = 300f;
        float blockY1 = majorY - majorRoadWidth/2 - pavementWidth - grassStripWidth - 12f;
        float blockY2 = majorY + majorRoadWidth/2 + pavementWidth + grassStripWidth + 12f;
        
        for (int i = 0; i < 5; i++)
        {
            float x = 250f + i * 100f;
            CreateFlatBlock(new Vector2(x, blockY1));
            CreateFlatBlock(new Vector2(x, blockY2));
        }
        
        majorY = 500f;
        blockY1 = majorY - majorRoadWidth/2 - pavementWidth - grassStripWidth - 12f;
        blockY2 = majorY + majorRoadWidth/2 + pavementWidth + grassStripWidth + 12f;
        
        for (int i = 0; i < 5; i++)
        {
            float x = 250f + i * 100f;
            CreateFlatBlock(new Vector2(x, blockY1));
            CreateFlatBlock(new Vector2(x, blockY2));
        }
        
        majorY = 700f;
        blockY1 = majorY - majorRoadWidth/2 - pavementWidth - grassStripWidth - 12f;
        blockY2 = majorY + majorRoadWidth/2 + pavementWidth + grassStripWidth + 12f;
        
        for (int i = 0; i < 5; i++)
        {
            float x = 250f + i * 100f;
            CreateFlatBlock(new Vector2(x, blockY1));
            CreateFlatBlock(new Vector2(x, blockY2));
        }
    }
    
    private void CreateHouse(Vector2 position, HouseType type)
    {
        GameObject house = new GameObject();
        house.transform.position = (Vector3)position;
        house.transform.parent = transform;
        
        SpriteRenderer sr = house.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.sortingOrder = 3;
        
        BoxCollider2D col = house.AddComponent<BoxCollider2D>();
        
        switch (type)
        {
            case HouseType.Terraced:
                sr.color = new Color(0.75f, 0.55f, 0.45f, 1f);
                house.transform.localScale = new Vector3(7f, 9f, 1f);
                col.size = new Vector2(7f, 9f);
                house.name = "TerracedHouse";
                break;
            case HouseType.SemiDetached:
                sr.color = new Color(0.8f, 0.7f, 0.6f, 1f);
                house.transform.localScale = new Vector3(11f, 11f, 1f);
                col.size = new Vector2(11f, 11f);
                house.name = "SemiHouse";
                break;
            case HouseType.Detached:
                sr.color = new Color(0.85f, 0.75f, 0.65f, 1f);
                house.transform.localScale = new Vector3(14f, 13f, 1f);
                col.size = new Vector2(14f, 13f);
                house.name = "DetachedHouse";
                break;
        }
        
        unnamedBuildings.Add(house);
    }
    
    private void CreateFlatBlock(Vector2 position)
    {
        GameObject flat = new GameObject("FlatBlock");
        flat.transform.position = (Vector3)position;
        flat.transform.parent = transform;
        
        SpriteRenderer sr = flat.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.GetDefaultSprite();
        sr.color = new Color(0.55f, 0.55f, 0.6f, 1f);
        sr.sortingOrder = 3;
        flat.transform.localScale = new Vector3(16f, 22f, 1f);
        
        BoxCollider2D col = flat.AddComponent<BoxCollider2D>();
        col.size = new Vector2(16f, 22f);
        
        unnamedBuildings.Add(flat);
    }
    
    private void CreateShopBuildings()
    {
        float majorRoadY = 300f;
        float buildingY1 = majorRoadY - majorRoadWidth/2 - pavementWidth - grassStripWidth - 10f;
        float buildingY2 = majorRoadY + majorRoadWidth/2 + pavementWidth + grassStripWidth + 10f;
        
        for (int i = 0; i < 4; i++)
        {
            float x = 280f + i * 120f;
            CreateGenericBuilding(new Vector2(x, buildingY1), BuildingType.ShopFront, new Color(0.6f, 0.5f, 0.4f));
            CreateGenericBuilding(new Vector2(x + 8f, buildingY1), BuildingType.ShopFront, new Color(0.55f, 0.5f, 0.45f));
            CreateGenericBuilding(new Vector2(x, buildingY2), BuildingType.ShopFront, new Color(0.5f, 0.55f, 0.45f));
        }
        
        majorRoadY = 500f;
        buildingY1 = majorRoadY - majorRoadWidth/2 - pavementWidth - grassStripWidth - 10f;
        buildingY2 = majorRoadY + majorRoadWidth/2 + pavementWidth + grassStripWidth + 10f;
        
        for (int i = 0; i < 4; i++)
        {
            float x = 280f + i * 120f;
            CreateGenericBuilding(new Vector2(x, buildingY1), BuildingType.ShopFront, new Color(0.6f, 0.5f, 0.4f));
            CreateGenericBuilding(new Vector2(x + 8f, buildingY1), BuildingType.ShopFront, new Color(0.55f, 0.5f, 0.45f));
            CreateGenericBuilding(new Vector2(x, buildingY2), BuildingType.ShopFront, new Color(0.5f, 0.55f, 0.45f));
        }
        
        majorRoadY = 700f;
        buildingY1 = majorRoadY - majorRoadWidth/2 - pavementWidth - grassStripWidth - 10f;
        buildingY2 = majorRoadY + majorRoadWidth/2 + pavementWidth + grassStripWidth + 10f;
        
        for (int i = 0; i < 4; i++)
        {
            float x = 280f + i * 120f;
            CreateGenericBuilding(new Vector2(x, buildingY1), BuildingType.ShopFront, new Color(0.6f, 0.5f, 0.4f));
            CreateGenericBuilding(new Vector2(x + 8f, buildingY1), BuildingType.ShopFront, new Color(0.55f, 0.5f, 0.45f));
            CreateGenericBuilding(new Vector2(x, buildingY2), BuildingType.ShopFront, new Color(0.5f, 0.55f, 0.45f));
        }
    }
    
    private void CreateSportsVenues()
    {
        float stadiumX = 450f;
        float stadiumY = 560f;
        CreateGenericBuilding(new Vector2(stadiumX, stadiumY), BuildingType.Stadium, new Color(0.85f, 0.75f, 0.2f));
        CreateGenericBuilding(new Vector2(stadiumX + 5f, stadiumY - 5f), BuildingType.StadiumPitch, new Color(0.2f, 0.55f, 0.2f));
        
        float racingX = 650f;
        float racingY = 600f;
        CreateGenericBuilding(new Vector2(racingX, racingY), BuildingType.RacingTrack, new Color(0.6f, 0.6f, 0.6f));
        
        CreateGenericBuilding(new Vector2(250f, 550f), BuildingType.GolfClub, new Color(0.3f, 0.5f, 0.3f));
        CreateGenericBuilding(new Vector2(680f, 720f), BuildingType.GolfClub, new Color(0.3f, 0.5f, 0.3f));
        
        CreateGenericBuilding(new Vector2(175f, 440f), BuildingType.SportsGround, new Color(0.2f, 0.55f, 0.2f));
        
        CreateGenericBuilding(new Vector2(200f, 580f), BuildingType.CricketGround, new Color(0.4f, 0.5f, 0.3f));
        
        CreateGenericBuilding(new Vector2(540f, 510f), BuildingType.Gym, new Color(0.5f, 0.3f, 0.4f));
        CreateGenericBuilding(new Vector2(495f, 525f), BuildingType.Gym, new Color(0.5f, 0.3f, 0.4f));
        CreateGenericBuilding(new Vector2(460f, 500f), BuildingType.Gym, new Color(0.5f, 0.3f, 0.4f));
    }
    
    private void CreateSocialClubs()
    {
        CreateGenericBuilding(new Vector2(520f, 480f), BuildingType.SocialClub, new Color(0.3f, 0.5f, 0.6f));
        CreateGenericBuilding(new Vector2(800f, 550f), BuildingType.SocialClub, new Color(0.3f, 0.5f, 0.6f));
        CreateGenericBuilding(new Vector2(530f, 800f), BuildingType.SocialClub, new Color(0.3f, 0.5f, 0.6f));
        CreateGenericBuilding(new Vector2(150f, 480f), BuildingType.SocialClub, new Color(0.3f, 0.5f, 0.6f));
        
        CreateGenericBuilding(new Vector2(160f, 470f), BuildingType.WorkingMensClub, new Color(0.4f, 0.35f, 0.3f));
        CreateGenericBuilding(new Vector2(480f, 800f), BuildingType.WorkingMensClub, new Color(0.4f, 0.35f, 0.3f));
        CreateGenericBuilding(new Vector2(300f, 450f), BuildingType.WorkingMensClub, new Color(0.4f, 0.35f, 0.3f));
        
        CreateGenericBuilding(new Vector2(260f, 290f), BuildingType.BowlingClub, new Color(0.5f, 0.4f, 0.35f));
        CreateGenericBuilding(new Vector2(700f, 620f), BuildingType.BowlingClub, new Color(0.5f, 0.4f, 0.35f));
        
        CreateGenericBuilding(new Vector2(200f, 550f), BuildingType.TennisClub, new Color(0.3f, 0.6f, 0.4f));
        CreateGenericBuilding(new Vector2(750f, 700f), BuildingType.TennisClub, new Color(0.3f, 0.6f, 0.4f));
    }
    
    private void CreateIndustrialBuildings()
    {
        CreateGenericBuilding(new Vector2(270f, 350f), BuildingType.Warehouse, new Color(0.45f, 0.42f, 0.38f));
        CreateGenericBuilding(new Vector2(290f, 345f), BuildingType.Warehouse, new Color(0.46f, 0.43f, 0.39f));
        
        CreateGenericBuilding(new Vector2(400f, 420f), BuildingType.Office, new Color(0.5f, 0.55f, 0.6f));
        CreateGenericBuilding(new Vector2(410f, 430f), BuildingType.Office, new Color(0.48f, 0.53f, 0.58f));
        
        CreateGenericBuilding(new Vector2(250f, 420f), BuildingType.Office, new Color(0.5f, 0.5f, 0.5f));
        
        CreateGenericBuilding(new Vector2(550f, 420f), BuildingType.Warehouse, new Color(0.44f, 0.42f, 0.40f));
        
        CreateGenericBuilding(new Vector2(360f, 340f), BuildingType.Warehouse, new Color(0.45f, 0.43f, 0.40f));
        CreateGenericBuilding(new Vector2(420f, 320f), BuildingType.Warehouse, new Color(0.44f, 0.42f, 0.39f));
    }
    
    private void CreateFarmBuildings()
    {
        CreateGenericBuilding(new Vector2(100f, 280f), BuildingType.Farm, new Color(0.6f, 0.5f, 0.35f));
        CreateGenericBuilding(new Vector2(150f, 250f), BuildingType.Farm, new Color(0.58f, 0.48f, 0.33f));
        CreateGenericBuilding(new Vector2(200f, 200f), BuildingType.Farm, new Color(0.62f, 0.52f, 0.37f));
        
        CreateGenericBuilding(new Vector2(850f, 300f), BuildingType.Farm, new Color(0.6f, 0.5f, 0.35f));
        CreateGenericBuilding(new Vector2(880f, 400f), BuildingType.Farm, new Color(0.58f, 0.48f, 0.33f));
        CreateGenericBuilding(new Vector2(820f, 200f), BuildingType.Farm, new Color(0.62f, 0.52f, 0.37f));
        
        CreateGenericBuilding(new Vector2(100f, 800f), BuildingType.Farm, new Color(0.6f, 0.5f, 0.35f));
        CreateGenericBuilding(new Vector2(200f, 850f), BuildingType.Farm, new Color(0.58f, 0.48f, 0.33f));
        CreateGenericBuilding(new Vector2(850f, 800f), BuildingType.Farm, new Color(0.62f, 0.52f, 0.37f));
        
        CreateGenericBuilding(new Vector2(50f, 500f), BuildingType.Farm, new Color(0.6f, 0.5f, 0.35f));
        CreateGenericBuilding(new Vector2(900f, 600f), BuildingType.Farm, new Color(0.58f, 0.48f, 0.33f));
    }
    
    private void CreateGenericBuilding(Vector2 position, BuildingType type, Color color)
    {
        GameObject building = new GameObject(type.ToString());
        building.transform.position = (Vector3)position;
        building.transform.parent = transform;
        
        SpriteRenderer sr = building.AddComponent<SpriteRenderer>();
        
        string spriteKey = GetBuildingSpriteKey(type);
        sr.sprite = GTASpriteGenerator.GetSprite(spriteKey);
        sr.sortingOrder = 3;
        
        BoxCollider2D col = building.AddComponent<BoxCollider2D>();
        
        switch (type)
        {
            case BuildingType.ShopFront:
                building.transform.localScale = new Vector3(9f, 12f, 1f);
                col.size = new Vector2(9f, 12f);
                break;
            case BuildingType.Stadium:
                building.transform.localScale = new Vector3(40f, 35f, 1f);
                col.size = new Vector2(40f, 35f);
                break;
            case BuildingType.StadiumPitch:
                building.transform.localScale = new Vector3(30f, 22f, 1f);
                col.size = new Vector2(30f, 22f);
                break;
            case BuildingType.RacingTrack:
                building.transform.localScale = new Vector3(60f, 45f, 1f);
                col.size = new Vector2(60f, 45f);
                break;
            case BuildingType.GolfClub:
                building.transform.localScale = new Vector3(25f, 22f, 1f);
                col.size = new Vector2(25f, 22f);
                break;
            case BuildingType.SportsGround:
                building.transform.localScale = new Vector3(35f, 28f, 1f);
                col.size = new Vector2(35f, 28f);
                break;
            case BuildingType.CricketGround:
                building.transform.localScale = new Vector3(30f, 30f, 1f);
                col.size = new Vector2(30f, 30f);
                break;
            case BuildingType.Gym:
                building.transform.localScale = new Vector3(14f, 11f, 1f);
                col.size = new Vector2(14f, 11f);
                break;
            case BuildingType.SocialClub:
                building.transform.localScale = new Vector3(18f, 15f, 1f);
                col.size = new Vector2(18f, 15f);
                break;
            case BuildingType.WorkingMensClub:
                building.transform.localScale = new Vector3(20f, 16f, 1f);
                col.size = new Vector2(20f, 16f);
                break;
            case BuildingType.BowlingClub:
                building.transform.localScale = new Vector3(16f, 16f, 1f);
                col.size = new Vector2(16f, 16f);
                break;
            case BuildingType.TennisClub:
                building.transform.localScale = new Vector3(30f, 25f, 1f);
                col.size = new Vector2(30f, 25f);
                break;
            case BuildingType.Warehouse:
                building.transform.localScale = new Vector3(35f, 25f, 1f);
                col.size = new Vector2(35f, 25f);
                break;
            case BuildingType.Office:
                building.transform.localScale = new Vector3(22f, 16f, 1f);
                col.size = new Vector2(22f, 16f);
                break;
            case BuildingType.Farm:
                building.transform.localScale = new Vector3(30f, 25f, 1f);
                col.size = new Vector2(30f, 25f);
                break;
            case BuildingType.CanalBasin:
                building.transform.localScale = new Vector3(18f, 12f, 1f);
                col.size = new Vector2(18f, 12f);
                break;
            case BuildingType.Lock:
                building.transform.localScale = new Vector3(10f, 8f, 1f);
                col.size = new Vector2(10f, 8f);
                break;
            default:
                building.transform.localScale = new Vector3(14f, 14f, 1f);
                col.size = new Vector2(14f, 14f);
                break;
        }
        
        unnamedBuildings.Add(building);
    }

    private string GetBuildingSpriteKey(BuildingType type)
    {
        return type switch
        {
            BuildingType.ShopFront => "building_shop",
            BuildingType.Warehouse => "building_warehouse",
            BuildingType.Office => "building_office",
            BuildingType.Farm => "building_farm",
            BuildingType.Gym => "building_gym",
            _ => "building_house"
        };
    }
    
    public List<GameObject> GetUnnamedBuildings() => unnamedBuildings;
}

public enum HouseType
{
    Terraced,
    SemiDetached,
    Detached
}

public enum BuildingType
{
    ShopFront,
    Stadium,
    StadiumPitch,
    RacingTrack,
    GolfClub,
    SportsGround,
    CricketGround,
    Gym,
    SocialClub,
    WorkingMensClub,
    BowlingClub,
    TennisClub,
    Warehouse,
    Office,
    Pub,
    Church,
    School,
    Hospital,
    Farm,
    CanalBasin,
    Lock
}