/*
Make sure to read _README_FIRST_IMPORTANT.txt in the folder above this one
before trying to play the example scene.

NOTE: Unlike previous versions of this support package, you no longer have to 
assign a special manager. Just assign Adventure Creator's regular Demo scene 
managers for the Basement scene.

This folder contains a modified version of Adventure Creator's Demo scene that
replaces the AC conversation "IntroConv" with a Dialogue System equivalent.

It uses the AC() sequencer command to start IntroConv2 at the end of the 
conversation in "nowait" mode, which immediately releases control back to AC.

It also includes a simple quest example: "Find a Sword". This quest starts
active. When the player picks up the sword by triggering the Sword_Use
interaction, it runs a Lua action to update the quest state and refresh the
quest tracker HUD. You can also use the quest log window system, but you need
to implement the menus to open and close it.

There is also a simple interactive object, a Trash Can containing an 
obsolete model robot, Copper Pot. Copper Pot demonstrates barks (one-off
lines of dialogue during gameplay) and a conversation that uses the
sequencer command AC(TrashCanShake,nowait).
*/