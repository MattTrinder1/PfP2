using API.Models.IYC;
using Common.Models.Business;
using Common.Models.Dataverse;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.DataverseAccess
{
    public class LookupJsonConverter : JsonConverter<EntityReference>
    {
        public override EntityReference Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                new EntityReference("",Guid.NewGuid());

        public override void Write(
            Utf8JsonWriter writer,
            EntityReference er,
            JsonSerializerOptions options) =>
                writer.WriteStringValue( $"/{SchemaHelpers.GetEntityPluralName(er.EntityLogicalName)}({er.EntityId.ToString()})" );
    }
}