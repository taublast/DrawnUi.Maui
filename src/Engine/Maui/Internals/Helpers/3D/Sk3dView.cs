namespace DrawnUi.Maui.Draw;

using SkiaSharp;

/// <summary>
/// Under construction, custom implementation of removed API
/// </summary>
public class Sk3dView
{
    private float _rotationX = 0;
    private float _rotationY = 0;
    private float _rotationZ = 0;
    private float _translationX = 0;
    private float _translationY = 0;
    private float _translationZ = 0;
    private SKMatrix _source;

    public Sk3dView()
    {
        Reset();
    }

    protected bool Invalidated;
    private SKMatrix _matrix;

    private readonly Stack<SKMatrix> _states = new();

    public void Save()
    {
        _states.Push(Matrix);
    }

    public void Restore()
    {
        if (_states.TryPop(out var matrix))
        {
            _source = matrix;
            Invalidated = true;

            OnRestore();
        }
    }

    public void ApplyToCanvas(SKCanvas canvas)
    {
        if (canvas == null)
            throw new ArgumentNullException(nameof(canvas));

        canvas.Concat(Matrix);
    }

    /// <summary>
    /// Resets the current state and clears all saved states
    /// </summary>
    public void Reset()
    {
        _states.Clear();

        _source = SKMatrix.Identity;

        _rotationX = 0;
        _rotationY = 0;
        _rotationZ = 0;
        _translationX = 0;
        _translationY = 0;
        _translationZ = 0;

        Invalidated = true;

        OnReset();
    }

    protected virtual void OnReset()
    {

    }

    protected virtual void OnRestore()
    {

    }

    public void RotateXDegrees(float degrees)
    {
        _rotationX += degrees;
        Invalidated = true;
    }

    public void RotateYDegrees(float degrees)
    {
        _rotationY += degrees;
        Invalidated = true;
    }

    public void RotateZDegrees(float degrees)
    {
        _rotationZ += degrees;
        Invalidated = true;
    }

    public void TranslateX(float value)
    {
        _translationX += value;
        Invalidated = true;
    }

    public void TranslateY(float value)
    {
        _translationY += value;
        Invalidated = true;
    }

    public void TranslateZ(float value)
    {
        _translationZ += value;
        Invalidated = true;
    }

    public void Translate(float x, float y, float z)
    {
        _translationX += x;
        _translationY += y;
        _translationZ += z;

        Invalidated = true;
    }

    public SKMatrix Matrix
    {
        get
        {
            if (Invalidated)
            {
                _matrix = CreateProjectionMatrix();

                Invalidated = false;
            }
            return _matrix;
        }
    }

    /// <summary>
    /// 2D magic number camera distance 8 inches
    /// </summary>
    public float CameraDistance = 576;

    private SKMatrix CreateProjectionMatrix()
    {
        float radX = SkiaControl.DegreesToRadians(_rotationX);
        float radY = SkiaControl.DegreesToRadians(_rotationY);
        float radZ = SkiaControl.DegreesToRadians(_rotationZ);

        // Compute sine and cosine of rotation angles
        float sinX = MathF.Sin(radX);
        float cosX = MathF.Cos(radX);
        float sinY = MathF.Sin(radY);
        float cosY = MathF.Cos(radY);
        float sinZ = MathF.Sin(radZ);
        float cosZ = MathF.Cos(radZ);

        // Compute the combined rotation matrix elements
        float m00 = cosY * cosZ;
        float m01 = cosY * sinZ;
        float m02 = -sinY;
        float m10 = sinX * sinY * cosZ - cosX * sinZ;
        float m11 = sinX * sinY * sinZ + cosX * cosZ;
        float m12 = sinX * cosY;
        float m20 = cosX * sinY * cosZ + sinX * sinZ;
        float m21 = cosX * sinY * sinZ - sinX * cosZ;
        float m22 = cosX * cosY;

        // Include translation
        float tx = -_translationX;
        float ty = -_translationY;
        float tz = -_translationZ;

        // Set camera distance, 2d magic number 8 inches
        float dz = CameraDistance;

        // Adjust m22 to include _translationZ
        float m22tz = m22 + tz;

        // pre-compute dividers
        float m20DivDz = m20 / dz;
        float m21DivDz = m21 / dz;
        float m22tzDivDz = m22tz / dz;

        //build new matrix
        SKMatrix matrix = new SKMatrix
        {
            ScaleX = m00 + m20DivDz * tx,
            SkewX = m01 + m21DivDz * tx,
            TransX = tx + (m02 + m22tzDivDz * tx),

            SkewY = m10 + m20DivDz * ty,
            ScaleY = m11 + m21DivDz * ty,
            TransY = ty + (m12 + m22tzDivDz * ty),

            Persp0 = m20DivDz,
            Persp1 = m21DivDz,
            Persp2 = 1f + m22tzDivDz
        };

        //combine with pre-saved one
        //todo not working as original for pre-saved _source
        //todo original implementation is using Patch

        return matrix.PreConcat(_source);
    }

}






