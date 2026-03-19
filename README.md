# Wolverhampton Crime City - Game Development

A 2D isometric action game inspired by GTA 1, Shakedown Hawaii, and Retro City Rampage DX. Set in a fictional Wolverhampton with core mechanics focusing on driving, combat, missions, and vehicle theft.
 
## Project Type
- **Engine**: Unity 2D
- **Target Platform**: PC
- **Art Style**: Isometric/Top-Down 2D
 
## Game Features

### Core Mechanics
- **Driving**: Vehicle physics and controls for cars, trucks, and motorcycles
- **Combat**: Hand-to-hand combat and weapon system
- **Missions**: Dynamic mission system from three unique mission hubs
- **Vehicle Theft**: Car stealing mechanics with wanted level system
- **Day/Night Cycle**: Dynamic 24-hour cycle with visual lighting changes
- **Weather System**: Random weather patterns (clear, rain, snow, fog, storm)
- **Pedestrian AI**: 50+ NPCs walking around the city
- **Roadworks**: 8 construction zones that slow vehicles

### Mission Hubs

#### 1. The Factory (West End) - Rusty
- **Character**: Rusty, a worn-out 60-year-old chain-smoking bar owner
- **Location**: West End district
- **Mission Types**: Bar debt collection, deliveries, muscle work, robbery
- **Atmosphere**: Gritty dive bar environment

#### 2. Crimson Cab Station (City Centre) - Veronica
- **Character**: Veronica, an attractive seductive woman in her mid-40s
- **Location**: City Centre district
- **Mission Types**: Corporate escorts, vehicle recovery, sabotage, deliveries
- **Special Feature**: Operates a high-end brothel upstairs with various services
- **Atmosphere**: Professional taxi dispatch with hidden illicit business

#### 3. Mobile Meetups - Detective Morgan
- **Character**: Detective Morgan, an early-50s female cop earning extra pre-retirement cash
- **Locations**: Changes between 5 different locations around the city
- **Mission Types**: Evidence disposal, contraband transport, witness intimidation, grand theft auto
- **Atmosphere**: Paranoid, secretive cop meetings in dark alleys and industrial areas

## Directory Structure

```
Assets/
├── Scripts/
│   ├── Core/              # Game managers
│   │   ├── GameManager.cs
│   │   └── CityManager.cs
│   ├── Player/            # Player controller and management
│   ├── Vehicles/          # Vehicle system
│   ├── Combat/            # Combat system
│   ├── Missions/          # Mission system
│   │   ├── MissionManager.cs
│   │   ├── MissionHubCharacter.cs
│   │   ├── MissionHubManager.cs
│   │   ├── MissionGenerator.cs
│   │   └── HubLocation.cs
│   ├── NPCs/              # NPC and enemy AI
│   ├── UI/                # User interface
│   └── Utils/             # Utility classes
├── Scenes/                # Game scenes
├── Prefabs/               # Reusable game objects
├── Sprites/               # 2D graphics
└── Audio/                 # Sound effects and music
```

## Vehicle System

### Vehicles Available

**Trucks** (1 type)
- Slow Truck: Industrial Hauler (8 mph max speed, heavy)

**Vans** (3 varieties)
- Economy Van: 10 mph - good capacity, slow
- Transit Van: 12 mph - medium speed, reliable
- Delivery Van: 14 mph - fastest van, responsive

**Cars** (10 types, slow to fast)
- Compact Runner: 12 mph - small and nimble, 3-seater
- City Runner: 13 mph - basic economy option, red
- Yellow Cab: 13.5 mph - taxi, reliable workhorse
- Retro Coupe: 14 mph - classic styled, purple, 2-seater
- Family Sedan: 15 mph - balanced all-arounder, black, 4-seater
- Executive: 16 mph - luxury sedan, dark blue, premium
- Bruiser: 17 mph - muscle car, dark red, heavy and powerful
- Thunder Coupe: 18 mph - sports car, orange-red, high acceleration
- Police Cruiser: 19 mph - cop car, tanky, dark blue
- Apex Predator: 22 mph - supercar, yellow, elite speed machine

**Motorcycle**
- Street Bike: 20 mph - quick and nimble, solo vehicle

Each vehicle has unique:
- Max speed and acceleration
- Turn speed and weight
- Passenger capacity
- Brake power
- Color coding for easy identification

### Vehicle Systems
- **VehicleFactory**: Creates vehicle instances with predefined stats
- **VehicleManager**: Spawns 35+ vehicles around the map with realistic distribution
- **VehicleGarage**: Allows players to store vehicles (expandable system)
- **Vehicle Theft**: Each vehicle can be stolen, increasing wanted level

## Implementation Progress

