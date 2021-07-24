[System.AttributeUsage(System.AttributeTargets.Property)]
public class DVImageAttribute : System.Attribute
{
    private bool retrieveFullImage;

    public DVImageAttribute(bool retrieveFullImage)
    {
        this.retrieveFullImage = retrieveFullImage;
    }
    public bool RetrieveFullImage { get { return retrieveFullImage; } }
}