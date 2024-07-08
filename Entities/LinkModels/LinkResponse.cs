using Entities.Entities;

namespace Entities.LinkModels
{
    public class LinkResponse
    {
        public bool HasLinks { get; set; }
        // if we don't need links in our response
        public List<Entity> ShapedEntities { get; set; }
        // if we need links iin response we return shaped
        // Entities collection + Links collection
        public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }

        public LinkResponse()
        {
            ShapedEntities = new List<Entity>();
            LinkedEntities = new LinkCollectionWrapper<Entity>();
        }
    }
}
