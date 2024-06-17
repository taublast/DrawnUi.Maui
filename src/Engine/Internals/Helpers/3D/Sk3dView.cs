using Markdig.Extensions.Tables;
using Microsoft.Maui.Devices.Sensors;
using SkiaSharp;
using System.Numerics;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace DrawnUi.Maui.Draw;

public class Sk3dView
{

    public Sk3dView()
    {
        Reset();
    }

    public void Reset()
    {
        fCamera = new();
        fRec = new();
    }

    const float inchesToPoints = 72.0f;
    static readonly float SK_ScalarPI = (float)Math.PI;

    internal class Rec
    {
        public Rec()
        {
            fMatrix = SKMatrix44.Identity;
        }
        //disabled, do not need save/restore public - Rec fNext;
        public SKMatrix44 fMatrix;
    };

    internal Rec fRec;
    internal SkCamera3D fCamera;

    public void SetCameraLocation(float x, float y, float z)
    {
        // the camera location is passed in inches, set in pt
        float lz = z * inchesToPoints;
        fCamera.fLocation = new(x * inchesToPoints, y * inchesToPoints, lz);
        fCamera.fObserver = new(0, 0, lz);
        fCamera.Update();
    }

    public float GetCameraLocationX()
    {
        return fCamera.fLocation.X / inchesToPoints;
    }

    public float GetCameraLocationY()
    {
        return fCamera.fLocation.Y / inchesToPoints;
    }

    public float GetCameraLocationZ()
    {
        return fCamera.fLocation.X / inchesToPoints;
    }

    public void Translate(float x, float y, float z)
    {
        var preTranslate = SKMatrix44.CreateTranslation(x, y, z);
        fRec.fMatrix.PreConcat(preTranslate); //todo check if need use return value
    }

    public void RotateX(float deg)
    {
        var preRotate = SKMatrix44.CreateRotation(1, 0, 0, deg * SK_ScalarPI / 180f);
        fRec.fMatrix.PreConcat(preRotate); //todo check if need use return value

        //fRec->fMatrix.preConcat(SkM44::Rotate({ 1, 0, 0}, deg* SK_ScalarPI / 180));
    }

    public void RotateY(float deg)
    {
        var preRotate = SKMatrix44.CreateRotation(0, -1, 0, deg * SK_ScalarPI / 180f);
        fRec.fMatrix.PreConcat(preRotate); //todo check if need use return value

        //fRec->fMatrix.preConcat(SkM44::Rotate({ 0,-1, 0}, deg* SK_ScalarPI / 180));
    }

    public void RotateZ(float deg)
    {
        var preRotate = SKMatrix44.CreateRotation(0, 0, 1, deg * SK_ScalarPI / 180f);
        fRec.fMatrix.PreConcat(preRotate); //todo check if need use return value

        //fRec->fMatrix.preConcat(SkM44::Rotate({ 0, 0, 1}, deg* SK_ScalarPI / 180));
    }

    public void RotateXDegrees(float deg)
    {
        var preRotate = SKMatrix44.CreateRotationDegrees(1, 0, 0, deg);
        var check = fRec.fMatrix.PreConcat(preRotate); //todo check if need use return value
        var test = check == fRec.fMatrix;

    }

    public void RotateYDegrees(float deg)
    {
        var preRotate = SKMatrix44.CreateRotationDegrees(0, -1, 0, deg);
        fRec.fMatrix.PreConcat(preRotate); //todo check if need use return value
    }

    public void RotateZDegrees(float deg)
    {
        var preRotate = SKMatrix44.CreateRotationDegrees(0, 0, 1, deg);
        fRec.fMatrix.PreConcat(preRotate); //todo check if need use return value
    }

    public float DotWithNormal(float x, float y, float z)
    {
        SkPatch3D patch = new();
        patch.Transform(fRec.fMatrix);
        return patch.DotWith(x, y, z);
    }

    public SKMatrix GetMatrix()
    {
        SkPatch3D patch = new();
        patch.Transform(fRec.fMatrix);
        return fCamera.PatchToMatrix(patch);
    }
}


