# DrawingApp
This application is for making simple drawings and pictures. It uses the in built windows GDI API to make drawings of various shapes such as lines, circles,
quadrilaterals, Polygons, Arcs and Triangles. The application supports free hand drawings as shown in the sample screenshots. The application is still being revised to make better
use of memory as the current build uses a stack of Bitmaps to store the painting actions for the undo and redo functionalities. We will implement a version that stores the drawing commands
then pushes each command to the stack such that when the user undoes a painting action then the last painting action is removed from the top of the stack and then the code iterates from the command
at the bottom of the stack to the one at the top and draws them on the panel. This approach will be more memory efficient an will fix any performance issuses the program might be experiencing.
You can draw rectangle and fill with a color of your choice as shown in the screenshots below.
![image](https://user-images.githubusercontent.com/56290548/230437908-da5c86ca-4391-4bd6-a991-55b00e912928.png)
![image](https://user-images.githubusercontent.com/56290548/230437964-bdb55b70-845f-4730-afcc-2bd75face32d.png)
![image](https://user-images.githubusercontent.com/56290548/230438046-dead791f-2619-4b99-a574-b0e92a47392a.png)
![image](https://user-images.githubusercontent.com/56290548/230438147-304f43c6-1f9a-4825-a920-980757f28efd.png)
![image](https://user-images.githubusercontent.com/56290548/230438207-8ffb54ee-4b75-4838-b709-5260c81fb6a5.png)