- [x] Project structure and directories
- [x] Core game manager system
- [x] City manager with Wolverhampton districts
- [x] Mission hub characters (Rusty, Veronica, Detective Morgan)
- [x] Mission hub manager and location system
- [x] Mission generator for dialog and missions
- [x] Brothel system
- [x] Vehicle system with 14 vehicle types (1 truck, 3 vans, 10 cars, 1 motorcycle)
- [x] Vehicle factory and spawn system
- [x] Vehicle garage system
- [x] Day/Night cycle with dynamic lighting
- [x] Weather system with 5 weather types
- [x] Environment HUD (time, weather, temperature display)
- [x] Roadworks system with 8 construction zones
- [x] Pedestrian AI with 50+ NPCs
- [x] 12 Points of Interest (shops, schools, universities)
- [ ] Player controller and movement
- [ ] Combat system
- [ ] AI pathfinding for NPCs
- [ ] Police wanted system
- [ ] Save/Load system
- [ ] UI implementation
- [ ] Audio system
- [ ] Level design and map
- [ ] Sprite creation/import

## Key Characters

### Rusty (Dive Bar Owner)
- Age: 60s
- Personality: Gruff, tired, chain-smoking
- Business: The Factory (dive bar in West End)
- Offers: Debt collection, deliveries, muscle work
- Dialogue: Cynical and world-weary

### Veronica (Taxi Rank Owner)
- Age: Mid-40s
- Personality: Seductive, charming, business savvy
- Businesses: Crimson Cab Station (taxi rank) + Velvet Dreams (brothel)
- Offers: Escorts, vehicle recovery, sabotage, deliveries
- Special: Unlockable brothel services for player
- Dialogue: Flirtatious and suggestive

### Detective Morgan (Cop)
- Age: Early 50s
- Personality: Paranoid, desperate for money, cautious
- Meeting Style: Moves between 5 different locations
- Offers: Evidence disposal, contraband, intimidation, theft
- DEnvironmental Systems

### Day/Night Cycle
- **Full 24-hour cycle** - 10 minute real-time day/night
- **Time Periods**: Night, Sunrise (6-8 AM), Day (8 AM - 6 PM), Sunset (6-8 PM), Dusk (8-10 PM)
- **Dynamic Lighting**: Ambient light and rendering changes throughout the day
- **Visual Effects**: Color shifts for sunrise/sunset, enhanced darkness at night
- **Temperature System**: Affects gameplay and visual feedback

### Weather System
- **5 Weather Types**:
  - Clear: Perfect visibility, bright skies
  - Rain: Particle effects, reduced visibility slightly
  - Snow: Winter particle effects, slippery conditions
  - Fog: Heavy visual obstruction, mysterious atmosphere
  - Storm: Intense rain, dark skies, hazardous conditions
- **Dynamic Transitions**: Weather smoothly transitions over 10 seconds
- **Random Changes**: Weather changes every 1-3 minutes
- **Visual Overlays**: Color correction and particle effects per weather type

### Environment Features
- **Real-time Clock Display**: HUD shows current game time and period
- **Weather Display**: Current weather condition on HUD
- **Temperature Tracking**: Dynamic temperature based on time and weather
- **Color-Coded UI**: Time, weather, and temperature display different colors
- **Hazard System**: Storms and fog affect gameplay difficulty

## Roadworks & Construction Zones

### Roadworks System
- **8 Roadworks Zones** - Scattered randomly across the city
- **Obstacle Colliders** - Physical obstacles that block and slow vehicles
- **Visual Effects** - Orange construction zones with dust particle effects
- **Speed Reduction** - Vehicles reduced to 60% speed when passing through
- **Dynamic Spawning** - Zones spawn at least 50 units apart for variety
- **Activation/Deactivation** - Can be temporarily closed or reopened

## Pedestrian System

### Pedestrian AI
- **50+ NPCs** - Walking around the city continuously
- **5 Pedestrian Types**:
  - Students (around universities and schools)
  - Shoppers (around shops and shopping centres)
  - Workers (around libraries and restaurants)
  - Tourists (random locations)
  - Local Residents (all areas)
- **Dynamic Movement** - Pedestrians walk toward destinations and pause naturally
- **Color-Coded**: Each pedestrian type has a distinct color for identification

### Points of Interest (POI)
12 major gathering points with pedestrian concentrations:

**City Centre** - Main Shopping Centre, Chrome Wheels Shop, City Centre Park
**East Side** - Wolverhampton University, Campus Shops, Library  
**West End** - West End High School, West End Shopping, West End Park
**Northside** - Northside Shopping Complex, Primary School, Shopping District
**Southside** - South Mall, Restaurant District, Retail Parkous, always checking surroundings

## Fictional Business Names

**City Centre**: Chrome Wheels Auto, Neon Nights Club, Grease Pit Diner, Crimson Cab Station
**East Side**: Rusty's Chop Shop, The Velvet Underground, Iron Fist Pawn
**West End**: Speed Demons Motors, The Factory, Lucky's Electronics
**Northside**: Midnight Garage, Apex Club
**Southside**: Thunder's Garage, The Diamond, Black Market

## Next Steps

1. Implement player controller with WASD movement
2. Create vehicle system with physics
3. Develop combat mechanics
4. Build out mission system with full dialog trees
5. Create police/wanted system
6. Design and implement main map layout
7. Add sprites and visual polish
8. Implement audio system
