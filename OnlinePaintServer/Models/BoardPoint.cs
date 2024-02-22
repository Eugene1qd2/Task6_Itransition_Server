using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePaintServer.Models
{
    public  class BoardPoint
    {
        public double X,Y;
        public BoardPoint()
        {
            X = 0;
            Y = 0;
        }
        public BoardPoint(double x,double y)
        {
            X=x;   
            Y=y;
        }
        public override string ToString()
        {
            return X+ "'" + Y ;
        }
    }
}
