Nick Holbrook's Quick-Unity-Assessment-Photon-FPS

This is a 6 hour Unity Assessment Project that I was asked to build for showcasing my approach using SOLID Principles with a Data Oriented Approach using Scriptable Objects inside of the Unity Engine. The project is focused on providing a simple First Person Shooter User Experience for both a single player game and a multiplayer game. 

The Single Player Mode scene has been designed to be built separately from the Multiplayer Player Mode scene, so that the single player mode can be built without any of the Photon libraries being included in the build.

Though both modes share a lot of the same functionality, the Game Manager and User Manager scripts/prefabs for each mode have been split up to allow for the Photon libraries to be stripped from the Single Player Mode build. In a game that would include both modes, I would merge these assets into one set of scripts/prefabs to allow for a more optimized development approach.

Separating your data from your functionality allows your functionality to be decoupled from each other; this ensures functionality can be unit tested and easily evolved without the risk of breaking other functionality. This approach also ensures an optimal development velocity due to the power of scriptable objects as data containers within the Unity Engine. You can add, remove, and switch out functionality without worrying about dependencies to other functionality in the Unity project. While the Unity Editor is playing, changes within the scriptable objects can be tested without stopping the Unity Editor; these changes will remain in place even after stopping the Unity Editor.

I set up most of my references within my scripts to avoid dragging references into the Inspector Window, which reduces the chance of merge conflicts between other developers. I also ensured each gameObject utilized in a scene is turned into a prefab, so developers can work within the same scene without generating merge conflicts within the Unity scene file.

Code Readability is a very important topic to me. I believe code should be written to allow other people to quickly understand it, instead of focusing on over optimizing the code for the compiler. Early function exits, no magic numbers, and well named variables/functions are three ways I ensure code is easily read by other humans.
