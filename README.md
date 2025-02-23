# SpellScape Sandbox ReadMe

## Platform and Development
- This project is built for **Android** using **Unity's AR Foundation template**.
- Unity's AR Foundation provides robust support for augmented reality features, including plane detection, image tracking, and gesture recognition, which are integral to the game's functionality.
- The template ensures cross-platform capabilities, facilitating future expansions to other AR-enabled platforms.

## Controls

### Scene 1 - Image Tracking
- **Select & Place Blocks:** Tap to select a block, and tap a slot to place the block into the slot.
- **Pronunciation:** Tap the pronunciation button to play the pronunciation audio.
- **Reset:** Tap the reset button to reset the word blocks.
- **Difficulty Scaling:** Words of higher difficulty are locked (their prefabs will not be shown). Complete 2 of 3 words of the current difficulty to unlock subsequent difficulties.

### Scene 2 - Sandbox
- **Plane Detection:** Move the device to scan for plane detection.
- **Choose Block:** Open the carousel menu and tap to choose the letter block you want to spawn.
- **Spawn Block:** Tap to spawn a block onto the detected plane.
- **Rotate Block:** Use a twisting gesture to rotate the block.
- **Move Block:** Drag objects with one finger to move them.

### Scene 3 - Challenge Area
- **Plane Detection & Start:** Move the device to scan for plane detection. Once a plane of suitable size is found, word prefabs will spawn, starting the timer and challenge.
- **Unscramble:** Drag the letter blocks to the sockets to unscramble the word.
- **Pronunciation:** Tap the pronunciation button to play the pronunciation audio.
- **Difficulty Scaling:** Words of a higher difficulty will not spawn until 3 words in the current difficulty are completed. In higher difficulties, words from previous difficulties do not count toward progression.

---

## Limitations / Bugs

### Image Tracking Scene
- **Reset Button Issue:** In the build version, resetting the blocks moves them to the world origin (0, 0, 0) instead of their initial spawn position, preventing the word from being completed correctly. *(This issue is not present in the editor.)*

### Challenge Area Scene
- **Plane Detection Speed:** Can sometimes be slow depending on lighting conditions or surface texture.
- **Offset Objects:** If the player moves too far from the initially detected plane, objects may appear offset or be difficult to interact with.
- **Spawn Locking:** Since the spawning of words is locked to the initial transform position of the first word instantiated, subsequent words might be impossible to solve if letter blocks/sockets are out of reach.
- **Recommendation:** Use a big open space and stand in the middle for better plane detection.

### Sandbox Scene
- **Plane Detection:** May be slow or inconsistent depending on environmental conditions.
- **Rotation Sensitivity:** Rotating blocks using the twisting gesture can sometimes be unresponsive if the gesture is not precise.
- **Movement Inconsistency:** Dragging to move blocks may be inconsistent due to their small hitbox.
- **Recommendation:** Use a big open space and stand in the middle for optimal plane detection.

---

## Puzzle Answers (Words used for Unscrambling Minigame)

### Easy Words (3-4 Letters)
- Cat*
- Dog*
- Sun*
- Hat
- Car
- Egg
- Red
- Ball
- Fish
- Duck

### Medium Words (5-6 Letters)
- Apple
- Chair*
- Table
- Tiger
- Happy
- Flower*
- Pencil
- Banana
- Rocket*
- Monkey

### Hard Words (7+ Letters)
- Rainbow
- Elephant
- Octopus*
- Strawberry*
- Backpack
- Butterfly*
- Dinosaur
- Chocolate
- Umbrella
- Helicopter

> *Words marked with an asterisk are also used in the Image Tracking Scene.*

---

## References / Credits

### English Dictionary
- [English Dictionary on GitHub](https://github.com/dwyl/english-words)

### Sound Effects
- **Success Sound Effect (Kids Cheering):** [Link](https://www.myinstants.com/en/instant/kids-cheering-yay/)
- **Incorrect Sound Effect (Buzzer):** [Link](https://www.myinstants.com/en/instant/extremely-loud-incorrect-buzzer-43033/)
- **Wood Block Sound Effect:** [Link](https://pixabay.com/sound-effects/wood-effect-254997/)
- **Pop/Socket Sound Effect:** [Link](https://pixabay.com/sound-effects/pop-39222/)

---

## Future Implementations
- **3DE Terrain:** Spawn the 3DE terrain when the plane is detected. This will make the game look more interesting. Currently, the props spawn in the correct location but the terrain plane spawns next to it.
- **More Words:** Additional words for both Image Tracking and Challenge Scenes.
- **Dictionary & TTS API Integration:** For the Sandbox Scene, so players can learn more about the words spawned (show the definition and pronunciation of words created).
