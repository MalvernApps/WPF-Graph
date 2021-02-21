using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphControl
{
    class AxisScale
    {
        double scale;
        double off;

        public AxisScale( double datasize, double screenSize, double offset)
        {
            scale = screenSize / datasize;

            off = offset;
        }

        public double GetScaledValue( double value )
        {
            double res = off + (value * scale);
            return res;
        }

      
    }
}
