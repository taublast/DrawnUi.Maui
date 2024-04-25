using System.Numerics;

namespace DrawnUi.Maui.Draw;
/*
public class HelperSk3DView
{
    private Matrix4x4 transformationMatrix;

    public HelperSk3DView()
    {
        Reset();
    }

    public void Reset()
    {
        transformationMatrix = Matrix4x4.Identity;
    }

    public SKMatrix GetMatrix()
    {
        return ToSKMatrix(transformationMatrix);
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
        Translate(0, 0, z);
    }

    private void Rotate(float degrees, float x, float y, float z)
    {
        var radians = MathF.PI * degrees / 180.0f;
        var rotationMatrix = Matrix4x4.CreateFromAxisAngle(new Vector3(x, y, z), radians);
        transformationMatrix *= rotationMatrix;
    }

    private void Translate(float x, float y, float z)
    {
        var translationMatrix = Matrix4x4.CreateTranslation(x, y, z);
        transformationMatrix *= translationMatrix;
    }

}
*/

public class HelperSk3DView
{
    private Matrix4x4 transformationMatrix;
    private Vector3 cameraPosition = new Vector3(0, 0, -576); // 8 inches backward
    private Vector3 cameraAxis = new Vector3(0, 0, 1); // forward
    private Vector3 cameraZenith = new Vector3(0, -1, 0); // up

    public HelperSk3DView()
    {
        Reset();
    }

    public void Reset()
    {
        transformationMatrix = Matrix4x4.Identity;
        cameraAxis = new Vector3(0, 0, 1);
        cameraZenith = new Vector3(0, -1, 0);
        cameraPosition = new Vector3(0, 0, -576);
        UpdateViewMatrix();
    }

    public SKMatrix GetMatrix()
    {
        return ToSKMatrix(transformationMatrix);
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
        Translate(0, 0, z);
    }

    private void UpdateViewMatrix()
    {
        Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(cameraZenith, cameraAxis));
        cameraZenith = Vector3.Normalize(Vector3.Cross(cameraAxis, cameraRight));
        transformationMatrix = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraAxis, cameraZenith);
    }


    public void Translate(float x, float y, float z)
    {
        // Apply translation relative to camera's current position and orientation
        Vector3 translation = new Vector3(x, y, z);
        translation = Vector3.Transform(translation, Matrix4x4.CreateFromAxisAngle(cameraAxis, 0)); // Rotate translation to camera orientation
        cameraPosition += translation;
        UpdateViewMatrix();
    }

    public void Rotate(float degrees, float x, float y, float z)
    {
        // Rotate around an axis defined relative to the camera's current orientation
        Vector3 axis = new Vector3(x, y, z);
        axis = Vector3.Transform(axis, Matrix4x4.CreateFromAxisAngle(cameraAxis, 0)); // Adjust axis to camera orientation
        var radians = MathF.PI * degrees / 180.0f;
        var rotationMatrix = Matrix4x4.CreateFromAxisAngle(axis, radians);
        cameraAxis = Vector3.TransformNormal(cameraAxis, rotationMatrix);
        cameraZenith = Vector3.TransformNormal(cameraZenith, rotationMatrix);
        UpdateViewMatrix();
    }



}
