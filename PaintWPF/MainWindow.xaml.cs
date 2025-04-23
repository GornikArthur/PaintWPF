using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Numerics;
using System.Windows.Navigation;
using System.ComponentModel;

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
		private List<Action> arr_actions = new List<Action>();
		private int cur_action_pos = 0;
		private bool isDrawing = false;
		private Figure chose_figure = Figure.FLine;
		private int thickness = 1;
		private Color selectedColor;
		private bool selectedFillColor;

		public delegate MyFigure FigureCreator(Point startPoint, Color color, int thickness, Canvas paint_canvas, List<MyFigure> arr);
		private Dictionary<Figure, FigureCreator> figureCreators = new Dictionary<Figure, FigureCreator>()
		{
			{ Figure.FLine, (startPoint, color, thickness, paint_canvas, arr) => 
			new MyLine(startPoint, color, thickness, paint_canvas, arr) },

			{ Figure.FRectangle, (startPoint, color, thickness, paint_canvas, arr) => 
			new MyRectangle(startPoint, color, thickness, paint_canvas, arr) },

			{ Figure.FEllipse, (startPoint, color, thickness, paint_canvas, arr) =>
			new MyEllipse(startPoint, color, thickness, paint_canvas, arr) },

			{ Figure.FBrokenLine, (startPoint, color, thickness, paint_canvas, arr) =>
			MyBrokenLine.CreatingLine(startPoint, color, thickness, paint_canvas, arr) },

			{ Figure.FPolygon, (startPoint, color, thickness, paint_canvas, arr) =>
			MyPolygon.CreatePolygonLine(startPoint, color, thickness, paint_canvas, arr) }
		};

		public delegate bool FigureFinisher(Color color, int thickness, Canvas paint_canvas, List<MyFigure> arr, Key key);
		private Dictionary<Figure, FigureFinisher> figureFinishers = new Dictionary<Figure, FigureFinisher>()
		{
			{ Figure.FLine, (color, thickness, canvas, arr, key) => false },
			{ Figure.FRectangle, (color, thickness, canvas, arr, key) => false },
			{ Figure.FEllipse, (color, thickness, canvas, arr, key) => false },
			{ Figure.FBrokenLine, (color, thickness, canvas, arr, key) => MyBrokenLine.FinishBrokenLine(color, thickness, canvas, arr, key) },
			{ Figure.FPolygon, (color, thickness, canvas, arr, key) => MyPolygon.FinishPolygon(color, thickness, canvas, arr, key) }
		};

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
			selectedFillColor = false;
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
			if (selectedFillColor) return;
			if (figureCreators.ContainsKey(chose_figure))
			{
				MyFigure figure = figureCreators[chose_figure](e.GetPosition(Paint_canvas), selectedColor, thickness, Paint_canvas, arr_figures);
				isDrawing = true;
			}
		}

		private void MainWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if (isDrawing)
			{
				arr_figures[arr_figures.Count - 1].MouseMove(e.GetPosition(Paint_canvas), Paint_canvas, arr_figures);
				if (chose_figure == Figure.FPolygon && MyPolygon.my_polygon != null && MyPolygon.my_polygon.GetLineByIndex(MyPolygon.my_polygon.last_line) != null)
				{
					MyPolygon.my_polygon.GetLineByIndex(0).Calc(e.GetPosition(Paint_canvas));
				}
			}
		}
		private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (arr_figures.Count > 0 && isDrawing)
			{
				isDrawing = figureFinishers[chose_figure](selectedColor, thickness, Paint_canvas, arr_figures, Key.None);
				if (!isDrawing)
				{
					for (int i = cur_action_pos; i < arr_actions.Count;)
					{
						if (arr_actions[i] is MyFigure myFigure)
						{
							arr_figures.Remove(myFigure);
						}
						arr_actions.RemoveAt(i);
					}
					cur_action_pos++;
					arr_actions.Add(arr_figures[arr_figures.Count-1]);
				}
			}
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (arr_figures.Count > 0 && isDrawing)
			{
				isDrawing = figureFinishers[chose_figure](selectedColor, thickness, Paint_canvas, arr_figures, e.Key);
				if (!isDrawing)
				{

					for (int i = cur_action_pos; i < arr_actions.Count;)
					{
						if (arr_actions[i] is MyFigure myFigure)
						{
							arr_figures.Remove(myFigure);
						}
						arr_actions.RemoveAt(i);
					}
					cur_action_pos++;
					arr_actions.Add(arr_figures[arr_figures.Count-1]);
				}
			}
		}
		private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			if (e.NewValue.HasValue) // Проверяем, что цвет не null
			{
				selectedColor = e.NewValue.Value;
			}
		}
		private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (SizeTextBox != null)  // Проверяем, что элемент инициализирован
			{
				thickness = (int)e.NewValue;
				SizeTextBox.Text = thickness.ToString();
			}
		}

		private void FillButton_Click(object sender, RoutedEventArgs e)
		{
			selectedFillColor = !selectedFillColor;
		}
		private void Paint_canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (selectedFillColor)
			{
				for (int i = arr_figures.Count - 1; i >= 0; i--)
				{
					if (arr_figures[i].IsPointInside(e.GetPosition(Paint_canvas)))
					{
						arr_actions.Add(new ActionFill(arr_figures[i], selectedColor));
						cur_action_pos++;
						for (int j = cur_action_pos - 1; j < arr_actions.Count - 1;)
						{
							if (arr_actions[j] is MyFigure myFigure)
							{
								arr_figures.Remove(myFigure);
							}
							arr_actions.RemoveAt(j);
						}
						break;
					}
				}
			}
		}
		
		private void SaveFigures(string filePath)
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			string json = JsonSerializer.Serialize(arr_figures, options);
			File.WriteAllText(filePath, json);
		}
		private void LoadFigures(string filePath)
		{
			if (!File.Exists(filePath)) return;

			string json = File.ReadAllText(filePath);
			arr_figures = JsonSerializer.Deserialize<List<MyFigure>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			arr_actions.Clear();
			arr_actions.AddRange(arr_figures.Cast<Action>());
			cur_action_pos = arr_actions.Count;

			Paint_canvas.Children.Clear(); // Очищаем холст

			foreach (var figure in arr_figures)
			{
				figure.AddFigure(Paint_canvas); // Добавляем фигуры обратно
			}
		}
		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog();
			if (dialog.ShowDialog() == true)
			{
				string file_name = dialog.FileName;
				LoadFigures(file_name);
			}
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.SaveFileDialog();
			if (dialog.ShowDialog() == true)
			{
				string file_name = dialog.FileName;
				SaveFigures(file_name);
			}
		}

		private void RedoButton_Click(object sender, RoutedEventArgs e)
		{
			if (!isDrawing && cur_action_pos < arr_actions.Count)
			{
				arr_actions[cur_action_pos].RedoAction(Paint_canvas, cur_action_pos, arr_actions);
				cur_action_pos++;
			}
		}

		private void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			if (!isDrawing && cur_action_pos > 0)
			{
				arr_actions[--cur_action_pos].UndoAction(Paint_canvas, cur_action_pos, arr_actions);
			}
		}

		private void RedoAllButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = cur_action_pos; i < arr_actions.Count; i++)
			{
				arr_actions[i].RedoAction(Paint_canvas, cur_action_pos, arr_actions);
			}
			cur_action_pos = arr_actions.Count;
		}
	}
}
