# Unity Scene Setup Instructions

## Creating the Scenes

### 1. Create Main Menu Scene
1. Open Unity and go to **File > New Scene**
2. Save it as `MainMenuScene` in your Assets/Scenes folder
3. Create an empty GameObject named "MainMenuManager"
4. Add the `MainMenuSetup` script to it
5. Build Settings: Add this scene first (index 0)

### 2. Create Game Scene
1. Go to **File > New Scene**
2. Save it as `GameScene` in your Assets/Scenes folder
3. Create an empty GameObject named "GameManager"
4. Add the `GameSceneSetup` script to it
5. Also add a `Camera` positioned at (500, 500, -10) looking at the map center
6. Set Camera Size to 400
7. Build Settings: Add this scene second (index 1)

### 3. Set Build Order
1. Go to **File > Build Settings**
2. Ensure both scenes are added
3. Drag `MainMenuScene` to the top (it will load first)
4. Close Build Settings

### 4. Scene Configuration

**MainMenuScene Setup:**
- Main Camera: Position (0, 0, -10), Size 5, Background Color: Black
- Add "MainMenuManager" GameObject with MainMenuSetup script

**GameScene Setup:**
- Main Camera: Position (500, 500, -10), Size 350
- Add "GameManager" GameObject with GameSceneSetup script

### 5. Testing
- Press Play in Unity
- The Main Menu should appear with "CAR BASH WOLVERHAMPTON" title
- Use W/S to navigate and Enter to select
- Selecting a game mode will load the Game Scene