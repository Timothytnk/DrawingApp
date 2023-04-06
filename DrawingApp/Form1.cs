using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.WebSockets;
using System.Timers;

namespace DrawingApp;

public partial class Form1 : Form
{
    //list to hold the draw commands
    private Stack<DrawItem> drawCommands = new Stack<DrawItem>();

    //variable to hold the backup color of the panel
    private Color color;

    //declare a pen for drawing the shapes in the class
    private Pen drawingPen;

    //condition to clear the panel to listen for click events
    private bool listening = false;

    //declare a string list to hold the different shapes 
    private List<string> shapes = new List<string>
    {
        "Free Hand",
        "Arc",
        "Line",
        "Circle",
        "Triangle",
        "Quadrilateral",
        "Polygon",
    };

    //list to hold the menu strip items
    private List<string> toolItems = new List<string>()
    {
        "Draw",
        "File",
        "Clear",
        "Simulate"
    };

    //define a padding for the menu strip items
    private Padding itemPadding = new Padding(15);

    //this string holds the shape being drawn
    private string drawItem = "Free Hand";

    //picture box for displaying the color of choice
    private PictureBox colorBox;

    //picture box for drawing on
    private PictureBox drawingPanel;

    //disgusting indexer to help you count the number of times the 
    //picture box was clicked
    private int indexer = 0;

    //list to hold the points
    private List<Point> pointsClicked = new List<Point>();

    //combo box for holding the pen sizes in even numbers
    private ComboBox penSizes;

    //queue object for enqueuing and dequeue bitmap
    private Stack<Bitmap> paintStates = new Stack<Bitmap>();

    //queue object for adding the redo states
    private Stack<Bitmap> redoStates = new Stack<Bitmap>();
    //initialize the width of the pen
    private int penWidth = 4;
    //picture box for changing the color of the panel
    private PictureBox panelColor;

    //declare a point to hold the previous mouse location of the mouse move event
    private Point previousMouseLocation = Point.Empty;

