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
using System.Reflection;
using MyFigureLibrary;
using PaintWPF.Figures;
using System.Runtime.CompilerServices;

namespace PaintWPF
{
    public partial class MainWindow : Window
	{
		private List<MyFigure> arr_figures = new List<MyFigure>();
		private List<MyFigureLibrary.Action> arr_actions = new List<MyFigureLibrary.Action>();
		private int cur_action_pos = 0;
		private bool isDrawing = false;
		private string chose_figure = "FLine";
		private int thickness = 1;
		private Color selectedColor;
		private bool selectedFillColor;

		public delegate MyFigure FigureCreator(Point startPoint, Color color, int thickness, Canvas paint_canvas, List<MyFigure> arr);
		private Dictionary<string, FigureCreator> figureCreators = new Dictionary<string, FigureCreator>()
		{
			{ "FLine", (startPoint, color, thickness, paint_canvas, arr) => 
			new MyLine(startPoint, color, thickness, paint_canvas, arr) },

			{ "FRectangle", (startPoint, color, thickness, paint_canvas, arr) => 
			new MyRectangle(startPoint, color, thickness, paint_canvas, arr) },

			{ "FEllipse", (startPoint, color, thickness, paint_canvas, arr) =>
			new MyEllipse(startPoint, color, thickness, paint_canvas, arr) },

			{ "FBrokenLine", (startPoint, color, thickness, paint_canvas, arr) =>
			MyBrokenLine.CreatingLine(startPoint, color, thickness, paint_canvas, arr) },

			{ "FPolygon", (startPoint, color, thickness, paint_canvas, arr) =>
			MyPolygon.CreatePolygonLine(startPoint, color, thickness, paint_canvas, arr) }
		};

