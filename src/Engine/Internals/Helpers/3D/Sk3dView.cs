namespace DrawnUi.Maui.Draw;

/*
  public void RotateXDegrees(float degrees)
   {
       var rotation = SKMatrix44.CreateRotationDegrees(1, 0, 0, degrees);
       matrix44 = rotation.PreConcat(matrix44);
   }

   public void RotateYDegrees(float degrees)
   {
       var rotation = SKMatrix44.CreateRotationDegrees(0, 1, 0, degrees);
       matrix44 = rotation.PreConcat(matrix44);
   }

   public void RotateZDegrees(float degrees)
   {
       var rotation = SKMatrix44.CreateRotationDegrees(0, 0, 1, degrees);
       matrix44 = rotation.PreConcat(matrix44);
   }
 */

using SkiaSharp;
using System.Numerics;

public class Sk3dView
{
    private Matrix4x4 matrix44;
    private Vector3 cameraPosition;
    private Vector3 cameraTarget;
    private Vector3 cameraUp;

    private float cameraRotationX = 0;
    private float cameraRotationY = 0;
    private float cameraRotationZ = 0;

    private float cameraTranslationZ = 0;

    public Sk3dView()
    {
        Reset();
    }

    public void Reset()
    {
        matrix44 = Matrix4x4.Identity;
        cameraRotationX = 0;
        cameraRotationY = 0;
        cameraRotationZ = 0;

        //        cameraPosition = new Vector3(0, 0, -1);
        cameraTarget = new Vector3(0, 0, 0);
        cameraUp = new Vector3(0, 1, 0);
    }

    public void RotateXDegrees(float degrees)
    {
        cameraRotationX -= degrees / 360.0f;
    }

    public void RotateYDegrees(float degrees)
    {
        cameraRotationY += degrees / 360.0f;
    }

    public void RotateZDegrees(float degrees)
    {
        cameraRotationZ += degrees;
    }

    public void Translate(float x, float y, float z)
    {
        cameraTranslationZ = z;

        //var translation = Matrix4x4.CreateTranslation(x, y, z);
        // matrix44 = translation * matrix44;
    }

    public SKMatrix GetMatrix()
    {
        var viewMatrix = CreateViewMatrix();
        var projectionMatrix = CreateProjectionMatrix();
        var mvpMatrix = viewMatrix * projectionMatrix;

        return ConvertToSKMatrix(mvpMatrix);
    }


    private Matrix4x4 CreateViewMatrix()
    {

        // Apply camera rotations to the camera's orientation
        var rotationX = Matrix4x4.CreateRotationX(SkiaControl.DegreesToRadians(cameraRotationX));
        var rotationY = Matrix4x4.CreateRotationY(SkiaControl.DegreesToRadians(cameraRotationY));
        var rotationZ = Matrix4x4.CreateRotationZ(SkiaControl.DegreesToRadians(cameraRotationZ));

        // Combine rotations
        var rotationMatrix = rotationY * rotationX * rotationZ;

        // Apply rotations to camera direction
        var forward = Vector3.Transform(new Vector3(0, 0, -1), rotationMatrix);

        cameraPosition = new Vector3(0, 1 + cameraTranslationZ, 1);

        // Update camera target
        cameraTarget = cameraPosition + forward;

        // Update cameraUp
        cameraUp = Vector3.Transform(new Vector3(0, 1, 0), rotationMatrix);

        // Create the view matrix
        var viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, cameraTarget, cameraUp);

        return viewMatrix;
    }


    private Matrix4x4 CreateProjectionMatrix()
    {
        float fieldOfView = MathF.PI / 2;
        float aspectRatio = 1.0f;
        float nearPlane = 0.1f;
        float farPlane = 1f;

        if (fieldOfView == 0 || float.IsInfinity(fieldOfView))
        {
            fieldOfView = MathF.PI / 2;
        }

        return Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
    }


    private SKMatrix ConvertToSKMatrix(Matrix4x4 m)
    {
        //return new SKMatrix(
        //m.M00, 
        //m.m10, 
        //m.m30, 
        //m.m01, 
        //m.m11, 
        //m.m31, 
        //m.m03, 
        //m.m13, 
        //m.m33);

        //-m.M11,
        //-m.M21,
        //-m.M41,
        //m.M12,
        //m.M22,
        //m.M42,
        //m.M14,
        //m.M24,
        //m.M44);

        // Extract the relevant elements for the 3x3 matrix
        float m00 = m.M11; // ScaleX
        float m01 = m.M21; // SkewX
        float m02 = m.M41; // Translation X
        float m10 = m.M21; // SkewY
        float m11 = m.M22; // ScaleY
        float m12 = m.M24; // Translation Y
        float m20 = m.M41; // Perspective X
        float m21 = m.M24; // Perspective Y
        float m22 = m.M44; // Perspective W


        // Create the SKMatrix
        SKMatrix matrix = new SKMatrix
        {
            ScaleX = m00,
            SkewX = m01,
            TransX = m02,
            SkewY = m10,
            ScaleY = m11,
            TransY = m12,
            Persp0 = m20,
            Persp1 = m21,
            Persp2 = m22
        };

        return matrix;
    }
}






