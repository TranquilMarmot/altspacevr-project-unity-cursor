# AltspaceVR Programming Project - Unity Cursor

#### Nate Moore

# Part 1 - 3D Cursor

Pretty simple; the cursor's location is represented by a quaternion that gets rotated by the player's mouse. Ray cast gets sent out from the camera in the direction of the quaternion, and if it hits something it places the cursor there. If it doesn't hit anything, it puts the cursor on a sphere around the player.

As for rendering the cursor, rather than writing my own shader I used Unity 5's built-in default shader, and set the "emission" value to be the desired color of the cursor. The cursor is rendered with its own camera that *just* renders the cursor, so that it is always on top no matter what. The default camera doesn't render the cursor at all.

I added right-click as an extra modifier to allow mouse look (along with the defaulted shift keys), and I modified the escape key's behavior to just unlock the mouse, rather than fulling quitting the game. Unfortunately, this means that to exit you have to do an Alt+F4 (or Command+Q if you roll with Apple). Or run it in windowed mode, unlock the mouse, and close the window normally.

# Part 2 - Enhancements

I had fun with this! As you start the scene, you'll notice big red buttons on the ground in front of you with labels floating on top of them (who doesn't like pressing big red buttons?!)

They have a couple functions:
- Pull: This pulls the selected object *towards* you
- Push: This pushes the selected object *away* from you
- Grab: Grabs an object so it can be moved around
- Fling: Allows the player to fling an object around using the cursor
- Clone: Clones the selected object
- Explode: Causes an explosion at the selected object, sending it into the air and anything around it flying. Also lets out a neat particle effect, using Unity's default particle system.
- Disable Gravity: Turn on "space simulator" mode and have objects float around! Doesn't effect the player; the player must have special space boots.
- Flip Gravity: Turn the world upside down! Makes everything float towards the ceiling

All of the code for the object manipulation is in ForceModule.cs, which is attached to the main camera along with the cursor. The button code is in WorldButton.cs, which is extended by ToggleableWorldButton.cs.

You can only have one "mode" active at a time, so all the buttons (aside from the gravity mainpulators) act as radio buttons and only one can be pressed down at a time (the active mode is displayed in the top-left corner of the screen). The code for the radio list is in ToggleableWorldButtonRadioList.cs, and basically just makes sure only one of its children is pressed at any given time.

I also added a cool glowing transparent bouncing ball to the scene, because I got bored with the lamps.

Fun stuff:
- Make a bunch of clones. Blow them up.
- Turn off gravity. Blow stuff up.
- Reverse gravity, then un-reverse it and turn it off before everything hits the ground. Then blow stuff up.
- Make a bunch of clones, reverse gravity, un-reverse gravity, turn gravity off, and blow stuff up.


No known issues or bugs. Let me know if you find any!!! The project will probably only work in Unity 5, but the "Builds" folder has an x86 build of it.

##Acknowledgements

*Assets used in this project are from* [Free Furniture Props](https://www.assetstore.unity3d.com/en/#!/content/8822)


