# Welcome to my VRC Tools repository!
This repo is to store a bunch of different tools ive written to do different things. Here is the VCC listing for the repo [Happys VRC Tools](https://www.matthewherber.com/Happys-VRC-tools/)

All of these tools are located under the VRC Packages tab

## Package Information
<details>
<summary>Package Window Consolidator</summary>

This package adds a tab along the top of the Unity editor that allows you to consolidate any other packages you have in that project into a single organized dropdown
an example dropdown would end up looking like this (Some public packages shown here)
- VRC Packages
    - Consolidate Packages
    - [OpenFlight](https://github.com/Mattshark89/OpenFlight-VRC)
        - Prefabs
            - ...
    - [Easy Quest Switch](https://github.com/JordoVR/EasyQuestSwitch)
    - [VRWorld Toolkit](https://github.com/oneVR/VRWorldToolkit)
        - Post Processing
            - ...
        - Custom Editors
            - ...
        - ...

Clicking on Consolidate Packages will attempt to pull everything into this structure that either isnt there or isnt the vrchat sdk
**Any packages that receive a update will require a re-run of the Consolidate Packages button. If a package breaks while using this function, all you need to do is remove the package that broke from the project and add it back in through the VCC. If you encounter a package that doesnt work/breaks this, let me know so I can fix it right away**
</details>
<details>
<summary>Editor Games</summary>

This is a simple package that just adds some fun random in-editor games to the Unity editor. Nuf said
</details>
<details>
<summary>Inspector Tweaks</summary>

This package aims to improve the inspectors of different components in the Unity editor. Currently it only modifies the Transform component, but more will be added in the future.

### Features added by this package
- Transform Component
    - Added a readout to show both the local and world position/rotation/scale of the object seperately
    - Added a button to copy the local and world position/rotation/scale of the object to the clipboard
    - Experimental mirroring controls
        - Mirror on X Y Z for local and world space

</details>
<details>
<summary>Shader Finder</summary>

This tool will find all the shaders in the scene, and select the relevant gameobjects that have materials using those shaders. If you are using a shader that locks itself into a 'optimized' mode, then you will need to search for it under Hidden, IE for Poiyomi it will be under Hidden -> Locked. Works with particle systems as well
</details>
<details>
<summary>Pretty JSON UI</summary>

This is a small UI helper element that will display JSON in a dropdown like format, akin to this website [JSON Viewer](https://codebeautify.org/jsonviewer). Go into the VRC Packages dropdown and inside you will find two prefabs. the JSON Manager prefab just spits out the expandable format, while the JSON Scrollable puts the whole thing into a scrolling section that will have scrollbars so you can keep the content size consistent
</details>
<details>
<summary>Light Probe Generator</summary>

This is a editor utility that will automatically create a light probe group and add light probe points to all relevant static game objects, along with adding points for spot, point and area lights. This is a improved version of [alexismorin's Light Probe Populator](https://github.com/alexismorin/Light-Probe-Populator), which took a slightly more primitive approach. You will find the control window in the VRC Packages tab in unity.
</details>
<details>
<summary>FPS Controller</summary>

This is a editor utility that allows for setting the FPS of the Unity editor, along with also setting the fixed delta time. This is useful for testing physics based things in the editor, as the default fixed delta time is not consistent with the VRC Client, as the VRC Client sets the fixed delta time to the Hz of the headset you are using. You will find the control window in the VRC Packages tab in unity. Presets for common headsets are included, but you can also set a custom FPS and fixed delta time.
<details>
<summary>Presets</summary>

- Oculus Rift
    - Hz: 90
- Oculus Rift S
    - Hz: 80
- Oculus Quest
    - Hz: 72
- Oculus Quest Pro
    - Hz: 90
- Oculus Quest 2 72Hz
    - Hz: 72
- Oculus Quest 2 90Hz
    - Hz: 90
- Oculus Quest 2 120Hz
    - Hz: 120
- HTC Vive
    - Hz: 90
- HTC Vive Pro
    - Hz: 90
- HTC Vive Pro 2
    - Hz: 120
- HTC Vive Cosmos
    - Hz: 90
- Valve Index 120Hz
    - Hz: 120
- Valve Index 144Hz
    - Hz: 144
- Windows Mixed Reality 60Hz
    - Hz: 60
- Windows Mixed Reality 90Hz
    - Hz: 90
- Pimax 5K
    - Hz: 90
- Pimax 8K
    - Hz: 80
- Pico 4 72Hz
    - Hz: 72
- Pico 4 90Hz
    - Hz: 90
- Pico 4 Pro
    - Hz: 90
</details>
</details>
<details>
<summary>GUID Regenerator</summary>

This is a editor utility that will automatically regenerate the GUID's for an entire projects asset folder. This is useful if you have a avatar base edit that you want to import into an existing project without modifying the original base files. KEEP IN MIND THIS APPLYS TO THE ENTIRE PROJECT IT IS RUN IN!!!!!
</details>
<details>
<summary>VRChat Build Size Viewer</summary>

Originally forked from [VRChat Build Size Viewer](https://github.com/MunifiSense/VRChat-Build-Size-Viewer) with permission from Munifi themselves to include it in this repository and make improvements to it.
</details>
<details>
<summary>Atlas Generator</summary>

Automatically generate atlas textures from a set of source textures, and keep it updated anytime the source textures change. Create the definition under the assets creation menu and then add the source textures to the list.
</details>
