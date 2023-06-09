# DrawingApp
This application is for making simple drawings and pictures. It uses the in built windows GDI API to make drawings of various shapes such as Lines, Circles,
Quadrilaterals, Polygons, Arcs and Triangles. The application supports free hand drawings as shown in the sample screenshots. The application is still being revised to make better
use of memory as the current build uses a stack of Bitmaps to store the painting actions for the undo and redo functionalities. We will implement a version that stores the drawing commands
then pushes each command to the stack such that when the user undoes a painting action then the last painting action is removed from the top of the stack and then the code iterates from the command
at the bottom of the stack to the one at the top and draws them on the panel. This approach will be more memory efficient an will fix any performance issuses the program might be experiencing.
You can draw rectangle and fill with a color of your choice as shown in the screenshots below.
![image](https://user-images.githubusercontent.com/56290548/230437908-da5c86ca-4391-4bd6-a991-55b00e912928.png)
![image](https://user-images.githubusercontent.com/56290548/230437964-bdb55b70-845f-4730-afcc-2bd75face32d.png)
![image](https://user-images.githubusercontent.com/56290548/230438147-304f43c6-1f9a-4825-a920-980757f28efd.png)
![image](https://user-images.githubusercontent.com/56290548/230438207-8ffb54ee-4b75-4838-b709-5260c81fb6a5.png)
You can also specify the Brush style to use for drawing the figures and so far the program supports Brick, Backward Diagonal, Plaid, Cross, Checker Board, Diamond, Sphere and Wave. The screenshot below shows all of them at a glance.
![image](https://user-images.githubusercontent.com/56290548/230440131-1c33caa0-8c9e-4915-979a-0877469ed38a.png)
An example of a free hand drawing made using the Cross brush type is shown below.
![image](https://user-images.githubusercontent.com/56290548/230440365-ba5349ad-57fc-4bf5-80de-30cea8358bc8.png)
An example of a free hand drawing made using the Checkerboard brush type is shown below.
![image](https://user-images.githubusercontent.com/56290548/230441278-c60563bc-4d5b-45ce-a51c-fb47acd356a3.png)

You can also draw polygons and fill them with a fill color of choice, check the screenshow below for more details.

![image](https://user-images.githubusercontent.com/56290548/230587962-fe9d49c6-ff4f-4a1e-abe4-b392ee6225b7.png)
You can also draw an array of infinitely connected lines, after marking the points on the panel then you hit the Connect button to draw lines connecting all of those points.
![image](https://user-images.githubusercontent.com/56290548/230591924-0cf7d09a-0cb0-4bad-ba7c-f36e12b56c04.png)

