using System.Text.Json.Serialization;
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
using MyFigureLibrary;

namespace PaintWPF.Figures
{
    public class MyEllipse : MyFigure
    {
        public override string Name => "FEllipse";
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string StrokeColor { get; set; }
        public int StrokeThickness { get; set; }
        public string FillColor { get; set; } = "Transparent";

        [JsonIgnore]
        private Ellipse ellip;

        public MyEllipse() { }

        public MyEllipse(Point startPoint, Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
        {
            X = startPoint.X;
            Y = startPoint.Y;
            Width = 0;
            Height = 0;
            StrokeColor = color.ToString();
            StrokeThickness = thickness;
            InitializeEllipse();

            Paint_canvas.Children.Add(GetFigure());
            arr_figures.Add(this);
        }

        private void InitializeEllipse()
        {
            ellip = new Ellipse
            {
                Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(StrokeColor),
                StrokeThickness = StrokeThickness,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor),
                Width = Width,
                Height = Height
            };
            Canvas.SetLeft(ellip, X);
            Canvas.SetTop(ellip, Y);
        }

        public override void Calc(Point newPoint)
        {
            X = Math.Min(X, newPoint.X);
            Y = Math.Min(Y, newPoint.Y);
            Width = Math.Abs(newPoint.X - X);
            Height = Math.Abs(newPoint.Y - Y);

            if (ellip != null)
            {
                Canvas.SetLeft(ellip, X);
                Canvas.SetTop(ellip, Y);
                ellip.Width = Width;
                ellip.Height = Height;
            }
        }

        public Ellipse GetFigure()
        {
            if (ellip == null) InitializeEllipse();
            return ellip;
        }

        public override bool IsPointInside(Point point)
        {
            double centerX = X + Width / 2;
            double centerY = Y + Height / 2;
            double a = Width / 2;
            double b = Height / 2;

            double distance = Math.Pow(point.X - centerX, 2) / Math.Pow(a, 2) + Math.Pow(point.Y - centerY, 2) / Math.Pow(b, 2);
            return distance < 1;
        }

        public override void SetFillColor(Color color)
        {
            FillColor = color.ToString();
            if (ellip != null)
            {
                ellip.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor);
            }
        }

        public override void RemoveFigure(Canvas canvas)
        {
            canvas.Children.Remove(ellip);
        }

        public override void AddFigure(Canvas canvas)
        {
            if (ellip == null) InitializeEllipse();
            canvas.Children.Add(ellip);
        }
        public override void MouseMove(Point pos, Canvas Paint_canvas, List<MyFigure> arr_figures)
        {
            arr_figures[arr_figures.Count - 1].Calc(pos);
        }
        public override int UndoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
        {
            RemoveFigure(canvas);
            cur_action_pos--;
            return cur_action_pos;
        }
        public override int RedoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
        {
            AddFigure(canvas);
            cur_action_pos++;
            return cur_action_pos;
        }

        public override bool AreEqualFigures(MyFigure fig1, MyFigure fig2)
        {
            if (fig1.GetType() != fig2.GetType()) return false;
            Ellipse сfig1 = ((MyEllipse)fig1).GetFigure();
            Ellipse сfig2 = ((MyEllipse)fig2).GetFigure();

            if (сfig1.Width != сfig2.Width || сfig1.Height != сfig2.Height || Canvas.GetLeft(сfig1) != Canvas.GetLeft(сfig2) || Canvas.GetTop(сfig1) != Canvas.GetTop(сfig2)) return false;
            return true;
        }

    }
}
