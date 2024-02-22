using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePaintServer.Models
{
    public class LineObject
    {
        public List<BoardPoint> linePoints;
        public string Color;
        public int Width;

        public LineObject()
        {
            linePoints=new List<BoardPoint>();
            Color = "#000000";
            Width = 1;
        }

        public LineObject(List<BoardPoint> linePoints, string color, int width)
        {
            this.linePoints = linePoints;
            Color = color;
            Width = width;
        }

        public static LineObject FromString(string v)
        {
            var data=v.Split(new char[] { '>'},StringSplitOptions.RemoveEmptyEntries);
            LineObject obj = new LineObject();
            obj.Color= data[0];
            obj.Width= int.Parse(data[1]);
            for (int i = 2; i < data.Length; i++)
            {
                var dat = data[i].Split(new char[] { '\'' },StringSplitOptions.RemoveEmptyEntries);
                obj.AppendPoint(double.Parse(dat[0]), double.Parse(dat[1]));
            }
            return obj;
        }

        public void AppendPoint(BoardPoint point)
        {
            linePoints.Add(point);
        }
        public void AppendPoint(double x,double y)
        {
            linePoints.Add(new BoardPoint(x,y));
        }

        public override string ToString()
        {
            StringBuilder sb= new StringBuilder();
            sb.Append(Color + ">" + Width + ">");
            foreach(BoardPoint point in linePoints) 
            {
                sb.Append(point.ToString()+">");
            }
            return sb.ToString();
        }
    }
}
