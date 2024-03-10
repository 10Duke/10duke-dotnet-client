using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace Tenduke.Client.Authorization;

internal class SnakeCaseLowerJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return string.Concat(
            name.Select(
                (x, i) =>
                    i > 0 && char.IsUpper(x)
                        ? "_" + char.ToLowerInvariant(x).ToString(CultureInfo.InvariantCulture)
                        : char.ToLowerInvariant(x).ToString(CultureInfo.InvariantCulture)
            )
        );
    }
}