    //points for updating the snake's position
    private Point startP, endP;
    //we need a combo box for letting the user select a different brush type
    private ComboBox comboBox;
    private List<string> brushes = new List<string>
    {
        "Normal",
        "Brick",
		"Backward Diagonal",
        "Plaid",
        "Cross",
        "Checker Board",
        "Diamond",
        "Sphere",
        "Wave"
	};
    //declare a default brush style
    private HatchBrush brush;
    //move the menu strip declaration to the class for method visibility
    MenuStrip menuStrip;
    public Form1()
    {
        InitializeComponent();
        //set a title for the form
        Text = "Simple Drawing Utility";
        //set the window to display full screen
        WindowState = FormWindowState.Maximized;
        //disable the maximize button
        MaximizeBox = false;
        //programatically add a menu strip to the window 
        menuStrip = new MenuStrip();
        //set the background color of the strip
        menuStrip.BackColor = Color.RoyalBlue;
        menuStrip.ForeColor = Color.White;  
        //dock it to the left
        menuStrip.Dock = DockStyle.Left;
        //define a padding for each menu strip item
        menuStrip.Padding = itemPadding;
        //add the items
        var drawItem = new ToolStripMenuItem(toolItems[0]);
        var fileItem = new ToolStripMenuItem(toolItems[1]);
        var clearItem = new ToolStripMenuItem(toolItems[2]);
        var simulate = new ToolStripMenuItem(toolItems[3]);
        var menuItems = new ToolStripMenuItem[] { drawItem, fileItem, clearItem, simulate };
        foreach (var tool in menuItems)
        {
            tool.Margin = itemPadding;
            tool.Padding = new Padding(10);
            //set the font family
            tool.Font = new Font("Corbel",10);
        }

        //what happens when the clear iteis clicked
        clearItem.Click += ClearItemOnClick;
        //what happens when the simulate item is clicked
        simulate.Click += SimulateOnClick;
        menuStrip.Items.AddRange(menuItems);
        //add the drop down items to the first menu strip item
        var free = new ToolStripMenuItem(shapes[0]);
        var arc = new ToolStripMenuItem(shapes[1]);
        var line = new ToolStripMenuItem(shapes[2]);
        var circle = new ToolStripMenuItem(shapes[3]);
        var triangle = new ToolStripMenuItem(shapes[4]);
        var quad = new ToolStripMenuItem(shapes[5]);
        var poly = new ToolStripMenuItem(shapes[6]);
        var arr = new ToolStripMenuItem[] { free, arc, line, circle, triangle, quad, poly };
        drawItem.DropDownItems.AddRange(arr);
        //add a listener for the drop down  menu items
        drawItem.DropDownItemClicked += DrawItemOnDropDownItemClicked;
        //add the strip to the form
        Controls.Add(menuStrip);
        //Point to add a new picture box to the application
        Point point = new Point(212, 47);
        //create a new picture box
        colorBox = new PictureBox();
        colorBox.Width = 50;
        colorBox.Height = 50;
        //call method to fill a color into the picture box
        FillColor(colorBox);
        colorBox.Location = point;
        //what happens when the picture box is clicked
        colorBox.Click += ColorBoxOnClick;
        Controls.Add(colorBox);
        //add a label to the picture box above
        var point1 = new Point(196, 17);
        var label = new Label();
        label.Text = "Pen Color";
        label.Location = point1;
        label.ForeColor = Color.White;
        label.BackColor = Color.Transparent;
        label.Width = 200;
        Controls.Add(label);
        //define the label for setting the pen size
        var label2 = new Label();
        label2.Text = "Set Pen Size";
        label2.Location = new Point(400, 17);
        label2.Width = 200;
        label2.ForeColor = Color.White;
        label2.BackColor = Color.Transparent;
        Controls.Add(label2);
        //add the label for filling the background color
        var label3 = new Label();
        label3.Width = 200;
        label3.Text = "Panel Color";
        label3.Location = new Point(620, 17);
        label3.ForeColor = Color.White;
        label3.BackColor = Color.Transparent;
        Controls.Add(label3);
        //add a picture box for changing the fill of the panel
        panelColor = new PictureBox();
        panelColor.Width = 50;
        panelColor.Height = 50;
        panelColor.Location = new Point(640, 47);
        //what happens when the color panel is clicked
        panelColor.Click += PanelColorOnClick;
        Controls.Add(panelColor);
        //run a process to color it
        ColorPanel(panelColor);
        //add a label for the brush type
        var label4 = new Label();
        label4.ForeColor = Color.White; 
        label4.BackColor = Color.Transparent;   
        label4.Text = "Brush Style";
        label4.Location = new Point(830, 17);
        label4.Width = 200;
        Controls.Add(label4);
        //instantiate the combo box 
        comboBox = new ComboBox();
        comboBox.Location = new Point(830, 47);
        //populate the combo box
        foreach(var item in brushes)
        {
            comboBox.Items.Add(item);
        }
        //set a default selected index
        comboBox.SelectedIndex = 2;
        //set te font name for thecombo box items
        comboBox.Font = new Font("Corbel", 10);
		//event handler for changing the type of the brush
		comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        Controls.Add(comboBox);
        //add the combo box to the form
        penSizes = new ComboBox();
        penSizes.Location = new Point(400, 47);
        PopulateComboBox(penSizes);
        //what happens when an item on the combo box is changed
        penSizes.SelectedIndexChanged += PenSizesOnSelectedIndexChanged;
        Controls.Add(penSizes);
        //populate the combo box

        var point3 = new Point(162, 139);
        drawingPanel = new PictureBox();
        drawingPanel.Location = point3;
        //define the size of the drawing panel
        var psize = new Size(800 * 2, 800);
        drawingPanel.Size = psize;
        //set the background color to white
        drawingPanel.BackColor = Color.White;
        //set the picture mode to stretch
        drawingPanel.SizeMode = PictureBoxSizeMode.StretchImage;
        //listen for mouse click events on the panel
        drawingPanel.MouseClick += DrawingPanelOnMouseClick;
        //to prevent the picture box from returning a null for the image we se pixels
        SetPixel(drawingPanel);
        //what happen when the mouse is held down and moved around on the drawing panel
        drawingPanel.MouseMove += DrawingPanelOnMouseMove;
		drawingPanel.Focus();
		Controls.Add(drawingPanel);
        //instantiate the pen with a default width of 5
        drawingPen = new Pen(Color.RoyalBlue, penWidth);
        MouseClick += OnMouseClick;
        //this form will receive keyboard events before any other control does
        KeyPreview = true;
        //override on key down event to listen for undo, redo, paste and copy events
        KeyDown += OnKeyDown;
        //subscribe to a form on load event
        Load += OnLoad;
        //subscribe to an event handler that handles when the size of the form changes
        Resize += OnResize;
		//listen when the left mouse button is released on the drawing panel
		drawingPanel.MouseUp += DrawingPanel_MouseUp;
		//listen for the on mouse down drawing panel event
		drawingPanel.MouseDown += DrawingPanel_MouseDown;
    }

