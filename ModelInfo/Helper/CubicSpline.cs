using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ModelInfo
{
    public class CubicSpline
    {
        #region Fields

        // N-1 spline coefficients for N points
        private double[] a;
        private double[] b;

        // Save the original x and y for Eval
        private double[] xOrig;
        private double[] yOrig;

        #endregion

        #region Ctor

        /// <summary>
        /// Default ctor.
        /// </summary>
        public CubicSpline()
        {
        }

        /// <summary>
        /// Construct and call Fit.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        public CubicSpline(int num, double[] x, double[] y, double startSlope = double.NaN, double endSlope = double.NaN, bool debug = false)
        {
            Fit(num, x, y, startSlope, endSlope, debug);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Throws if Fit has not been called.
        /// </summary>
        private void CheckAlreadyFitted()
        {
            if (a == null) throw new Exception("Fit must be called before you can evaluate.");
        }

        private int _lastIndex = 0;

        /// <summary>
        /// Find where in xOrig the specified x falls, by simultaneous traverse.
        /// This allows xs to be less than x[0] and/or greater than x[n-1]. So allows extrapolation.
        /// This keeps state, so requires that x be sorted and xs called in ascending order, and is not multi-thread safe.
        /// </summary>
        private int GetNextXIndex(double x)
        {
            if (x < xOrig[_lastIndex])
            {
                throw new ArgumentException("The X values to evaluate must be sorted.");
            }

            while ((_lastIndex < xOrig.Length - 2) && (x > xOrig[_lastIndex + 1]))
            {
                _lastIndex++;
            }

            return _lastIndex;
        }

        /// <summary>
        /// Evaluate the specified x value using the specified spline.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="j">Which spline to use.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The y value.</returns>
        private double EvalSpline(double x, int j, bool debug = false)
        {
            double dx = xOrig[j + 1] - xOrig[j];
            double t = (x - xOrig[j]) / dx;
            double y = (1 - t) * yOrig[j] + t * yOrig[j + 1] + t * (1 - t) * (a[j] * (1 - t) + b[j] * t); // equation 9
            if (debug) Console.WriteLine("xs = {0}, j = {1}, t = {2}", x, j, t);
            return y;
        }

        #endregion

        #region Fit*

        /// <summary>
        /// Fit x,y and then eval at points xs and return the corresponding y's.
        /// This does the "natural spline" style for ends.
        /// This can extrapolate off the ends of the splines.
        /// You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each xs.</returns>
        public double[] FitAndEval(int num, double[] x, double[] y, double[] xs, double startSlope = double.NaN, double endSlope = double.NaN, bool debug = false)
        {
            Fit(num, x, y, startSlope, endSlope, debug);
            return Eval(xs, debug);
        }

        /// <summary>
        /// Compute spline coefficients for the specified x,y points.
        /// This does the "natural spline" style for ends.
        /// This can extrapolate off the ends of the splines.
        /// You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        public void Fit(int num, double[] x, double[] y, double startSlope = double.NaN, double endSlope = double.NaN, bool debug = false)
        {
            if (Double.IsInfinity(startSlope) || Double.IsInfinity(endSlope))
            {
                throw new Exception("startSlope and endSlope cannot be infinity.");
            }

            // Save x and y for eval
            this.xOrig = x;
            this.yOrig = y;

            int n = num;  // x.Length;
            double[] r = new double[n]; // the right hand side numbers: wikipedia page overloads b

            TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
            double dx1, dx2, dy1, dy2;

            // First row is different (equation 16 from the article)
            if (double.IsNaN(startSlope))
            {
                dx1 = x[1] - x[0];
                m.C[0] = 1.0f / dx1;
                m.B[0] = 2.0f * m.C[0];
                r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
            }
            else
            {
                m.B[0] = 1;
                r[0] = startSlope;
            }

            // Body rows (equation 15 from the article)
            for (int i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];

                m.A[i] = 1.0f / dx1;
                m.C[i] = 1.0f / dx2;
                m.B[i] = 2.0f * (m.A[i] + m.C[i]);

                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            // Last row also different (equation 17 from the article)
            if (double.IsNaN(endSlope))
            {
                dx1 = x[n - 1] - x[n - 2];
                dy1 = y[n - 1] - y[n - 2];
                m.A[n - 1] = 1.0f / dx1;
                m.B[n - 1] = 2.0f * m.A[n - 1];
                r[n - 1] = 3 * (dy1 / (dx1 * dx1));
            }
            else
            {
                m.B[n - 1] = 1;
                r[n - 1] = endSlope;
            }

            if (debug) Console.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
            if (debug) Console.WriteLine("r: {0}", ArrayUtil.ToString<double>(r));

            // k is the solution to the matrix
            double[] k = m.Solve(r);
            if (debug) Console.WriteLine("k = {0}", ArrayUtil.ToString<double>(k));

            // a and b are each spline's coefficients
            this.a = new double[n - 1];
            this.b = new double[n - 1];

            for (int i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                a[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
                b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
            }

            if (debug) Console.WriteLine("a: {0}", ArrayUtil.ToString<double>(a));
            if (debug) Console.WriteLine("b: {0}", ArrayUtil.ToString<double>(b));
        }

        #endregion

        #region Eval*

        /// <summary>
        /// Evaluate the spline at the specified x coordinates.
        /// This can extrapolate off the ends of the splines.
        /// You must provide X's in ascending order.
        /// The spline must already be computed before calling this, meaning you must have already called Fit() or FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public double[] Eval(double[] x, bool debug = false)
        {
            CheckAlreadyFitted();

            int n = x.Length;
            double[] y = new double[n];
            _lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            for (int i = 0; i < n; i++)
            {
                // Find which spline can be used to compute this x (by simultaneous traverse)
                int j = GetNextXIndex(x[i]);

                // Evaluate using j'th spline
                y[i] = EvalSpline(x[i], j, debug);
            }

            return y;
        }

        /// <summary>
        /// Evaluate (compute) the slope of the spline at the specified x coordinates.
        /// This can extrapolate off the ends of the splines.
        /// You must provide X's in ascending order.
        /// The spline must already be computed before calling this, meaning you must have already called Fit() or FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public double[] EvalSlope(double[] x, bool debug = false)
        {
            CheckAlreadyFitted();

            int n = x.Length;
            double[] qPrime = new double[n];
            _lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            for (int i = 0; i < n; i++)
            {
                // Find which spline can be used to compute this x (by simultaneous traverse)
                int j = GetNextXIndex(x[i]);

                // Evaluate using j'th spline
                double dx = xOrig[j + 1] - xOrig[j];
                double dy = yOrig[j + 1] - yOrig[j];
                double t = (x[i] - xOrig[j]) / dx;

                // From equation 5 we could also compute q' (qp) which is the slope at this x
                qPrime[i] = dy / dx
                    + (1 - 2 * t) * (a[j] * (1 - t) + b[j] * t) / dx
                    + t * (1 - t) * (b[j] - a[j]) / dx;

                if (debug) Console.WriteLine("[{0}]: xs = {1}, j = {2}, t = {3}", i, x[i], j, t);
            }

            return qPrime;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Static all-in-one method to fit the splines and evaluate at X coordinates.
        /// </summary>
        /// <param name="num">Input. Real Lenth of X. Values in X must in ascend Order</param>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each xs.</returns>
        public static double[] Compute(int num, double[] x, double[] y, double[] xs, double startSlope = double.NaN, double endSlope = double.NaN, bool debug = false)
        {
            CubicSpline spline = new CubicSpline();
            return spline.FitAndEval(num, x, y, xs, startSlope, endSlope, debug);
        }

        /// <summary>
        /// Fit the input x,y points using a 'geometric' strategy so that y does not have to be a single-valued
        /// function of x.
        /// </summary>
        /// <param name="x">Input x coordinates.</param>
        /// <param name="y">Input y coordinates, do not need to be a single-valued function of x.</param>
        /// <param name="nOutputPoints">How many output points to create.</param>
        /// <param name="xs">Output (interpolated) x values.</param>
        /// <param name="ys">Output (interpolated) y values.</param>
        public static void FitGeometric(double[] x, double[] y, int nOutputPoints, out double[] xs, out double[] ys)
        {
            // Compute distances
            int n = x.Length;
            double[] dists = new double[n]; // cumulative distance
            dists[0] = 0;
            double totalDist = 0;

            for (int i = 1; i < n; i++)
            {
                double dx = x[i] - x[i - 1];
                double dy = y[i] - y[i - 1];
                double dist = (double)Math.Sqrt(dx * dx + dy * dy);
                totalDist += dist;
                dists[i] = totalDist;
            }

            // Create 'times' to interpolate to
            double dt = totalDist / (nOutputPoints - 1);
            double[] times = new double[nOutputPoints];
            times[0] = 0;

            for (int i = 1; i < nOutputPoints; i++)
            {
                times[i] = times[i - 1] + dt;
            }

            // Spline fit both x and y to times
            CubicSpline xSpline = new CubicSpline();
            xs = xSpline.FitAndEval(x.Length, dists, x, times);

            CubicSpline ySpline = new CubicSpline();
            ys = ySpline.FitAndEval(x.Length, dists, y, times);
        }

        #endregion
    }

    public class TriDiagonalMatrixF
    {
        /// <summary>
        /// The values for the sub-diagonal. A[0] is never used.
        /// </summary>
        public double[] A;

        /// <summary>
        /// The values for the main diagonal.
        /// </summary>
        public double[] B;

        /// <summary>
        /// The values for the super-diagonal. C[C.Length-1] is never used.
        /// </summary>
        public double[] C;

        /// <summary>
        /// The width and height of this matrix.
        /// </summary>
        public int N
        {
            get { return (A != null ? A.Length : 0); }
        }

        /// <summary>
        /// Indexer. Setter throws an exception if you try to set any not on the super, main, or sub diagonals.
        /// </summary>
        public double this[int row, int col]
        {
            get
            {
                int di = row - col;

                if (di == 0)
                {
                    return B[row];
                }
                else if (di == -1)
                {
                    Debug.Assert(row < N - 1);
                    return C[row];
                }
                else if (di == 1)
                {
                    Debug.Assert(row > 0);
                    return A[row];
                }
                else return 0;
            }
            set
            {
                int di = row - col;

                if (di == 0)
                {
                    B[row] = value;
                }
                else if (di == -1)
                {
                    Debug.Assert(row < N - 1);
                    C[row] = value;
                }
                else if (di == 1)
                {
                    Debug.Assert(row > 0);
                    A[row] = value;
                }
                else
                {
                    throw new ArgumentException("Only the main, super, and sub diagonals can be set.");
                }
            }
        }

        /// <summary>
        /// Construct an NxN matrix.
        /// </summary>
        public TriDiagonalMatrixF(int n)
        {
            this.A = new double[n];
            this.B = new double[n];
            this.C = new double[n];
        }

        /// <summary>
        /// Produce a string representation of the contents of this matrix.
        /// </summary>
        /// <param name="fmt">Optional. For String.Format. Must include the colon. Examples are ':0.000' and ',5:0.00' </param>
        /// <param name="prefix">Optional. Per-line indentation prefix.</param>
        public string ToDisplayString(string fmt = "", string prefix = "")
        {
            if (this.N > 0)
            {
                var s = new StringBuilder();
                string formatString = "{0" + fmt + "}";

                for (int r = 0; r < N; r++)
                {
                    s.Append(prefix);

                    for (int c = 0; c < N; c++)
                    {
                        s.AppendFormat(formatString, this[r, c]);
                        if (c < N - 1) s.Append(", ");
                    }

                    s.AppendLine();
                }

                return s.ToString();
            }
            else
            {
                return prefix + "0x0 Matrix";
            }
        }

        /// <summary>
        /// Solve the system of equations this*x=d given the specified d.
        /// </summary>
        /// <remarks>
        /// Uses the Thomas algorithm described in the wikipedia article: http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
        /// Not optimized. Not destructive.
        /// </remarks>
        /// <param name="d">Right side of the equation.</param>
        public double[] Solve(double[] d)
        {
            int n = this.N;

            if (d.Length != n)
            {
                throw new ArgumentException("The input d is not the same size as this matrix.");
            }

            // cPrime
            double[] cPrime = new double[n];
            cPrime[0] = C[0] / B[0];

            for (int i = 1; i < n; i++)
            {
                cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);
            }

            // dPrime
            double[] dPrime = new double[n];
            dPrime[0] = d[0] / B[0];

            for (int i = 1; i < n; i++)
            {
                dPrime[i] = (d[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);
            }

            // Back substitution
            double[] x = new double[n];
            x[n - 1] = dPrime[n - 1];

            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = dPrime[i] - cPrime[i] * x[i + 1];
            }

            return x;
        }
    }
    public static class ArrayUtil
    {
        /// <summary>
        /// Create a string to display the array values.
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="format">Optional. A string to use to format each value. Must contain the colon, so something like ':0.000'</param>
        public static string ToString<T>(T[] array, string format = "")
        {
            var s = new StringBuilder();
            string formatString = "{0" + format + "}";

            for (int i = 0; i < array.Length; i++)
            {
                if (i < array.Length - 1)
                {
                    s.AppendFormat(formatString + ", ", array[i]);
                }
                else
                {
                    s.AppendFormat(formatString, array[i]);
                }
            }

            return s.ToString();
        }
    }
}
