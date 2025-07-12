using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentApp.Api.Utilities;

public class ForgivingGuidConverter : JsonConverter<Guid>
{
	public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String)
		{
			var s = reader.GetString();
			if (Guid.TryParse(s, out var g))
				return g;
			return Guid.Empty;
		}
		return reader.GetGuid();
	}

	public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}