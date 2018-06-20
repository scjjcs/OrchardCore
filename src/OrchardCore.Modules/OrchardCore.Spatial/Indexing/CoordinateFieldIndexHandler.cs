using System.Threading.Tasks;
using OrchardCore.Indexing;
using OrchardCore.Spatial.Fields;

namespace OrchardCore.Spatial.Indexing
{
    public class CoordinateFieldIndexHandler : ContentFieldIndexHandler<CoordinateField>
    {
        public override Task BuildIndexAsync(CoordinateField field, BuildFieldIndexContext context)
        {
            var options = context.Settings.ToOptions();
            context.DocumentIndex.Entries.Add(context.Key, new DocumentIndex.DocumentIndexEntry(
                new DocumentIndex.Point
                {
                    X = field.Longitude,
                    Y = field.Latitude
                }, DocumentIndex.Types.GeoPoint, options));

            return Task.CompletedTask;
        }
    }
}