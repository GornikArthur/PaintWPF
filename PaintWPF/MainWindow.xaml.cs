using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintWPF
{
	public partial class MainWindow : Window
	{
		enum Figure
		{
			FLine,
			FRectangle,
			FEllipse,
			FBrokenLine,
			FPolygon
		}

		private List<MyFigure> arr_figures = new List<MyFigure>();
		private bool isDrawing = false;
		private Figure chose_figure = Figure.FLine;
		private MyBrokenLine broken_line;
		private MyPolygon my_polygon;

		public MainWindow()
		{
			InitializeComponent();
			this.MouseDown += MainWindow_MouseDown;
			this.MouseMove += MainWindow_MouseMove;
			this.MouseUp += MainWindow_MouseUp;
			this.KeyDown += MainWindow_KeyDown;
			LineButton.Click += LineButton_Click;
			RectangleButton.Click += RectangleButton_Click;
			EllipseButton.Click += EllipseButton_Click;
			BrokenLineButton.Click += BrokenLineButton_Click;
			PolygonButton.Click += PolygonButton_Click;
		}
		private void LineButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = Figure.FLine;
		}
		private void RectangleButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = Figure.FRectangle;
		}
		private void EllipseButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = Figure.FEllipse;
		}
		private void BrokenLineButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = Figure.FBrokenLine;
		}
		private void PolygonButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = Figure.FPolygon;
		}
		private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			switch (chose_figure)
			{
				case Figure.FLine:
					{
						MyLine figure = new MyLine(e.GetPosition(Paint_canvas));
						Paint_canvas.Children.Add(figure.GetFigure());
						arr_figures.Add(figure);
						isDrawing = true;
						break;
					}
				case Figure.FRectangle:
					{
						MyRectangle figure = new MyRectangle(e.GetPosition(Paint_canvas));
						Paint_canvas.Children.Add(figure.GetFigure());
						arr_figures.Add(figure);
						isDrawing = true;
						break;
					}
				case Figure.FEllipse:
					{
						MyEllipse figure = new MyEllipse(e.GetPosition(Paint_canvas));
						Paint_canvas.Children.Add(figure.GetFigure());
						arr_figures.Add(figure);
						isDrawing = true;
						break;
					}
				case Figure.FBrokenLine:
					{
						if (broken_line == null) broken_line = new MyBrokenLine();
						MyLine line = new MyLine(e.GetPosition(Paint_canvas));
						broken_line.AddLine(line);
						Paint_canvas.Children.Add(broken_line.GetLineByIndex(broken_line.last_line).GetFigure());
						isDrawing = true;
						break;
					}
				case Figure.FPolygon:
					{
						if (my_polygon == null)
						{
							my_polygon = new MyPolygon(e.GetPosition(Paint_canvas));
							Paint_canvas.Children.Add(my_polygon.GetLineByIndex(0).GetFigure());
						}
						MyLine line = new MyLine(e.GetPosition(Paint_canvas));
						my_polygon.AddLine(line);
						Paint_canvas.Children.Add(my_polygon.GetLineByIndex(my_polygon.last_line).GetFigure());
						isDrawing = true;
						break;
					}
				default:
					break;
			}
		}

		private void MainWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if (isDrawing)
			{
				switch (chose_figure)
				{
					case Figure.FLine:
						{
							((MyLine)arr_figures[arr_figures.Count - 1]).Calc(e.GetPosition(Paint_canvas));
							break;
						}
					case Figure.FRectangle:
						{
							((MyRectangle)arr_figures[arr_figures.Count - 1]).Calc(e.GetPosition(Paint_canvas));
							break;
						}
					case Figure.FEllipse:
						{
							((MyEllipse)arr_figures[arr_figures.Count - 1]).Calc(e.GetPosition(Paint_canvas));
							break;
						}
					case Figure.FBrokenLine:
						{
							broken_line.GetLineByIndex(broken_line.last_line).Calc(e.GetPosition(Paint_canvas));
							break;
						}
					case Figure.FPolygon:
						{
							my_polygon.GetLineByIndex(my_polygon.last_line).Calc(e.GetPosition(Paint_canvas));
							my_polygon.GetLineByIndex(0).Calc(e.GetPosition(Paint_canvas));
							break;
						}
					default:
						break;
				}
			}
		}

		private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (chose_figure == Figure.FLine || chose_figure == Figure.FRectangle || chose_figure == Figure.FEllipse) isDrawing = false;
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if ((chose_figure == Figure.FBrokenLine || chose_figure == Figure.FPolygon) && e.Key == Key.Escape)
			{
				if (chose_figure == Figure.FBrokenLine)
				{
					arr_figures.Add(broken_line);
					broken_line = null;
				}
				else
				{
					arr_figures.Add(my_polygon);
					my_polygon = null;
				}
				isDrawing = false;
			}
		}
	}
}
