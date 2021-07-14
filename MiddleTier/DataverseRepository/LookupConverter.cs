using API.Models.IYC;
using Common.Models.Business;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
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
                writer.WriteStringValue( $"{er.EntityLogicalName}s({er.EntityId.ToString()})" );
    }
}