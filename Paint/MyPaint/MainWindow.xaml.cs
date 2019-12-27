/************************************************
 * Author: Tucker Myers
 * Date 5/10/2018
 * File Mainwindow.xaml.cs
 * *********************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;


/************************************************
 * Overview: This program is designed to replicate
 * a paint program. This file handles the logic
 * of the interface for the drawing tools and 
 * the drawing itself.
 * 
 * Note: The logic for undoing and load/save is broken
 * due to MouseUp not being registered everytime.
 * The realtime display of rectangle and triangle is currently
 * disabled due to a bug in the mouseup functionality.
 * *********************************************/

namespace MyPaint
{ 
    /***********************************************
     * This class is a wrapper for building a shape.
     * It takes in all the parameters needed to construct
     * various shapes from the System.Window.Shapes namespace.
     * ShapeWrapper also handles the drawing of the shape on
     * the Canvas object.
     * ShapeWrapper is serializer friendly.
     * ********************************************/
    public class ShapeWrapper
    {
        //An enum to represent which shape the wrapper is handling
        public enum shape { line, rectangle, circle, triangle, pencil };

        //Pass in all the parameters for a polygon-type shape. (rectangle, circle)
        public ShapeWrapper(double width, double height, double left, double top, double thickness, Color color, Color border, bool fill, shape shape)
        {
            Width = width;
            Height = height;
            Left = left;
            Top = top;
            Color = color;
            Fill = fill;
            Thickness = thickness;
            Shape = shape;
            Border = border;
        }
        //Pass in all the parameters for the creating of a polyline (used for the pencil and eraser tool)
        public ShapeWrapper(double thickness, Color color, Color border, shape shape, PointCollection points) 
        {
            Thickness = thickness;
            Shape = shape;
            Color = color;
            Points = points;
            Border = border;
        }
        //Default ctor
        public ShapeWrapper()
        {

        }

        //The width of the shape (x1)
        public double Width { get; set; }
        //The height of the shape (y1)
        public double Height { get; set; }
        //A secondary x coordinate, usually the last x coordinate the mouse was on (x2)
        public double Left { get; set; }
        //A secondary y coordinate, usually the last y coordinate the mouse was on (y2)
        public double Top { get; set; }
        //Thickness of the border
        public double Thickness { get; set; }
        //Fill or not to fill
        public bool Fill { get; set; }
        //Color of the fill
        public Color Color { get; set; }
        //Color of the border or pencil
        public Color Border { get; set; }
        //Shape enum to tell which shape we're drawing
        public shape Shape { get; set; }
        //Collection of points for the triangle and pencil/eraser
        public PointCollection Points { get; set; }


