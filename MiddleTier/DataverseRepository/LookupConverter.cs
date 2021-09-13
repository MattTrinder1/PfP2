using API.Models.IYC;
using Common.Models.Business;
using Common.Models.Dataverse;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.DataverseAccess
{
    public class LookupJsonConverter : JsonConverter<EntityRef>
    {
        public override EntityRef Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                new EntityRef("",Guid.NewGuid());

        public override void Write(
            Utf8JsonWriter writer,
            EntityRef er,
            JsonSerializerOptions options) =>
                writer.WriteStringValue( $"/{SchemaHelpers.GetEntityPluralName(er.EntityLogicalName)}({er.EntityId.ToString()})" );
    }
}