using System.Numerics;

namespace DrawnUi.Draw;

public class HelperSk3dView
{
    private Matrix4x4 transformationMatrix;

    public HelperSk3dView()
    {
        Reset();
    }

    public void Reset()
    {
        transformationMatrix = Matrix4x4.Identity;
    }

    public SKMatrix Matrix
    {
        get
        {
            return ToSKMatrix(transformationMatrix);
        }
    }

    public static SKMatrix ToSKMatrix(Matrix4x4 matrix)
    {
        return new SKMatrix
        {
            ScaleX = matrix.M11,
            SkewX = matrix.M21,
            TransX = matrix.M41,
            SkewY = matrix.M12,
            ScaleY = matrix.M22,
            TransY = matrix.M42,
            Persp0 = matrix.M14,
            Persp1 = matrix.M24,
            Persp2 = matrix.M44
        };
    }

    public void RotateXDegrees(float degrees)
    {
        Rotate(degrees, 1, 0, 0);
    }

    public void RotateYDegrees(float degrees)
    {
        Rotate(degrees, 0, 1, 0);
    }

    public void RotateZDegrees(float degrees)
    {
        Rotate(degrees, 0, 0, 1);
    }

    public void TranslateZ(float z)
    {
        // Matrix4x4 does not directly support Z translation in a 3D to 2D mapping context
        // we'll just update the translation component of the matrix
        // For more accurate 3D effect need a more complex 3D rendering approach
        Translate(0, 0, z);
    }

    private void Rotate(float degrees, float x, float y, float z)
    {
        var radians = Math.PI * degrees / 180.0f;
        var rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(x, y, z), (float)radians);
        transformationMatrix *= rotationMatrix;
    }

    private void Translate(float x, float y, float z)
    {
        var translationMatrix = Matrix4x4.CreateTranslation(x, y, z);
        transformationMatrix *= translationMatrix;
    }
}