        //Draw a shape using the passed in canvas
        public void drawShape(Canvas canvas)
        {
            switch (Shape)
            {
                //Create a single polyline using a collection of points
                case shape.pencil:
                    Polyline newShape = new Polyline();
                    newShape.Points = Points;
                    newShape.Stroke = new SolidColorBrush(Color);
                    newShape.StrokeThickness = Thickness;
                    canvas.Children.Add(newShape);
                    break;

                //Create a rectangle
                case shape.rectangle:
                    Rectangle rectangle = new Rectangle();
                    rectangle.Stroke = new SolidColorBrush(Border);

                    rectangle.Width = Math.Abs(Left - Width);
                    rectangle.Height = Math.Abs(Top - Height);

                    rectangle.StrokeThickness = Thickness;

                    /* Visual aid to help me lol
                  (-,+)     top                         (+,+)
                     ------------------------------------
                     |                |                 |
                     |                |                 |
             left    |                |  right          |
                     |                |                 |
                     |                |                 |
                     |    bottom      |                 |
                     |----------------+-----------------|
                     |                |                 |
                     |                |                 |
                     |                |                 |
                     |                |                 |                                         
                     |                |                 |
                     |                |                 |
                     ------------------------------------   
                   (-,-)                                (+,-)

                    */

                    // (+x +Y)
                    if (Left > Width && Top < Height)
                    {
                        Canvas.SetLeft(rectangle, Width);
                        Canvas.SetLeft(rectangle, Width);
                        Canvas.SetTop(rectangle, Top);
                        Canvas.SetBottom(rectangle, Height);
                        Canvas.SetRight(rectangle, Left);
                    }
                    //(-X +Y)
                    else if (Left < Width && Top < Height)
                    {
                        Canvas.SetLeft(rectangle, Left);
                        Canvas.SetTop(rectangle, Top);
                        Canvas.SetBottom(rectangle, Height);
                        Canvas.SetRight(rectangle, Width);
                    }
                    //(-X -Y)
                    else if (Left < Width && Top > Height)
                    {
                        Canvas.SetLeft(rectangle, Left);
                        Canvas.SetTop(rectangle, Height);
                        Canvas.SetBottom(rectangle, Top);
                        Canvas.SetRight(rectangle, Width);
                    }
                    //(+X -Y)
                    else
                    {
                        Canvas.SetLeft(rectangle, Width);
                        Canvas.SetTop(rectangle, Height);
                        Canvas.SetBottom(rectangle, Top);
                        Canvas.SetRight(rectangle, Left);
                    }

                    if (Fill == true)
                    {
                        rectangle.Fill = new SolidColorBrush(Color);
                    }

                    canvas.Children.Add(rectangle);
                    break;

                    //Create a circle using the same logic as the rectangle
                case shape.circle:
                    Ellipse ellipse = new Ellipse();
                    ellipse.Stroke = new SolidColorBrush(Border);
                    ellipse.Width = Math.Abs(Left - Width);
                    ellipse.Height = Math.Abs(Top - Height);

                    ellipse.StrokeThickness = Thickness;

                    // (+x +Y)
                    if (Left > Width && Top < Height)
                    {
                        Canvas.SetLeft(ellipse, Width);
                        Canvas.SetTop(ellipse, Top);
                        Canvas.SetBottom(ellipse, Height);
                        Canvas.SetRight(ellipse, Left);
                    }
                    //(-X +Y)
                    else if (Left < Width && Top < Height)
                    {

                        Canvas.SetLeft(ellipse, Left);
                        Canvas.SetTop(ellipse, Top);
                        Canvas.SetBottom(ellipse, Height);
                        Canvas.SetRight(ellipse, Width);
                    }
                    //(-X -Y)
                    else if (Left < Width && Top > Height)
                    {
                        Canvas.SetLeft(ellipse, Left);
                        Canvas.SetTop(ellipse, Height);
                        Canvas.SetBottom(ellipse, Top);
                        Canvas.SetRight(ellipse, Width);
                    }
                    //(+X -Y)
                    else
                    {
                        Canvas.SetLeft(ellipse, Width);
                        Canvas.SetTop(ellipse, Height);
                        Canvas.SetBottom(ellipse, Top);
                        Canvas.SetRight(ellipse, Left);
                    }


                    if (Fill == true)
                    {
                        ellipse.Fill = new SolidColorBrush(Color);
                    }

                    canvas.Children.Add(ellipse);
                    break;

                    //Create a right-triangle
                case shape.triangle:
                    Point p1 = new Point(Width, Height);
                    Point p2 = new Point(Left, Top);
                    Point p3 = new Point(Width, Top);

                    Polygon triangle = new Polygon();
                    PointCollection points = new PointCollection(3);
                    points.Add(p1);
                    points.Add(p2);
                    points.Add(p3);
                    triangle.Points = points;
                    triangle.Stroke = new SolidColorBrush(Border);
                    triangle.StrokeThickness = Thickness;
                    if (Fill == true)
                    {
                        triangle.Fill = new SolidColorBrush(Color);
                    }

                    canvas.Children.Add(triangle);
                    break;
                    
                    //Create a line from point a to b
                case shape.line:
                    Line line = new Line();

                    line.Stroke = line.Stroke = new SolidColorBrush(Border);
                    ;
                    line.StrokeThickness = Thickness;

                    line.X1 = Width;
                    line.Y1 = Height;
                    line.X2 = Left;
                    line.Y2 = Top;
                    canvas.Children.Add(line);
                    break;
            }
        }
    }

    /********************************************
     * This class handles the interface logic such
     * as buttons and the mouse events.
     * *****************************************/
    public partial class MainWindow : Window
    {
        //Enum to tell which tool is currently selected
        enum Tools { pencil, line, shape, fill, dropper, eraser };


        //Enum to tell which tool is selected, default is pencil
        Tools m_tool = Tools.pencil;
        //Enum to tell which shape is selected, default is rectangle
        ShapeWrapper.shape m_shape = ShapeWrapper.shape.rectangle;
        //Color of the shapes fill
        Color m_color = new Color();
        //Color of the shapes border or the pencil
        Color m_border = new Color();
        //Is the program currently drawing, default is false
        bool m_startDraw = false;
        //Last position clicked on the canvas
        Point m_position;
        //Size of the border thickness, default is five
        double m_size = 5;
        //Are we filling the shape, default is false
        bool m_fill = false;
        //Collection of points for the free draw and triangle
        PointCollection m_points = new PointCollection();
        //List of shapes to use for saving and undoing
        List<ShapeWrapper> m_shapes = new List<ShapeWrapper>();


