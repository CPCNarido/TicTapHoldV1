# TicTapHoldV1

## Project Overview
TicTapHoldV1 is a Unity project designed for a level selection interface where users can scroll through levels and view their descriptions. The project includes a main menu for selecting levels and a dedicated scene for displaying level descriptions.

## File Structure
- **Assets/**
  - **Scripts/**
    - **Scroll.cs**: Handles scrolling functionality and button scaling based on the scrollbar's position.
    - **LevelManager.cs**: Manages level selection and scene transitions, passing the selected level index to the LevelDescription scene.
  - **Scenes/**
    - **MainMenu.unity**: The main menu scene where level selection buttons are displayed.
    - **LevelDescription.unity**: Displays the description of the selected level based on the index passed from the MainMenu scene.

## Features
- **Scrolling Mechanism**: Users can scroll through level buttons, which scale dynamically based on their position in the viewport.
- **Level Selection**: Clicking a level button will transition to the LevelDescription scene, passing the selected level index.
- **Level Description Display**: The LevelDescription scene will show relevant information about the selected level.

## Usage
1. Open the MainMenu scene to view the level selection interface.
2. Scroll through the levels using the scrollbar.
3. Click on a level button to view its description in the LevelDescription scene.

## Future Enhancements
- Add more levels and corresponding descriptions.
- Implement animations for scene transitions.
- Enhance the UI for better user experience.