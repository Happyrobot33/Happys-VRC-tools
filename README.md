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