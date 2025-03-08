namespace DrawnUi.Maui.Infrastructure;

public class Vector
{
    public static double Radians(double degrees)
    {
        return degrees * 0.0174533;
    }

    private double xComp, yComp;
    private double magnitude, angle;

    public Vector()
    {

    }

    public Vector(double x1, double y1, double x2, double y2)
    {


        xComp = x2 - x1;
        yComp = y2 - y1;

        calculateMagnitude();

        xComp /= magnitude;
        yComp /= magnitude;

        magnitude = 1;
        calculateAngle();
    }



    public Vector(double xComp, double yComp)
    {

        this.xComp = xComp;
        this.yComp = yComp;
        calculateMagnitude();
        calculateAngle();
    }

    public void setComponents(double xComp, double yComp)
    {
        this.xComp = xComp;
        this.yComp = yComp;

        calculateMagnitude();
        calculateAngle();
    }

    public double getXComp()
    {
        return xComp;
    }

    public void setXComp(double xComp)
    {
        this.xComp = xComp;

        calculateMagnitude();
        calculateAngle();
    }

    public double getYComp()
    {
        return yComp;
    }

    public void setYComp(double yComp)
    {
        this.yComp = yComp;

        calculateMagnitude();
        calculateAngle();
    }

    public Vector add(Vector other)
    {
        return new Vector(xComp + other.xComp,
            yComp + other.yComp);
    }

    public void mAdd(Vector other)
    {
        setComponents(xComp + other.xComp, yComp + other.yComp);
    }

    public void setAngle(double angle)
    {
        //		if(angle < 0) 
        //			angle += 2.0 * Math.PI;
        //		if(angle > 2.0 * Math.PI)
        //			angle -= 2.0 * Math.PI;

        this.angle = angle;

        xComp = magnitude * Math.Cos(angle);
        yComp = magnitude * Math.Sin(angle);
    }

    public void setMagnitude(double magnitude)
    {
        this.magnitude = Math.Abs(magnitude);
        xComp = magnitude * Math.Cos(angle);
        yComp = magnitude * Math.Sin(angle);

        if (magnitude < 0)
            setAngle(angle - Math.PI);
    }

    public double getMagnitude()
    {
        return magnitude;
    }

    private void calculateMagnitude()
    {
        magnitude = Math.Sqrt(Math.Pow(xComp, 2) + Math.Pow(yComp, 2));
    }

    public void mScale(double scalar)
    {
        setMagnitude(magnitude * scalar);
    }

    public Vector scale(double scalar)
    {
        return new Vector(xComp * scalar, yComp * scalar);
    }

    public double getAngle()
    {
        return angle;
    }

    private void calculateAngle()
    {
        angle = Math.Atan2(yComp, xComp);

        //		if(this.angle < 0) 
        //			this.angle += 2.0 * Math.PI;
    }
}