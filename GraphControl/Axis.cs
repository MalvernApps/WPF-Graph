using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationSDK
{
    /// <summary>
    /// This class defines a single axis on a graphical display
    /// </summary>
    public class Axis
    {
        double _ScreenHeight;

        double _NowMinmumValue;

        public double NowMinmumValue
        {
            get { return _NowMinmumValue; }
            set { _NowMinmumValue = value; }
        }
        double _NowMaximumValue;

        public double NowMaximumValue
        {
            get { return _NowMaximumValue; }
            set { _NowMaximumValue = value; }
        }

        double ZoomOutMinmumValue;
        double ZoomOutMaximumValue;
        double Offset;
        double Scaling;
        bool FlipAxis;

        /// <summary>
        /// use to configure the class
        /// </summary>
        public Axis( double MyScreenHeight, double MyZoomOutMin, double MyZoomOutMax, double myOffset, bool Flip)
        {
            // remember variables
            _ScreenHeight = MyScreenHeight;
            _NowMinmumValue = ZoomOutMinmumValue = MyZoomOutMin;
            _NowMaximumValue = ZoomOutMaximumValue = MyZoomOutMax;
            Offset = myOffset;
            FlipAxis = Flip;

            // calculate scaling
            CalculateScaling();
        }

        public float ConvertUnitsToPixels(double unit)
        {
            // calculate scaling
            float pixels = (float)((unit - _NowMinmumValue) * Scaling);
            // flip axis upside down if needed
            if (FlipAxis == true)
                pixels = (float)_ScreenHeight - pixels;

            pixels += (float) Offset;
            return pixels;
        }

        /// <summary>
        /// return the screen span
        /// </summary>
        /// <returns></returns>
        public double GetSpan()
        {
            return (_NowMaximumValue - _NowMinmumValue);
        }

        /// <summary>
        /// Note this function has not been tested since the offset change
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public double ConvertPixelsToUnits(double pixels)
        {
            double unit;

            pixels -= (int) Offset;

            // flip axis upside down if needed
            if (FlipAxis == true) pixels = (int)_ScreenHeight - pixels;
            // calculate units
            unit = ((double)pixels / Scaling) + _NowMinmumValue;

            return unit;
        }

        public void CalculateScaling()
        {
            // calculate simple scaling
            Scaling = (_ScreenHeight / (_NowMaximumValue - _NowMinmumValue));
        }

        public void ZoomOut()
        {
            // go to max space
            _NowMinmumValue = ZoomOutMinmumValue;
            _NowMaximumValue = ZoomOutMaximumValue;

            // calculate scaling
            CalculateScaling();
        }

        public void ZoomIn(double MyZoomOutMin, double MyZoomOutMax)
        {
            // go to max space
            _NowMinmumValue = MyZoomOutMin;
            _NowMaximumValue = MyZoomOutMax;

            // calculate scaling
            CalculateScaling();
        }

        public void ReSize(int Value)
        {
            _ScreenHeight = Value;

            // calculate scaling
            CalculateScaling();
        }

        /// <summary>
        /// Zoom in or out by a factor of 2
        /// </summary>
        /// <param name="Direction">Direction in which to zoom</param>
        public void ZoomInx2(int Direction)
        {
            double Center = (_NowMaximumValue + _NowMinmumValue) / 2;
            double Span = (_NowMaximumValue - _NowMinmumValue);

            if (Direction < 0) Span /= 2.5;
            else
                Span *= 0.65;

            // error check span, make sure always min of 100 kHz
            if (Span < 0.1) Span = 0.1;

            _NowMaximumValue = Center + Span;
            _NowMinmumValue = Center - Span;

            // error check min and max
            if (_NowMaximumValue > ZoomOutMaximumValue) _NowMaximumValue = ZoomOutMaximumValue;
            if (_NowMinmumValue < ZoomOutMinmumValue) _NowMinmumValue = ZoomOutMinmumValue;

            // calculate scaling
            CalculateScaling();
        }

        /// <summary>
        /// Centers onto a specifc pixel and zooms in by factor of 8
        /// </summary>
        /// <param name="pixel"></param>
        public void CenterAndZoom(int pixel)
        {
            double Center = ConvertPixelsToUnits(pixel);
            double Span = (_NowMaximumValue - _NowMinmumValue) / 8;

            // error check span, make sure always min of 100 kHz
            if (Span < 0.1) Span = 0.1;

            _NowMaximumValue = Center + Span;
            _NowMinmumValue = Center - Span;

            // error check min and max
            if (_NowMaximumValue > ZoomOutMaximumValue) _NowMaximumValue = ZoomOutMaximumValue;
            if (_NowMinmumValue < ZoomOutMinmumValue) _NowMinmumValue = ZoomOutMinmumValue;

            // calculate scaling
            CalculateScaling();

        }

        /// <summary>
        /// Pan the display
        /// </summary>
        /// <param name="pixel">Pixel describing the location to pan by</param>
        public void Pan(int StartPixel, int StopPixel)
        {
            // stop micro pans
            if (Math.Abs(StartPixel - StopPixel) < 10) return;

            double Shift = ConvertPixelsToUnits(StartPixel) - ConvertPixelsToUnits(StopPixel);
            _NowMaximumValue += Shift;
            _NowMinmumValue += Shift;

            // error check min and max
            if (_NowMaximumValue > ZoomOutMaximumValue) _NowMaximumValue = ZoomOutMaximumValue;
            if (_NowMinmumValue < ZoomOutMinmumValue) _NowMinmumValue = ZoomOutMinmumValue;

            // calculate scaling
            CalculateScaling();
        }
    }
}

