using System.Windows.Controls;
using System.Windows.Media;
using MyFigureLibrary;

namespace PaintWPF
{
    public class ActionFill : MyFigureLibrary.Action
    {
        public MyFigure figure;
        public Color color;
        private List<MyFigureLibrary.Action> arr_actions = new List<MyFigureLibrary.Action>();
        public ActionFill(MyFigure figure, Color color)
        {
            this.figure = figure;
            this.color = color;
            SetColour(figure, color);
        }
        public static void SetColour(MyFigure figure, Color color)
        {
            figure.SetFillColor(color);
        }

        public override int UndoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
        {
            int i = cur_action_pos - 1;
            for (; i >= 0; i--)
            {
                Type f_type = figure.GetType();
                Type s_type = arr_actions[i].GetType();
                if (arr_actions[i].GetType() == typeof(ActionFill) && figure.AreEqualFigures(figure, ((ActionFill)arr_actions[i]).figure))
                {
                    figure.SetFillColor(((ActionFill)arr_actions[i]).color);
                    return cur_action_pos;
                }
            }
            figure.SetFillColor(Colors.Transparent);
            cur_action_pos--;
            return cur_action_pos;
        }
        public override int RedoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
        {
            int i = cur_action_pos;
            for (; i < arr_actions.Count; i++)
            {
                Type f_type = figure.GetType();
                Type s_type = arr_actions[i].GetType();
                if (arr_actions[i].GetType() == typeof(ActionFill) && figure.AreEqualFigures(figure, ((ActionFill)arr_actions[i]).figure))
                {
                    figure.SetFillColor(((ActionFill)arr_actions[i]).color);
                    break;
                }
            }
            cur_action_pos++;
            return cur_action_pos;
        }
    }
}