	private void ComboBox_SelectedIndexChanged(object? sender, EventArgs e)
	{
		//switch the item
        switch(comboBox.SelectedIndex) {
            case 0:
                var normal = new SolidBrush(drawingPen.Color);
                drawingPen.Brush = normal;
               
                break;
            case 1:
                //set the brush to calligraphy brush
                var brick = new HatchBrush(HatchStyle.HorizontalBrick,drawingPen.Color); 
                drawingPen.Brush = brick;
                brush = brick;
                break;
            case 2:
                var backward = new HatchBrush(HatchStyle.BackwardDiagonal, drawingPen.Color);
                drawingPen.Brush = backward;
                brush = backward;   
                break;
            case 3:
                var plaid = new HatchBrush(HatchStyle.Plaid, drawingPen.Color);
                drawingPen.Brush = plaid;
                brush = plaid;
                break;
                case 4:
                var cross = new HatchBrush(HatchStyle.Cross, drawingPen.Color);  
                drawingPen.Brush = cross;
                brush = cross;  
                break;
            case 5:
                var checkerboard = new HatchBrush(HatchStyle.SmallCheckerBoard, drawingPen.Color);
                drawingPen.Brush = checkerboard;
                brush = checkerboard;
                break;
            case 6:
                var diamond = new HatchBrush(HatchStyle.SolidDiamond, drawingPen.Color); 
                drawingPen.Brush = diamond;  
                brush = diamond;
                break;
            case 7:
                var sphere = new HatchBrush(HatchStyle.Sphere, drawingPen.Color); 
                drawingPen.Brush = sphere;
                brush = sphere;
                break;
            case 8:
                var wave = new HatchBrush(HatchStyle.Wave, drawingPen.Color);
                drawingPen.Brush = wave;
                brush = wave;
                break;
            case 9:
                var confetti = new HatchBrush(HatchStyle.LargeConfetti, drawingPen.Color);
                drawingPen.Brush = confetti;
                brush = confetti;
                break;
        }

	}

	private void DrawingPanel_MouseDown(object? sender, MouseEventArgs e)
	{
		previousMouseLocation = e.Location;
	}

	private void DrawingPanel_MouseUp(object? sender, MouseEventArgs e)
	{
		//check if it is the left mouse button
		if (e.Button == MouseButtons.Left && drawItem == "Free Hand")
		{
		
			//the user is not drawing on free hand, push the bitmap to the stack
			var bitmap = new Bitmap(drawingPanel.Width, drawingPanel.Height);
			//opena  graphics context with the bitmap
			using (var graphics = Graphics.FromImage(bitmap))
			{
				var location = PointToScreen(drawingPanel.Location);
				//copy the image from the panel to the bitmap
				graphics.CopyFromScreen(location, Point.Empty, drawingPanel.Size);
			}
			//push the bitmap to the stack
			paintStates.Push(bitmap);

		}
	}

