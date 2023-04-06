# DrawingApp
This application is for making simple drawings and pictures. It uses the in built windows GDI API to make drawings of various shapes such as lines, circles,
quadrilaterals, Polygons, Arcs and Triangles. The application supports free hand drawings as shown in the sample screenshots. The application is still being revised to make better
use of memory as the current build uses a stack of Bitmaps to store the painting actions for the undo and redo functionalities. We will implement a version that stores the drawing commands
then pushes each command to the stack such that when the user undoes a painting action then the last painting action is removed from the top of the stack and then the code iterates from the command
at the bottom of the stack to the one at the top and draws them on the panel.
