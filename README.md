# SR Exterior Cities Patcher
Allows you to make a patch to make your entire modlist compatible with SR Exterior Cities by 1990SRider.

Please go download & endorse on nexus if you found the patcher useful! It'd help some :)
https://www.nexusmods.com/skyrimspecialedition/mods/89066


Any mod without navmesh can be patched without any additional step. Just run the patcher with the default settings.
IF a mod makes Navmesh edits to the interior city worldspace, it needs a patch made in the Creation Kit. Either use one that is already made, or make one following the instructions below.

Currently, any mod with navmesh edits in the city worldspaces will still need some work in the Creation Kit. This is sadly unavoidable, as there is no way to generate the Navigation Info Map (or NAVI) outside of the Creation Kit. The Creation Kit will automatically generate the Navigation Info Map (or NAVI), which has no other way to be modified or altered (any slight modification to it causes a hard crash). 

Mods that are concerned by this are typically major city overhauls or mods that make major edits to a city worldspace, for example (non exhaustive list!):
JK's Skyrim
Dawn of Skyrim
Capital cities
Legacy of the Dragonborn's museum in solitude
etc.

 Any other mod can be patched without any further work (examples include but are not limited to: Lux Orbis, Lux Orbis Master, free CC mods, ...)

HOWEVER: Patchmakers can also make use of the navmesh functionalities. 
It cannot currently automatically resolve all navmesh issues. It can however make a copy of the navmesh in the city worldspace and place it in the Tamriel worldspace, at the correct place. This can simplify the process a lot, as all there is left to do is to connect the navmesh at the city gates, possibly link to (or delete) the navmesh from the main mod, and finalize the navmesh.

Important note: the next update of SR Exterior Cities will allow me to improve on the process a lot, basically simplifying the work immensely.


# Detailed navmesh patching instructions:

0. Heavily recommended: only enable the mod/mods you want to patch and SR Exterior Cities.esp
1. Tick the "Enable Navmesh modification" setting
2. Leave the copy navmeshes ticked
3. Run the patcher
4. Change the ID of the copied navmeshes in xEdit so that they match the ID in SR Exterior Cities.esp (this will hopefully be done automatically soon)
5. Open the Creation Kit, load your newly made patch
5. For each worldspace: remove the navmeshes you don't need (ONLY from the newly created ones! It will cause crashes otherwise!), or move the SR Exterior Cities navmesh way lower so they don't confuse NPCs (See this tutorial: Navmesh Cutting (by Darkfox127) for detailed instructions)
6. If needed, connect the navmeshes at the city gates
7. Finalize the navmesh
8. Save your patch, it is done! The CK will automatically generate the NAVI for you.
9. Test ingame
10. Share your patch (on SRider's discord, if you like, so that it can be used in the patch hub)


# Settings:

-enableNavmeshEdit: Set to true to make navmeshes edits. Recommended for patch makers only, will need a run in the Creation Kit.

-copyNavmeshes: Set to true to make copies rather than move navmeshes. False is unsupported/WIP

-ignoreOcclusion: don't move any occlusion panes, recommended to turn on to avoid conflicts of occlusion (those can cause CTD).

-debug: Leave it off, it's just for me to display some additional information.
