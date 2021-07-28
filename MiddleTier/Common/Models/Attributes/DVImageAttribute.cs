[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
public class DVImageAttribute : System.Attribute
{
    private ImageRetrieveBehaviour retrieveImageType;

    public DVImageAttribute(ImageRetrieveBehaviour retrieveImageType = ImageRetrieveBehaviour.RequestDefault)
    {
        this.retrieveImageType = retrieveImageType;
    }
    public ImageRetrieveBehaviour RetrieveImageType { get { return this.retrieveImageType; } }
}

public enum ImageRetrieveBehaviour
{
    RequestDefault,
    AlwaysThumbnail,
    AlwaysFullImage
}