		public delegate bool FigureFinisher(Color color, int thickness, Canvas paint_canvas, List<MyFigure> arr, Key key);
		private Dictionary<string, FigureFinisher> figureFinishers = new Dictionary<string, FigureFinisher>()
		{
			{ "FLine", (color, thickness, canvas, arr, key) => false },
			{ "FRectangle", (color, thickness, canvas, arr, key) => false },
			{ "FEllipse", (color, thickness, canvas, arr, key) => false },
			{ "FBrokenLine", (color, thickness, canvas, arr, key) => MyBrokenLine.FinishBrokenLine(color, thickness, canvas, arr, key) },
			{ "FPolygon", (color, thickness, canvas, arr, key) => MyPolygon.FinishPolygon(color, thickness, canvas, arr, key) }
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
			chose_figure = "FLine";
		}
		private void RectangleButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = "FRectangle";
		}
		private void EllipseButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = "FEllipse";
		}
		private void BrokenLineButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = "FBrokenLine";
		}
		private void PolygonButton_Click(object sender, RoutedEventArgs e)
		{
			chose_figure = "FPolygon";
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
				if (chose_figure == "FPolygon" && MyPolygon.my_polygon != null && MyPolygon.my_polygon.GetLineByIndex(MyPolygon.my_polygon.last_line) != null)
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
			var saveList = arr_figures.Select(fig => new FigureSaveData
			{
				TypeName = fig.GetType().FullName!,
				JsonData = JsonSerializer.Serialize(fig, fig.GetType())
			}).ToList();

			var options = new JsonSerializerOptions { WriteIndented = true };
			string json = JsonSerializer.Serialize(saveList, options);
			File.WriteAllText(filePath, json);
		}

		private void LoadFigures(string filePath)
		{
			var currentAssembly = Assembly.GetExecutingAssembly();
			var figureTypes = currentAssembly
				.GetTypes()
				.Where(t => t.IsClass && t.Namespace == "PaintWPF.Figures" && typeof(MyFigure).IsAssignableFrom(t));

			foreach (var t in figureTypes)
			{
				// Принудительная инициализация типа
				RuntimeHelpers.RunClassConstructor(t.TypeHandle);
			}


			if (!File.Exists(filePath)) return;

			string json = File.ReadAllText(filePath);
			var saveList = JsonSerializer.Deserialize<List<FigureSaveData>>(json);

			arr_figures.Clear();

			if (saveList != null)
			{
				foreach (var item in saveList)
				{
					// Попробуем найти тип по всем загруженным сборкам
					Type? type = null;
					foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
					{
						type = Type.GetType(item.TypeName);
						if (type == null)
						{
							string pluginName = item.TypeName.Split('.')[0];
							string pluginPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", pluginName + ".dll");

							if (File.Exists(pluginPath))
							{
								try
								{
									var pluginAssembly = Assembly.LoadFrom(pluginPath);
									type = pluginAssembly.GetType(item.TypeName);
								}
								catch (Exception ex)
								{
									MessageBox.Show($"Ошибка загрузки плагина {pluginName}: {ex.Message}");
								}
							}
						}
					}
					if (type != null && typeof(MyFigure).IsAssignableFrom(type))
					{
						var figure = (MyFigure)JsonSerializer.Deserialize(item.JsonData, type)!;
						arr_figures.Add(figure);
					}
					else
					{
						MessageBox.Show($"Тип {item.TypeName} не найден в загруженных сборках.");
					}
				}
			}

			arr_actions.Clear();
			arr_actions.AddRange(arr_figures.Cast<MyFigureLibrary.Action>());
			cur_action_pos = arr_actions.Count;

			Paint_canvas.Children.Clear();
			foreach (var figure in arr_figures)
			{
				figure.AddFigure(Paint_canvas);
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

		private void LoadPluginFigure(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "DLL files (*.dll)|*.dll",
				Title = "Выберите плагин с фигурой"
			};

			if (openFileDialog.ShowDialog() != true)
				return;

			string pluginPath = openFileDialog.FileName;

			try
			{
				// Копируем DLL в папку Plugins
				string pluginsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
				Directory.CreateDirectory(pluginsDir);
				string dllFileName = System.IO.Path.GetFileName(pluginPath);
				string targetPath = System.IO.Path.Combine(pluginsDir, dllFileName);
				File.Copy(pluginPath, targetPath, overwrite: true);

				// Загружаем сборку
				Assembly pluginAssembly = Assembly.LoadFrom(targetPath);

				// Строим имя типа на основе имени DLL: Trapezoid.dll → Trapezoid.MyTrapezoid
				string baseName = System.IO.Path.GetFileNameWithoutExtension(dllFileName);
				string typeName = $"{baseName}.My{baseName}";

				// Пытаемся получить тип
				Type? type = pluginAssembly.GetType(typeName);
				if (type == null || !typeof(MyFigure).IsAssignableFrom(type))
				{
					MessageBox.Show($"Тип '{typeName}' не найден или не является наследником MyFigure.");
					return;
				}

				// Ищем нужный конструктор
				var constructor = type.GetConstructor(new[]
				{
			typeof(Point),
			typeof(Color),
			typeof(int),
			typeof(Canvas),
			typeof(List<MyFigure>)
		});

				if (constructor == null)
				{
					MessageBox.Show($"Конструктор для типа '{typeName}' не найден.");
					return;
				}

				string figureKey = "F" + type.Name;

				// Регистрируем создателя фигуры
				figureCreators[figureKey] = (startPoint, color, thickness, canvas, arr) =>
					(MyFigure)constructor.Invoke(new object[] { startPoint, color, thickness, canvas, arr });

				// Проверяем статический метод Finish (если есть)
				var finishMethod = type.GetMethod("Finish", BindingFlags.Public | BindingFlags.Static);
				if (finishMethod != null)
				{
					figureFinishers[figureKey] = (color, thickness, canvas, arr, key) =>
						(bool)finishMethod.Invoke(null, new object[] { color, thickness, canvas, arr, key })!;
				}
				else
				{
					figureFinishers[figureKey] = (color, thickness, canvas, arr, key) => false;
				}

				chose_figure = figureKey;
				MessageBox.Show($"Фигура '{figureKey}' успешно загружена и выбрана.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке плагина: {ex.Message}");
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

		private void PolygonButton_Click_1(object sender, RoutedEventArgs e)
		{

		}
	}
}