        //Initialise the color and border to black and default select pencil
        public MainWindow()
        {
            InitializeComponent();
            m_position = new Point();
            m_color = Brushes.Black.Color;
            m_border = Brushes.Black.Color;
            pencilButton.BorderBrush = Brushes.Blue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        //Toggle a buttons background to display which button is selected
        private void toggleButtonBackground()
        {
            {
                switch (m_tool)
                {
                    case Tools.pencil:
                        pencilButton.BorderBrush = Brushes.Black;
                        break;
                    case Tools.eraser:
                        eraserButton.BorderBrush = Brushes.Black;
                        break;
                    case Tools.line:
                        lineButton.BorderBrush = Brushes.Black;
                        break;

                    case Tools.shape:
                        if (rectangleButton.BorderBrush != Brushes.Black)
                        {
                            rectangleButton.BorderBrush = Brushes.Black;
                        }
                        if (ellipseButton.BorderBrush != Brushes.Black)
                        {
                            ellipseButton.BorderBrush = Brushes.Black;
                        }
                        if (triangleButton.BorderBrush != Brushes.Black)
                        {
                            triangleButton.BorderBrush = Brushes.Black;
                        }
                        break;
                }
            }
        }

      
        //Set tool to pencil
        private void setPencil(object sender, RoutedEventArgs e)
        {
            toggleButtonBackground();
            m_tool = Tools.pencil;
            pencilButton.BorderBrush = Brushes.Blue;
        }


        //Set tool to eraser
        private void setEraser(object sender, RoutedEventArgs e)
        {
            toggleButtonBackground();
            m_tool = Tools.eraser;
            eraserButton.BorderBrush = Brushes.Blue;
        }

        //Set tool to line
        private void setLine(object sender, RoutedEventArgs e)
        {
            toggleButtonBackground();
            m_tool = Tools.line;
            lineButton.BorderBrush = Brushes.Blue;
        }

        //Set shape to rectangle
        private void setRectangle(object sender, RoutedEventArgs e)
        {
            toggleButtonBackground();
            m_tool = Tools.shape;
            m_shape = ShapeWrapper.shape.rectangle;
            rectangleButton.BorderBrush = Brushes.Blue;
        }

        //Set shape to ellipse
        private void setEllipse(object sender, RoutedEventArgs e)
        {
            toggleButtonBackground();
            m_tool = Tools.shape;
            m_shape = ShapeWrapper.shape.circle;
            ellipseButton.BorderBrush = Brushes.Blue;
        }

        //Set shape to triangle
        private void setTriangle(object sender, RoutedEventArgs e)
        {
            toggleButtonBackground();
            m_tool = Tools.shape;
            m_shape = ShapeWrapper.shape.triangle;
            triangleButton.BorderBrush = Brushes.Blue;
        }

        //Toggle fill
        private void setFill(object sender, RoutedEventArgs e)
        {
            switch (m_fill)
            {
                case true:
                    m_fill = false;
                    fillButton.BorderBrush = Brushes.Black;
                    break;
                case false:
                    m_fill = true;
                    fillButton.BorderBrush = Brushes.Blue;
                    break;
            }
        }

        //Set the size of the border using the slider
        private void setSize(object sender, RoutedEventArgs e)
        {
            m_size = sizeSlider.Value;
        }

        //Deal with the mouse being moved 
        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //If the left button was pressed down
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (m_startDraw == true)
                {

                    if (m_shapes.Count != 0 && m_tool != Tools.eraser)
                    {
                        m_shapes.RemoveAt(m_shapes.Count - 1);
                    }

                    switch (m_tool)
                    {
                        case Tools.line:
                            ShapeWrapper newShapeLine = new ShapeWrapper(m_position.X, m_position.Y, e.GetPosition(myCanvas).X, e.GetPosition(myCanvas).Y, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.line);
                            myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                            newShapeLine.drawShape(myCanvas);
                            m_shapes.Add(newShapeLine);
                            break;
                        case Tools.pencil:
                            
                            m_points.Add(new Point(m_position.X, m_position.Y));
                            ShapeWrapper newPencil = new ShapeWrapper(m_size, m_color, m_border, ShapeWrapper.shape.pencil, m_points);
                            myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                            newPencil.drawShape(myCanvas);
                            m_shapes.Add(newPencil);
                            m_position = e.GetPosition(myCanvas);
                            break;
                        case Tools.eraser:
                            
                            m_points.Add(new Point(m_position.X, m_position.Y));
                            ShapeWrapper newErasor = new ShapeWrapper(m_size, Brushes.White.Color, m_border, ShapeWrapper.shape.pencil, m_points);
                            myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                            newErasor.drawShape(myCanvas);
                            m_shapes.Add(newErasor);
                            m_position = e.GetPosition(myCanvas);
                            break;
                        case Tools.shape:
                            double x2 = e.GetPosition(myCanvas).X;
                            double y2 = e.GetPosition(myCanvas).Y;
                            switch (m_shape)
                            {
                                case ShapeWrapper.shape.circle:

                                    ShapeWrapper newShapeEllipse = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.circle);
                                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                                    newShapeEllipse.drawShape(myCanvas);
                                    m_shapes.Add(newShapeEllipse);
                                    break;

                                case ShapeWrapper.shape.rectangle:
                                    ShapeWrapper newShapeRect = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.rectangle);
                                   // myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                                   // newShapeRect.drawShape(myCanvas);
                                    m_shapes.Add(newShapeRect);
                                    break;

                                case ShapeWrapper.shape.triangle:
                                    ShapeWrapper newShapeTri = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.triangle);
                                   // myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                                   // newShapeTri.drawShape(myCanvas);
                                    m_shapes.Add(newShapeTri);
                                    break;
                            }
                           break;
                    }
                }
                Console.WriteLine("m_shapes.Count: " + m_shapes.Count.ToString());
                Console.WriteLine("myCanvase.Count: " + myCanvas.Children.Count.ToString());
            }
            else
            {
                m_startDraw = false;
            }
        }

        //Deal with the mouse button being clicked down
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Check if any of the buttons were pressed down
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                m_startDraw = true;
                    switch (m_tool)
                    {

                        case Tools.pencil:
                        case Tools.eraser:
                            ShapeWrapper newPencil;
                            m_points = new PointCollection();
                            m_position = e.GetPosition(myCanvas);
                            m_points.Add(new Point(m_position.X, m_position.Y));
                            if (m_tool == Tools.eraser)
                            {
                                newPencil = new ShapeWrapper(m_size, Brushes.White.Color, Brushes.White.Color, ShapeWrapper.shape.pencil, m_points);
                            }
                            else
                            {
                                newPencil = new ShapeWrapper(m_size, m_color, m_border, ShapeWrapper.shape.pencil, m_points);
                            }
                            newPencil.drawShape(myCanvas);
                            break;

                        case Tools.line:
                            ShapeWrapper newShapeLine = new ShapeWrapper(m_position.X, m_position.Y, e.GetPosition(myCanvas).X, e.GetPosition(myCanvas).Y, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.line);
                            newShapeLine.drawShape(myCanvas);
                            m_shapes.Add(newShapeLine);
                            m_position = e.GetPosition(myCanvas);
                            break;

                        case Tools.shape:
                            m_position = e.GetPosition(myCanvas);
                            double x2 = e.GetPosition(myCanvas).X;
                            double y2 = e.GetPosition(myCanvas).Y;
                            switch (m_shape)
                            {
                                case ShapeWrapper.shape.circle:
                                    ShapeWrapper newShapeEllipse = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.circle);
                                    newShapeEllipse.drawShape(myCanvas);
                                    m_position = e.GetPosition(myCanvas);
                                    break;

                                case ShapeWrapper.shape.rectangle:
                                    ShapeWrapper newShapeRect = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.rectangle);
                                    newShapeRect.drawShape(myCanvas);
                                    m_position = e.GetPosition(myCanvas);
                                    break;

                                case ShapeWrapper.shape.triangle:
                                    ShapeWrapper newShapeTri = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.triangle);
                                    newShapeTri.drawShape(myCanvas);
                                    break;
                            }
                            break;
                    }
                }
            else
            {
                m_startDraw = false;
            }
         }
        

        //Deal with the mouse button being released
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && myCanvas.IsMouseOver)
            {
                m_startDraw = false;
                switch (m_tool)
                {
                    case Tools.pencil:
                        ShapeWrapper newPencil = new ShapeWrapper(m_size, m_color, m_border, ShapeWrapper.shape.pencil, m_points);
                        m_points = new PointCollection();
                        newPencil.drawShape(myCanvas);
                        m_shapes.Add(newPencil);
                        break;
                    case Tools.eraser:
                        ShapeWrapper newErasor = new ShapeWrapper(m_size * 2, Brushes.White.Color, Brushes.White.Color, ShapeWrapper.shape.pencil, m_points);
                        m_points = new PointCollection();
                        newErasor.drawShape(myCanvas);
                        m_shapes.Add(newErasor);
                        break;

                    case Tools.line:
                        ShapeWrapper newShapeLine = new ShapeWrapper(m_position.X, m_position.Y, e.GetPosition(myCanvas).X, e.GetPosition(myCanvas).Y, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.line);
                        newShapeLine.drawShape(myCanvas);
                        m_shapes.Add(newShapeLine);
                        break;

                    case Tools.shape:
                        double x2 = e.GetPosition(myCanvas).X;
                        double y2 = e.GetPosition(myCanvas).Y;
                        switch (m_shape)
                        {
                            case ShapeWrapper.shape.circle:
                                ShapeWrapper newShapeEllipse = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.circle);
                                newShapeEllipse.drawShape(myCanvas);
                                m_shapes.Add(newShapeEllipse);
                                break;

                            case ShapeWrapper.shape.rectangle:
                                ShapeWrapper newShapeRect = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.rectangle);
                                newShapeRect.drawShape(myCanvas);
                                m_shapes.Add(newShapeRect);
                                break;

                            case ShapeWrapper.shape.triangle:
                                ShapeWrapper newShapeTri = new ShapeWrapper(m_position.X, m_position.Y, x2, y2, m_size, m_color, m_border, m_fill, ShapeWrapper.shape.triangle);
                                newShapeTri.drawShape(myCanvas);
                                m_shapes.Add(newShapeTri);
                                break;
                        }
                        break;
                }
                m_position = new Point();
                m_position = e.GetPosition(myCanvas);
                Console.WriteLine("released");
                Console.WriteLine("m_shapes.Count: " + m_shapes.Count.ToString());
                Console.WriteLine("myCanvase.Count: " + myCanvas.Children.Count.ToString());
                if (myCanvas.Children.Count != 1)
                {
                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 2);
                }
            }
        }

        //Save the file
        private void saveClick(object sender, RoutedEventArgs e)
        {
            string filename = "";
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.InitialDirectory = "";

            if (saveFileDialog.ShowDialog() == true)
            {
                filename = saveFileDialog.FileName;
            }

            var serializer = new XmlSerializer(m_shapes.GetType());
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, m_shapes);
            writer.Close();
        }

        //Load a file
        private void loadClick(object sender, RoutedEventArgs e)
        {
            string filename = "";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = "";
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
            }

            var serializer = new XmlSerializer(m_shapes.GetType());
            FileStream fs = new FileStream(filename, FileMode.Open);
            m_shapes.Clear();
            m_shapes = (List<ShapeWrapper>)serializer.Deserialize(fs);
            fs.Close();

            myCanvas.Children.Clear();

            foreach (ShapeWrapper shape in m_shapes)
            {
                shape.drawShape(myCanvas);
            }

        }

        //Display the about page
        private void aboutClick(object sender, RoutedEventArgs e)
        {
            AboutPage about = new AboutPage();
            about.ShowDialog();
        }

        //Show an exit dialogue
        private void exitClick(object sender, RoutedEventArgs e)
        {
            ExitPage exit = new ExitPage(this);
            exit.ShowDialog();
        }

        //Undo the last drawing
        private void undoClick(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();

            if (m_shapes.Count != 0)
            {
                m_shapes.RemoveAt(m_shapes.Count - 1);
                foreach (ShapeWrapper shape in m_shapes)
                {
                    shape.drawShape(myCanvas);
                }
            }
        }

        //Set color to white
        private void white_click(object sender, MouseButtonEventArgs e)
        {

            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.White.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.White.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Set color to black
        private void black_Click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Black.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Black.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Set the color to blue
        private void blue_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Blue.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Blue.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }
        
        //Set the color to red
        private void red_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Red.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Red.Color;
                borderColor.Background = new SolidColorBrush(m_border);

            }

        }

        //Set the color to green
        private void green_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Green.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Green.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Set the color to yellow
        private void yellow_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Yellow.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Yellow.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Set the color to brown
        private void brown_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Brown.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Brown.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }
        }

        //Set the color to pink
        private void pink_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Pink.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Pink.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Set the color to orange
        private void orange_click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Orange.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Orange.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Set the color to purple
        private void purple_Click(object sender, MouseButtonEventArgs e)
        {
            m_startDraw = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_color = Brushes.Purple.Color;
                currentColor.Background = new SolidColorBrush(m_color);
            }
            else
            {
                m_border = Brushes.Purple.Color;
                borderColor.Background = new SolidColorBrush(m_border);
            }

        }

        //Clear the whole canvas
        private void clearClick(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();
            
            m_shapes.Clear();
            m_shapes = new List<ShapeWrapper>();
            m_points.Clear();
            m_points = new PointCollection();
            
        }


    }
}
