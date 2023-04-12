# SR Exterior Cities Patcher
Allows you to make a patch to make your entire modlist compatible with SR Exterior Cities by 1990SRider.


Currently, any mod with navmesh edits will still need some work in the Creation Kit. Any other mod can be patched without any further work.


Patchmakers can also make use of the navmesh functionalities. 
It cannot currently automatically resolve all navmesh issues. It can however make a copy of the navmesh in the city worldspace and place it in the Tamriel worldspace, at the correct place.
This can simplify the process a lot, as all there is left to do is to connect the navmesh at the city gates, and finalize the navmesh.


The Creation Kit will automatically generate the Navigation Info Map (or NAVI), which has no other way to be modified or altered (any slight modification to it causes a crash). 



Settings:

-enableNavmeshEdit: Set to true to make navmeshes edits. Recommended for patch makers only, will need a run in the Creation Kit.

-copyNavmeshes: Set to true to make copies rather than move navmeshes. False is unsupported/WIP

-ignoreOcclusion: don't move any occlusion panes, recommended to turn on to avoid conflicts of occlusion (those can cause CTD).

-debug: Leave it off, it's just for me to display some additional information.