/// <summary>
/// DV properties of type byte array (byte[]) are assumed to represent image data unless attributed with DVNotImageAttribute.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
public class DVNotImageAttribute : System.Attribute
{
    public DVNotImageAttribute()
    {
    }
}