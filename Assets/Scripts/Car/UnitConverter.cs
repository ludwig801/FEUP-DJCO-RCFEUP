public static class UnitConverter
{
    public static float VelocityToKMH(float velocity)
    {
        return velocity * 3.6f;
    }

    public static float KmhToVelocity(float kmh)
    {
        return kmh / 3.6f;
    }
}