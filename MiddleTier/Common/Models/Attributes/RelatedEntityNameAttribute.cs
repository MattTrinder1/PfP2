﻿using Common.Models.Dataverse;

[System.AttributeUsage(System.AttributeTargets.Property)]
public class RelatedEntityNameAttribute : System.Attribute
{
    private string _name;
    private string _pluralName;

    public RelatedEntityNameAttribute(string name, string pluralName = "")
    {
        this._name = name;
        if (string.IsNullOrEmpty(pluralName))
        {
            this._pluralName = SchemaHelpers.GetEntityPluralName(this._name);
        }
        else
        {
            this._pluralName = pluralName;
        }
    }
    public string Name { get { return _name; } }
    public string PluralName { get { return _pluralName; } }
}