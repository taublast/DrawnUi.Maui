//#define DEBUG_RELEASE
using Android.Graphics;
using Android.Renderscripts;
using AppoMobi.Maui.Native.Droid.Graphics;
using Java.Lang;
using Java.Util;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;
using Math = System.Math;
using Object = Java.Lang.Object;
using StringBuilder = System.Text.StringBuilder;

namespace DrawnUi.Camera;

public class TriDiagonalMatrixF
{
    /// <summary>
    /// The values for the sub-diagonal. A[0] is never used.
    /// </summary>
    public float[] A;

    /// <summary>
    /// The values for the main diagonal.
    /// </summary>
    public float[] B;

    /// <summary>
    /// The values for the super-diagonal. C[C.Length-1] is never used.
    /// </summary>
    public float[] C;

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
    public float this[int row, int col]
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
        this.A = new float[n];
        this.B = new float[n];
        this.C = new float[n];
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
    public float[] Solve(float[] d)
    {
        int n = this.N;

        if (d.Length != n)
        {
            throw new ArgumentException("The input d is not the same size as this matrix.");
        }

        // cPrime
        float[] cPrime = new float[n];
        cPrime[0] = C[0] / B[0];

        for (int i = 1; i < n; i++)
        {
            cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);
        }

        // dPrime
        float[] dPrime = new float[n];
        dPrime[0] = d[0] / B[0];

        for (int i = 1; i < n; i++)
        {
            dPrime[i] = (d[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);
        }

        // Back substitution
        float[] x = new float[n];
        x[n - 1] = dPrime[n - 1];

        for (int i = n - 2; i >= 0; i--)
        {
            x[i] = dPrime[i] - cPrime[i] * x[i + 1];
        }

        return x;
    }
}

public class CubicSpline
{

    public float[] As
    {
        get
        {
            return a;
        }
        set
        {
            a = value;
        }
    }

    public float[] Bs
    {
        get
        {
            return b;
        }
        set
        {
            b = value;
        }
    }

    public float[] Xs
    {
        get
        {
            return _xs;
        }
        set
        {
            _xs = value;
        }
    }

    public float[] Ys
    {
        get
        {
            return _ys;
        }
        set
        {
            _ys = value;
        }
    }

