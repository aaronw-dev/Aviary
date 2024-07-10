# What's Aviary?
Hello, it's me, Aaron. Thank you for showing interest in Aviary. If you want, stay and read what I've taken the time to read. If not, you of course don't have to. Okay, let's do this.

Because I've named every single one of my mildly aerospace-based projects around bird-themed things (Raven, Nest, etc.), I named this app Aviary. It's basically a 2D airfoil simulator, with lift and drag stuff, as well as a visualization of pressure gradient etc. 

Of course, this won't be a perfect app, since this app is built for me to be able to apply the concepts I'm learning while I'm teaching myself aerospace engineering and the concepts that go with it. 

I originally made this app in HTML, CSS, and JS. You can find it at [awdev.codes/dev/aviary](https://awdev.codes/dev/aviary). I switched to using C# and Raylib because it is SOOOOOO much faster and using actual datatypes is so much better than the stupid JavaScript (web and without TypeScript) `var`, `let`, and `const` variable types. 

Raylib is definitely the best tool I could have picked for the job, since it is very easy to hook into and can run at speeds of upu to 5000 FPS (in my testing). There is a whole built-in camera system, it's like a game engine without the bloat.

# Actions
I reserve the right to switch around the order of these list items if I need/want to.

This list started on `Thursday, May 23, 2024` for future reference.
## To Do
- add capability for changing angle of attack of wing
- simulate moments on the wing due to pressure

## Done
- add capability for changing speed of flow onto wing
- Use panel method to simulate flow speed over airfoil, then
- add lift simulation using Bernoulli's Law
- add scaling capability