	private void SimulateOnClick(object? sender, EventArgs e)
    {
        //create a new line segment
        startP = new Point(drawingPanel.Location.X + 30, drawingPanel.Location.Y + 30);
        var segmentWidth = 30;
        var interval = 10;
        endP = new Point(startP.X + segmentWidth, startP.Y);
        //draw the snake in the initial position on the form
        drawingPanel.CreateGraphics().DrawLine(drawingPen, startP, endP);
        //start a timer to move the snake
        var timer = new System.Timers.Timer(1000);
        timer.Start();
        timer.Elapsed += delegate(object? o, ElapsedEventArgs args)
        {

            if (args.SignalTime.Ticks > 3)
            {

                //update the x co ordinate of the first point and add the interval
                startP.X += interval;
                //update the end point to include the width
                endP.X += segmentWidth;
                //start moving the snake
                //invalidate the form
                Thread.Sleep(1000);
                //update the first point
                drawingPanel.CreateGraphics().DrawLine(drawingPen, startP, endP);
            }
        };
    }

    private void DrawingPanelOnMouseMove(object? sender, MouseEventArgs e)
    {
        //check the mouse button first
        if (e.Button == MouseButtons.Left && drawItem == "Free Hand")
        {
           
            var currentLocation = e.Location;
            drawingPanel.CreateGraphics().DrawLine(drawingPen, previousMouseLocation, currentLocation);
            //swap the variables
            previousMouseLocation = currentLocation;
            //reset the variables
            pointsClicked.Clear();
            indexer = 0;
            //check if the mouse is released and push the bitmap to the stack
        }
    }
    


private void PanelColorOnClick(object? sender, EventArgs e)
    {
        //show a color dialog
        var colorDialog = new ColorDialog();
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            //get the color
            var color = colorDialog.Color;
            //back up this color in some variable inside the class
            this.color = color;
            var m = Task.Run(() =>
            {
                var map = new Bitmap(drawingPanel.Width, drawingPanel.Height);
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        map.SetPixel(x,y,color);
                    }
                }

                drawingPanel.Image = map;
            });
            m.Wait();
            //stop the watch
           
            //update the box that was clicked the change the color
            var t = Task.Run(() =>
            {
                var bitmap = new Bitmap(panelColor.Width, panelColor.Height);
                for (int u = 0; u < bitmap.Width; u++)
                {
                    for (int j = 0; j < bitmap.Width; j++)
                    {
                        bitmap.SetPixel(u,j,color);
                    }
                }

                panelColor.Image = bitmap;
            });
            t.Wait();
        }
        
    }

    private void ColorPanel(PictureBox pictureBox)
    {
        var color = Color.White;
        var map = new Bitmap(pictureBox.Width, pictureBox.Height);
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                map.SetPixel(x,y, color);
            }
        }
        pictureBox.Image = map;
        //push the bitmap to the stack
        //paintStates.Push(map);
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            //do nothing
        }

        if (WindowState == FormWindowState.Maximized)
        {
            //get the last bitmap pushed to the stack and display in the panel
            //var map = paintStates.Peek();
        }
    }

    private void OnLoad(object? sender, EventArgs e)
    {
		//get the  background image of the app
		Color color1 = Color.DarkSlateGray;
		Color color2 = Color.DarkSalmon;
		LinearGradientBrush brush = new LinearGradientBrush(
			this.ClientRectangle,
			color1,
			color2,
			LinearGradientMode.Vertical);

		// Set the form's background color to the gradient brush
		this.BackColor = Color.White;
		this.BackgroundImage = new Bitmap(this.Width, this.Height);
		using (Graphics g = Graphics.FromImage(this.BackgroundImage))
		{
			g.FillRectangle(brush, this.ClientRectangle);
            //apply gradient to the menu strip
            g.FillRectangle(brush, menuStrip.ClientRectangle);
		}

	}

    private void ClearItemOnClick(object? sender, EventArgs e)
    {
      //invalidate everything
      drawingPanel.Invalidate();
      SetPixel(drawingPanel);
      //read the color in the panel and re assign
      var panColor = this.color;
      var bitmap = new Bitmap(drawingPanel.Width, drawingPanel.Height);
      for (int x = 0; x < drawingPanel.Width; x++)
      {
          for (int y = 0; y < drawingPanel.Height; y++)
          {
              bitmap.SetPixel(x,y,panColor);
          }
      }

      drawingPanel.Image = bitmap;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        //listen for paste events
        if (e.Control && e.KeyCode == Keys.V)
        {
            //get the image from the screenshot and assign ot to the picture box
            var image = Clipboard.GetImage();
            if (image != null)
            {
                //assign it to the picture box
                drawingPanel.Image = image;
            }
        }
        if (e.Control && e.KeyCode == Keys.Z)
        {
            //the user is trying to undo the last paint action
            //if the user has performed only one draw command  clear
            if (paintStates.Count == 0)
            {
                MessageBox.Show("No more bitmaps to undo");
            }
            if (paintStates.Count == 1)
            {
                SetPixel(drawingPanel);
            }
            if (paintStates.Count > 1)
            {
                //add this bitmap to the redo stack before removing it
                redoStates.Push(paintStates.Peek());
                //remove the lastly added command to the stack
                paintStates.Pop();
                drawingPanel.Image = paintStates.Peek();
            }
        }
        if(e.Control && e.KeyCode == Keys.Y)
        {
            //the user is trying to redo the paint events
            //get the object pushed out and display it
            if (redoStates.Count > 1)
            {
                paintStates.Push(redoStates.Peek());
                //reverse the redo states
                drawingPanel.Image = redoStates.Pop();
            }
        }
        if (e.Control && e.KeyCode == Keys.S)
        {
            //the user is trying to save the bitmap, get the bitmap
            var bitmap = new Bitmap(drawingPanel.Width, drawingPanel.Height);
            //get the painting area rect
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var panelLocation = PointToScreen(drawingPanel.Location);
                graphics.CopyFromScreen(panelLocation, Point.Empty, drawingPanel.Size);
            }
            var folderBrowser = new SaveFileDialog();
            //set the initial directory to the pictures folder
            var initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            folderBrowser.InitialDirectory = initialDir;
            //set a title for te folder browser dialog
            folderBrowser.Title = "Image Save Location";
            //set the filter for saving image file
            folderBrowser.Filter = "Image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp|All files (*.*)|*.*";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                //get the file name and save the bitmap
                try
                {
                    bitmap.Save(folderBrowser.FileName, ImageFormat.Png);
                    MessageBox.Show("Drawing saved successfully");
                    e.Handled = true;
                }
                catch (Exception mm)
                {
                    MessageBox.Show(mm.Message);
                }
                
                
            }
        }
		if (e.Control && e.KeyCode == Keys.K)
        {
            //try to remove the last command from the top of the stack and redraw all items
            drawCommands.Pop();
            //for all the remaining commands, iterate from the first which is the last
            //item in the stack
            drawCommands.Reverse(); 
            foreach(var command in drawCommands)
            {
                if(command.Item == "Line")
                {
                    //get the first and second points
                    var first = command.Points[0];
                    var second = command.Points[1];
                    drawingPanel.CreateGraphics().DrawLine(drawingPen, first, second);
                }
            }
        }


	}

    void SetPixel(PictureBox box)
    {
        var map = new Bitmap(box.Width, box.Height);
        var cream = Color.White;
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                map.SetPixel(x,y,cream);
            }
        }

        box.Image = map;
    }
    private void PenSizesOnSelectedIndexChanged(object? sender, EventArgs e)
    {
       //get the new size and update the drawing pen width
       var width = penSizes.SelectedItem.ToString();
       //parse an integer out of the selection and use it
       var rWidth = Int32.Parse(width);
       drawingPen.Width = rWidth;
        //update the pen width variable inside the class
        penWidth = rWidth;
        //clear the focus from the handle
       drawingPanel.Focus();
    }

    //this method fills up the combo box
    void PopulateComboBox(ComboBox box)
    {
        //minimum pen size is 2
        for (int i = 2; i <= 96; i += 2)
        {
            box.Items.Add(i);
        }
    }
    private void DrawingPanelOnMouseClick(object? sender, MouseEventArgs e)
    {
       
       
        //increase the indexer if the app is listening
        if (listening)
        {
            indexer += 1;
        }
        //get the point clicked
        var point = new Point(e.X, e.Y);
        pointsClicked.Add(point);
        //add the point to a list
        if (indexer == 2 && drawItem == "Line")
        {
            //push object and points to stack
            drawCommands.Push(new DrawItem("Line",pointsClicked));
            //get the context and draw the line
            drawingPanel.CreateGraphics().DrawLine(drawingPen,pointsClicked[0],pointsClicked[1]);
            //reset the indexer
            indexer = 0;
            pointsClicked.Clear();
            //save the bitmap to a queue
            var bitmap = new Bitmap(drawingPanel.Width, drawingPanel.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var panelLocation = PointToScreen(drawingPanel.Location);
                graphics.CopyFromScreen(panelLocation, Point.Empty, drawingPanel.Size);
            }
            paintStates.Push(bitmap);
        }
        //draw a circle
        if (indexer == 2 && drawItem == "Circle")
        {
            //get the first point
            var center = pointsClicked[0];
            //get the point on the perimeter
            var border = pointsClicked[1];
            //compute the radius
            var radius = Math.Sqrt(Math.Pow((border.X - center.X), 2) + Math.Pow((border.Y - center.Y), 2));
            var dist = (int)radius;
            //get the top left corner of the client rectangle
            var x1 = center.X - dist;
            var y1 = center.Y - dist;
            //draw the circle
            drawingPanel.CreateGraphics().DrawEllipse(drawingPen,x1,y1,dist*2, dist*2);
            //clear the shapes
            pointsClicked.Clear();
            indexer = 0;
            //push the bitmap to the stack
            var map = new Bitmap(drawingPanel.Width,drawingPanel.Height);
            using (var graphics = Graphics.FromImage(map))
            {
                var panelLocation = PointToScreen(drawingPanel.Location);
                graphics.CopyFromScreen(panelLocation, Point.Empty, drawingPanel.Size);
            }
            paintStates.Push(map);
        }

        if (indexer == 2 && drawItem == "Arc")
        {
            var fPoint = pointsClicked[0];
            var sPoint = pointsClicked[1];
            //compute the angle between the two points
            // calculate the center point of the arc
            Point centerPoint = new Point((fPoint.X + sPoint.X) / 2, (fPoint.Y + sPoint.Y) / 2);

// calculate the radius of the arc
            int radius = (int)Math.Sqrt(Math.Pow(sPoint.X - fPoint.X, 2) + Math.Pow(sPoint.Y - fPoint.Y, 2)) / 2;

// calculate the start and end angles of the arc
            float startAngle = (float)Math.Atan2(fPoint.Y - centerPoint.Y, fPoint.X - centerPoint.X) * 180 / (float)Math.PI;
            float endAngle = (float)Math.Atan2(sPoint.Y - centerPoint.Y, sPoint.X - centerPoint.X) * 180 / (float)Math.PI; 
            // draw the arc
            drawingPanel.CreateGraphics().DrawArc(drawingPen, centerPoint.X - radius, centerPoint.Y - radius, radius * 2, radius * 2, startAngle, endAngle - startAngle);
            indexer = 0;
            pointsClicked.Clear();
            //screenshot the drawing and push to the stack
            var bitmap = new Bitmap(drawingPanel.Width, drawingPanel.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var location = PointToScreen(drawingPanel.Location);
                graphics.CopyFromScreen(location, Point.Empty, drawingPanel.Size);
            }
            paintStates.Push(bitmap);
        }
        //draw a quad
        if (indexer == 2 && drawItem == "Quadrilateral")
        {
            //get the rectangle
            var fPoint = pointsClicked[0];
            var lPoint = pointsClicked[1];
            var width = lPoint.X - fPoint.X;
            var height = lPoint.Y - fPoint.Y;
            var size = new Size(width, height);
            var rect = new Rectangle(fPoint, size);
            //get the graphics context and draw the figure
            drawingPanel.CreateGraphics().DrawRectangle(drawingPen, rect);
            //clear the variables
            indexer = 0;
            pointsClicked.Clear();
            //push the action bitmap to the stack
            var bitmap = new Bitmap(drawingPanel.Width, drawingPanel.Height);
            //create a new graphics context from the bitmap
            using(var  graphics = Graphics.FromImage(bitmap)) { 
                //copy the drawing to the bitmap
                var location =  PointToScreen(drawingPanel.Location);
                graphics.CopyFromScreen(location,Point.Empty,drawingPanel.Size);
            }
            paintStates.Push(bitmap);   
        }
        //implement the drawing of a polygon
        if(indexer >4 && drawItem == "Polygon")
        {
            var points = pointsClicked.ToArray();
            drawingPanel.CreateGraphics().DrawPolygon(drawingPen, points);
            drawingPanel.CreateGraphics().DrawPolygon(drawingPen, points);
            indexer = 0;
            pointsClicked.Clear();
            Bitmap bitmap = new(drawingPanel.Width, drawingPanel.Height);
            using(var graphics = Graphics.FromImage(bitmap))
            {
                var loc = PointToScreen(drawingPanel.Location);
                graphics.CopyFromScreen(loc, Point.Empty, drawingPanel.Size);
            }
            paintStates.Push(bitmap);
        }
    }


    private void ColorBoxOnClick(object? sender, EventArgs e)
    {
      //show a new color dialog
      var colorDialog = new ColorDialog();
      if (colorDialog.ShowDialog() == DialogResult.OK)
      {
          //use the updated width
          drawingPen = new Pen(colorDialog.Color, penWidth);
            if (brush != null)
            {
                //access the hatch style 
                var style = brush.HatchStyle;
                //build a new hatch brush
                var hbrush = new HatchBrush(style, drawingPen.Color);
                drawingPen.Brush = hbrush;   
            }
            //update the color on the picture box
            var task = Task.Run(() =>
            {
				var map = new Bitmap(colorBox.Width, colorBox.Height);
				for (int x = 0; x < map.Width; x++)
				{
					for (int y = 0; y < map.Height; y++)
					{
						map.SetPixel(x, y, colorDialog.Color);
					}
				}

				colorBox.Image = map;
			});
            task.Wait();
      }
    }

    void FillColor(PictureBox box)
    {
        //define the color
        var color = Color.RoyalBlue;
        //create a new bitmap
        var map = new Bitmap(box.Width, box.Height);
        for (int x = 0; x < box.Width; x++)
        {
            for (int y = 0; y < box.Height; y++)
            {
                //set the color pixels into the bitmap
                map.SetPixel(x,y,color);
            }
        }
        box.Image = map;
    }
    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        MessageBox.Show(e.X + " " + e.Y);
    }

    private void DrawItemOnDropDownItemClicked(object? sender, ToolStripItemClickedEventArgs e)
    {
        //update the listening variabe
        
        if(e.ClickedItem != null)
        {
			listening = true;
			drawItem = e.ClickedItem.ToString();
		}
        
    }
}
//wrap the drawing of items inside a drawitem class
public  class DrawItem
{
    private string _itemShape;
    private List<Point> shape = new List<Point>();
    //require the two inside the constructor
    public DrawItem(string itemShape, List<Point> points)
    {
        _itemShape = itemShape;
        shape = points;
    }
    //define the getters and setters for the properties
    public string Item
    {
        get
        {
            return _itemShape;
        }
        
    }

    public List<Point> Points
    {
        get
        {
            return shape;
        }
    }
}