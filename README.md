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
It cannot currently automatically resolve all navmesh issues. It can however prepare navmeshes for finalization in the Creation Kit. This can simplify the process a lot, as all there is left to do is to potentially connect the navmesh at the city gates and finalize the navmesh.

The patcher can also detect existing patches in the user's load order, to avoid any duplicates.

# Detailed navmesh patching instructions:

BEFORE YOU START: Your load order should be ONLY the vanilla files (no CC!!!), the mod(s) you want to patch and then SR Exterior Cities.esp (and any SREX patch required if you are making a combo patch of several mods).

1. Run Synthesis

- Make a separate group with the SR Exterior Cities Patcher and optionally the SynSetESL patcher below it. Call it "SREX AIO - Modname Patch" (exactly like this, do not replace "modname" with the actual mod's name at this stage). This step only needs to be done once, you can then use that setup to make multiple patches one after the other.
- In the settings of the patcher, tick the "PATCH MAKER ONLY: Enable navmesh modification" option, do not touch the other options.
- Run the patcher, close synthesis.
- Close synthesis and rename the patch according to our naming convention (replace Modname with the mod's name)


2. Run xEdit QuickAutoClean on it

3. Open the patch in xEdit (with all mods you have loaded like described in "before you start")

- Run the compress script on the plugin
- Run "Remove identical to previous override" on it.
- Save.
- Recommended: apply a filter for NAVM (middle column), to identify which cells have a navmesh record in them. Mark them down for the next step


4. Open the Creation Kit (CK), load up the patch as active

- go to each cell where a navmesh was edited, and visually check that the navmeshes look good and that there is no overlap. If there is an overlap, that needs more work, please notify us
- Open the navmesh tool. Finalize, save. Finalize a second time, save.
- Do this for every cell where a navmesh was edited!
- Double check visually that everything looks good. Some mods (like capital and JK) add stuff in the Tamriel worldspace that may end up clipping or looking weird. Delete those (next step is absolutely crucial if you do that, in order to properly disable those objects instead of deleting them)


5. Run xEdit QAC again

6. Open the patch again in xEdit

- check everything looks okay.
- Add the partial form flags to the Tamriel persistent cell (always) and Tamriel worldspace (IF the mod doesn't edit the max height data etc, I'm not 100% sure myself)
- Delete the city worldspaces if there are any left.
- Right click on the plugin > Add Masters... > select all vanilla files (and individual patches when making combo patches), then Sort Masters to match load order
- In the header, add your name to the author field
- In the header, add the version of SR and that of the mod you're patching for in the description field
- Use 7zip to make it a .7z file, and post it in SRider's discord, in the #⁠⁠community-patches channel.


7. We'll review/QA it and add it to the next patch hub iteration, thank you for your contribution! 


# Settings:
-checkLO: Set to true to do a quick check for the order of your SREX patches

-ignoreOcclusion: don't move any occlusion panes, recommended to turn on to avoid conflicts of occlusion (those can cause CTD).

-checkPatches: Set to true to remove all records already patched in the SREX patch hub (takes longer but will produce a cleaner patch). Only turn off if you know how to clean those in xEdit yourself.

-enableNavmeshEdit: PATCH MAKER ONLY: Enable navmesh modification. Recommended for patch makers only, will need a run in the CK"

-moveNavmeshes:PATCH MAKER ONLY: move all SREX + patches navmeshes down to -30000 (experimental)

-debug: Leave it off, it's just for me to display some additional information.
