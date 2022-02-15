namespace PDF;

[AttributeUsage(AttributeTargets.Property)]
public class OrderPdfFieldNameAttribute : Attribute
{
    private readonly string _fieldName;

    public OrderPdfFieldNameAttribute(string fieldName)
    {
        _fieldName = fieldName;
    }

    public string FieldName => _fieldName;
}