    public void Fit(float startSlope = float.NaN, float endSlope = float.NaN)
    {
        if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
        {
            throw new Exception("startSlope and endSlope cannot be infinity.");
        }

        // Save x and y for eval
        var xs = this._xs;
        var ys = this._ys;

        int n = xs.Length;
        float[] r = new float[n]; // the right hand side numbers: wikipedia page overloads b

        TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
        float dx1, dx2, dy1, dy2;

        // First row is different (equation 16 from the article)
        if (float.IsNaN(startSlope))
        {
            dx1 = xs[1] - xs[0];
            m.C[0] = 1.0f / dx1;
            m.B[0] = 2.0f * m.C[0];
            r[0] = 3 * (ys[1] - ys[0]) / (dx1 * dx1);
        }
        else
        {
            m.B[0] = 1;
            r[0] = startSlope;
        }

        // Body rows (equation 15 from the article)
        for (int i = 1; i < n - 1; i++)
        {
            dx1 = xs[i] - xs[i - 1];
            dx2 = xs[i + 1] - xs[i];

            m.A[i] = 1.0f / dx1;
            m.C[i] = 1.0f / dx2;
            m.B[i] = 2.0f * (m.A[i] + m.C[i]);

            dy1 = ys[i] - ys[i - 1];
            dy2 = ys[i + 1] - ys[i];
            r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
        }

        // Last row also different (equation 17 from the article)
        if (float.IsNaN(endSlope))
        {
            dx1 = xs[n - 1] - xs[n - 2];
            dy1 = ys[n - 1] - ys[n - 2];
            m.A[n - 1] = 1.0f / dx1;
            m.B[n - 1] = 2.0f * m.A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));
        }
        else
        {
            m.B[n - 1] = 1;
            r[n - 1] = endSlope;
        }

        //Debug.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
        //Debug.WriteLine("r: {0}", ArrayUtil.ToString<float>(r));

        // k is the solution to the matrix
        float[] k = m.Solve(r);
        //Debug.WriteLine("k = {0}", ArrayUtil.ToString<float>(k));

        // a and b are each spline's coefficients
        this.a = new float[n - 1];
        this.b = new float[n - 1];

        for (int i = 1; i < n; i++)
        {
            dx1 = xs[i] - xs[i - 1];
            dy1 = ys[i] - ys[i - 1];
            a[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
            b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
        }


        //Debug.WriteLine("a: {0}", ArrayUtil.ToString<float>(a));
        //Debug.WriteLine("b: {0}", ArrayUtil.ToString<float>(b));

    }


    #region Fields

    // N-1 spline coefficients for N points
    private float[] a;
    private float[] b;

    // Save the original x and y for Eval
    private float[] _xs;
    private float[] _ys;

    #endregion

    #region Ctor

    /// <summary>
    /// Default ctor.
    /// </summary>
    public CubicSpline()
    {

    }

    public CubicSpline(float[] xs, float[] ys)
    {
        _xs = xs;
        _ys = ys;
        Fit();
    }

    /// <summary>
    /// (input, output) 0-1.0
    /// </summary>
    /// <param name="knots"></param>
    //public CubicSpline(params (double, double)[] knots)
    //{
    //    var inputs = new List<float>();
    //    var outputs = new List<float>();
    //    foreach (var knot in knots)
    //    {
    //        inputs.Add((float)knot.Item1);
    //        outputs.Add((float)knot.Item2);
    //    }
    //    _xs = inputs.ToArray();
    //    _ys = outputs.ToArray();
    //    Fit();
    //}

    /// <summary>
    /// (input, output) 0-255
    /// </summary>
    /// <param name="knots"></param>
    public CubicSpline(params (int, int)[] knots)
    {
        var inputs = new List<float>();
        var outputs = new List<float>();
        foreach (var knot in knots)
        {
            inputs.Add(knot.Item1 / 255f);
            outputs.Add(knot.Item2 / 255f);
        }
        _xs = inputs.ToArray();
        _ys = outputs.ToArray();
        Fit();
    }

    /// <summary>
    /// Construct and call Fit.
    /// </summary>
    /// <param name="x">Input. X coordinates to fit.</param>
    /// <param name="y">Input. Y coordinates to fit.</param>
    /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
    /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
    /// <param name="debug">Turn on console output. Default is false.</param>
    public CubicSpline(float[] x, float[] y, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
    {
        Fit(x, y, startSlope, endSlope, debug);
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
    private int GetNextXIndex(float x)
    {
        if (x < _xs[_lastIndex])
        {
            throw new ArgumentException("The X values to evaluate must be sorted.");
        }

        while ((_lastIndex < _xs.Length - 2) && (x > _xs[_lastIndex + 1]))
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
    private float EvalSpline(float x, int j, bool debug = false)
    {
        float dx = _xs[j + 1] - _xs[j];

        float t = (x - _xs[j]) / dx;

        float y = (1 - t) * _ys[j] + t * _ys[j + 1] + t * (1 - t) * (a[j] * (1 - t) + b[j] * t); // equation 9

        if (debug)
            Super.Log(string.Format("xs = {0}, j = {1}, t = {2}", x, j, t));

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
    public float[] FitAndEval(float[] x, float[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
    {
        Fit(x, y, startSlope, endSlope, debug);

        return Eval(xs, debug);
    }

    /// <summary>
    /// Compute spline coefficients for the specified x,y points.
    /// This does the "natural spline" style for ends.
    /// This can extrapolate off the ends of the splines.
    /// You must provide points in X sort order.
    /// </summary>
    /// <param name="xs">Input. X coordinates to fit.</param>
    /// <param name="ys">Input. Y coordinates to fit.</param>
    /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
    /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
    /// <param name="debug">Turn on console output. Default is false.</param>
    public void Fit(float[] xs, float[] ys, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
    {
        if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
        {
            throw new Exception("startSlope and endSlope cannot be infinity.");
        }

#if DEBUG
        debug = true;
#endif

        // Save x and y for eval
        this._xs = xs;
        this._ys = ys;

        int n = xs.Length;
        float[] r = new float[n]; // the right hand side numbers: wikipedia page overloads b

        TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
        float dx1, dx2, dy1, dy2;

        // First row is different (equation 16 from the article)
        if (float.IsNaN(startSlope))
        {
            dx1 = xs[1] - xs[0];
            m.C[0] = 1.0f / dx1;
            m.B[0] = 2.0f * m.C[0];
            r[0] = 3 * (ys[1] - ys[0]) / (dx1 * dx1);
        }
        else
        {
            m.B[0] = 1;
            r[0] = startSlope;
        }

        // Body rows (equation 15 from the article)
        for (int i = 1; i < n - 1; i++)
        {
            dx1 = xs[i] - xs[i - 1];
            dx2 = xs[i + 1] - xs[i];

            m.A[i] = 1.0f / dx1;
            m.C[i] = 1.0f / dx2;
            m.B[i] = 2.0f * (m.A[i] + m.C[i]);

            dy1 = ys[i] - ys[i - 1];
            dy2 = ys[i + 1] - ys[i];
            r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
        }

        // Last row also different (equation 17 from the article)
        if (float.IsNaN(endSlope))
        {
            dx1 = xs[n - 1] - xs[n - 2];
            dy1 = ys[n - 1] - ys[n - 2];
            m.A[n - 1] = 1.0f / dx1;
            m.B[n - 1] = 2.0f * m.A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));
        }
        else
        {
            m.B[n - 1] = 1;
            r[n - 1] = endSlope;
        }

        // k is the solution to the matrix
        float[] k = m.Solve(r);

        // a and b are each spline's coefficients
        this.a = new float[n - 1];
        this.b = new float[n - 1];

        for (int i = 1; i < n; i++)
        {
            dx1 = xs[i] - xs[i - 1];
            dy1 = ys[i] - ys[i - 1];
            a[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
            b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
        }

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
    public float[] Eval(float[] x, bool debug = false)
    {
        CheckAlreadyFitted();

        int n = x.Length;
        float[] y = new float[n];
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

    public float Eval(float x)
    {

        // Find which spline can be used to compute this x (by simultaneous traverse)
        int j = 0;
        while ((j < _xs.Length - 2) && (x > _xs[j + 1]))
        {
            j++;
        }

        // Evaluate using j'th spline
        float dx = _xs[j + 1] - _xs[j];

        float t = (x - _xs[j]) / dx;

        float y = (1 - t) * _ys[j] + t * _ys[j + 1] + t * (1 - t) * (a[j] * (1 - t) + b[j] * t); // equation 9

#if DEBUG
        //Trace.WriteLine($"SINGLE X: {x} ({x * 255:0}), near Knot {j}, output: {y} ({y*255:0})");  
#endif

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
    public float[] EvalSlope(float[] x, bool debug = false)
    {
        CheckAlreadyFitted();

        int n = x.Length;
        float[] qPrime = new float[n];
        _lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

        for (int i = 0; i < n; i++)
        {
            // Find which spline can be used to compute this x (by simultaneous traverse)
            int j = GetNextXIndex(x[i]);

            // Evaluate using j'th spline
            float dx = _xs[j + 1] - _xs[j];
            float dy = _ys[j + 1] - _ys[j];
            float t = (x[i] - _xs[j]) / dx;

            // From equation 5 we could also compute q' (qp) which is the slope at this x
            qPrime[i] = dy / dx
                + (1 - 2 * t) * (a[j] * (1 - t) + b[j] * t) / dx
                + t * (1 - t) * (b[j] - a[j]) / dx;

            if (debug) Super.Log(string.Format("[{0}]: xs = {1}, j = {2}, t = {3}", i, x[i], j, t));
        }

        return qPrime;
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Static all-in-one method to fit the splines and evaluate at X coordinates.
    /// </summary>
    /// <param name="x">Input. X coordinates to fit.</param>
    /// <param name="y">Input. Y coordinates to fit.</param>
    /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
    /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
    /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
    /// <param name="debug">Turn on console output. Default is false.</param>
    /// <returns>The computed y values for each xs.</returns>
    public static float[] Compute(float[] x, float[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
    {
        CubicSpline spline = new CubicSpline();
        return spline.FitAndEval(x, y, xs, startSlope, endSlope, debug);
    }

    /// <summary>
    /// Fit the input x,y points using the parametric approach, so that y does not have to be an explicit
    /// function of x, meaning there does not need to be a single value of y for each x.
    /// </summary>
    /// <param name="x">Input x coordinates.</param>
    /// <param name="y">Input y coordinates.</param>
    /// <param name="nOutputPoints">How many output points to create.</param>
    /// <param name="xs">Output (interpolated) x values.</param>
    /// <param name="ys">Output (interpolated) y values.</param>
    /// <param name="firstDx">Optionally specifies the first point's slope in combination with firstDy. Together they
    /// are a vector describing the direction of the parametric spline of the starting point. The vector does
    /// not need to be normalized. If either is NaN then neither is used.</param>
    /// <param name="firstDy">See description of dx0.</param>
    /// <param name="lastDx">Optionally specifies the last point's slope in combination with lastDy. Together they
    /// are a vector describing the direction of the parametric spline of the last point. The vector does
    /// not need to be normalized. If either is NaN then neither is used.</param>
    /// <param name="lastDy">See description of dxN.</param>
    public static void FitParametric(float[] x, float[] y, int nOutputPoints, out float[] xs, out float[] ys,
        float firstDx = Single.NaN, float firstDy = Single.NaN, float lastDx = Single.NaN, float lastDy = Single.NaN)
    {
        // Compute distances
        int n = x.Length;
        float[] dists = new float[n]; // cumulative distance
        dists[0] = 0;
        float totalDist = 0;

        for (int i = 1; i < n; i++)
        {
            float dx = x[i] - x[i - 1];
            float dy = y[i] - y[i - 1];
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            totalDist += dist;
            dists[i] = totalDist;
        }

        // Create 'times' to interpolate to
        float dt = totalDist / (nOutputPoints - 1);
        float[] times = new float[nOutputPoints];
        times[0] = 0;

        for (int i = 1; i < nOutputPoints; i++)
        {
            times[i] = times[i - 1] + dt;
        }

        // Normalize the slopes, if specified
        NormalizeVector(ref firstDx, ref firstDy);
        NormalizeVector(ref lastDx, ref lastDy);

        // Spline fit both x and y to times
        CubicSpline xSpline = new CubicSpline();
        xs = xSpline.FitAndEval(dists, x, times, firstDx / dt, lastDx / dt);

        CubicSpline ySpline = new CubicSpline();
        ys = ySpline.FitAndEval(dists, y, times, firstDy / dt, lastDy / dt);
    }

    private static void NormalizeVector(ref float dx, ref float dy)
    {
        if (!Single.IsNaN(dx) && !Single.IsNaN(dy))
        {
            float d = (float)Math.Sqrt(dx * dx + dy * dy);

            if (d > Single.Epsilon) // probably not conservative enough, but catches the (0,0) case at least
            {
                dx = dx / d;
                dy = dy / d;
            }
            else
            {
                throw new ArgumentException("The input vector is too small to be normalized.");
            }
        }
        else
        {
            // In case one is NaN and not the other
            dx = dy = Single.NaN;
        }
    }

    #endregion
}

public class SplinesHelper
{
    public void Dispose()
    {
        foreach (var p in Presets)
        {
            p.Value.RendererLUT?.Destroy();
        }
        Renderer?.Destroy();
    }

    public RenderScripts Renderer { get; set; }

    public SplinesHelper()
    {

    }

    public Dictionary<string, FilterSplines> Presets { get; protected set; } = new(64);

    public void AddPreset(FilterSplines preset)
    {
        var existing = Presets.Any(x => x.Key == preset.Id);
        if (!existing)
            Presets.Add(preset.Id, preset);
    }

    public void SetPreset(string id)
    {
        var existing = Presets.FirstOrDefault(x => x.Key == id);
        if (existing.Value != null)
        {
            Current = existing.Value;
        }
        else
        {
            Current = null;
        }
    }


    private FilterSplines _Current;
    public FilterSplines Current
    {
        get { return _Current; }
        set
        {
            if (_Current != value)
            {
                _Current = value;
            }
        }
    }


    public void Initialize(RenderScript rs)
    {
        Renderer = new RenderScripts(rs);

        //normal
        var red = new CubicSpline((0, 0), (127, 127), (255, 255));
        var green = new CubicSpline((0, 0), (127, 127), (255, 255));
        var blue = new CubicSpline((0, 0), (127, 127), (255, 255));
        var preset = CreateFilterSplines(rs, "normal", red, green, blue);
        //Rendering.AddUpdateSplinesPreset(rs, preset.Id, preset.Red, preset.Green, preset.Blue);
        AddPreset(preset);

        //color negative AUTO
        red = new CubicSpline((33, 255), (119, 127), (185, 0));
        green = new CubicSpline((28, 255), (77, 127), (135, 0));
        blue = new CubicSpline((25, 255), (60, 127), (108, 0));
        preset = CreateFilterSplines(rs, "auto", red, green, blue);
        AddPreset(preset);


        //color negative FLAT
        red = new CubicSpline((0, 255), (127, 127), (255, 0));
        green = new CubicSpline((0, 255), (127, 127), (255, 0));
        blue = new CubicSpline((0, 255), (127, 127), (255, 0));
        preset = CreateFilterSplines(rs, "flat", red, green, blue);
        //Rendering.AddUpdateSplinesPreset(rs, preset.Id, preset.Red, preset.Green, preset.Blue);
        AddPreset(preset);


        //#if DEBUG
        //            //test
        //            System.Trace.WriteLine($"NEGA FLAT test");
        //            System.Trace.WriteLine($"Blue XS: {blue.Xs[0]} {blue.Xs[1]} {blue.Xs[2]}");
        //            blue.Eval(127 / 255f);
        //            blue.Eval(12 / 255f);
        //#endif


        //Rendering.AddUpdateSplinesPreset(rs, preset.Id, preset.Red, preset.Green, preset.Blue);
        ;
        //var check = Rendering.AvailableSplinePresets;
        SetPreset("auto");
    }


    //public FilterSplines PresetPositive { get; set; }

    //public FilterSplines PresetNegativeAuto { get; set; }

    //public FilterSplines PresetNegativeFlat { get; set; }

    public FilterSplines CreateFilterSplines(RenderScript rs, string id, CubicSpline red, CubicSpline green, CubicSpline blue, bool forceBW = false)
    {
        var ret = new FilterSplines
        {
            Id = id
        };

        //test all is working
        //_splineRed.Eval(213 /255f);
        //_splineGreen.Eval(154 /255f);
        // _splineBlue.Eval(95 /255f);

        //RED
        ret.Red = new ChannelSpline()
        {
            Xs = red.Xs,
            Ys = red.Ys,
            As = red.As,
            Bs = red.Bs,
            Tag = 'R'
        };


        //Green
        ret.Green = new ChannelSpline
        {
            Xs = green.Xs,
            Ys = green.Ys,
            As = green.As,
            Bs = green.Bs,
            Tag = 'G'
        };


        //Blue
        ret.Blue = new ChannelSpline
        {
            Xs = blue.Xs,
            Ys = blue.Ys,
            As = blue.As,
            Bs = blue.Bs,
            Tag = 'B'
        };

        ret.RendererLUT = ScriptIntrinsicLUT.Create(rs, Android.Renderscripts.Element.U8_4(rs));

        for (int x = 0; x < 256; x++)
        {
            float fX = x / 255f;

            ret.RendererLUT.SetAlpha(x, 255);

            var y = (int)Math.Round(red.Eval(fX) * 255.0);
            if (y > 255)
                y = 255;
            if (y < 0)
                y = 0;
            ret.RendererLUT.SetRed(x, y);

            y = (int)Math.Round(green.Eval(fX) * 255.0);
            if (y > 255)
                y = 255;
            if (y < 0)
                y = 0;
            ret.RendererLUT.SetGreen(x, y);

            y = (int)Math.Round(blue.Eval(fX) * 255.0);
            if (y > 255)
                y = 255;
            if (y < 0)
                y = 0;
            ret.RendererLUT.SetBlue(x, y);

        }

        return ret;
    }

}

public class FilterSplines
{
    public string Id { get; set; }
    public ChannelSpline Red { get; set; }
    public ChannelSpline Green { get; set; }
    public ChannelSpline Blue { get; set; }

    public ScriptIntrinsicLUT RendererLUT { get; set; }
}

public class CompareSizesByArea : Java.Lang.Object, IComparator
{
    public int Compare(Object lhs, Object rhs)
    {
        var lhsSize = (Android.Util.Size)lhs;
        var rhsSize = (Android.Util.Size)rhs;
        // We cast here to ensure the multiplications won't overflow
        return Long.Signum((long)lhsSize.Width * lhsSize.Height - (long)rhsSize.Width * rhsSize.Height);
    }


}

public class BitmapPool
{
    private Stack<AllocatedBitmap> pool = new();

    public AllocatedBitmap GetBitmap(RenderScript rs, int width, int height)
    {
        if (pool.Count > 0)
        {
            return pool.Pop();
        }
        else
        {
            return new AllocatedBitmap(rs, width, height);
        }
    }

    public void ReturnBitmap(AllocatedBitmap bitmap)
    {
        pool.Push(bitmap);
    }
}

public class AllocatedBitmap : IDisposable
{
    public Bitmap Bitmap { get; set; }
    public Allocation Allocation { get; set; }

    public AllocatedBitmap(RenderScript rs, int width, int height)
    {
        Bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
        Allocation = Allocation.CreateFromBitmap(rs,
            Bitmap,
            Allocation.MipmapControl.MipmapNone,
            AllocationUsage.Script);
        //Allocation.CreateFromBitmap(rs, Bitmap);
    }

    public void Update()
    {
        Allocation.CopyTo(Bitmap);
    }

    public void Dispose()
    {
        if (Allocation != null)
        {
            Allocation.Destroy();
            Allocation.Dispose();
            Allocation = null;
        }

        if (Bitmap != null)
        {
            //Bitmap.Recycle();
            Bitmap.Dispose();
            Bitmap = null;
        }
    }
}

public class DoubleBuffer
{
    private Bitmap[] bitmaps;
    private Allocation[] allocations;
    private int writeIndex;
    private int readIndex;

    public Bitmap WriteBitmap => bitmaps[writeIndex];
    public Bitmap ReadBitmap => bitmaps[readIndex];
    public Allocation WriteAllocation => allocations[writeIndex];
    public Allocation ReadAllocation => allocations[readIndex];

    public DoubleBuffer(RenderScript rs, int width, int height)
    {
        bitmaps = new Bitmap[2];
        allocations = new Allocation[2];

        bitmaps[0] = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
        bitmaps[1] = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

        allocations[0] = Allocation.CreateFromBitmap(rs, bitmaps[0]);
        allocations[1] = Allocation.CreateFromBitmap(rs, bitmaps[1]);

        writeIndex = 0;
        readIndex = 1;
    }

    public void Swap()
    {
        (writeIndex, readIndex) = (readIndex, writeIndex);
    }

    public void Dispose()
    {
        allocations[0].Destroy();
        allocations[0].Dispose();
        bitmaps[0].Dispose();

        allocations[1].Destroy();
        allocations[1].Dispose();
        bitmaps[1].Dispose();
    }
}