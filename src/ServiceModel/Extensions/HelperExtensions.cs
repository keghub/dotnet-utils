using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;

namespace EMG.Utilities.Extensions
{
    public static class Extensions
    {
        public static void UpdateOrAdd<TItem>(this KeyedByTypeCollection<IServiceBehavior> collection, Action<TItem> action) where TItem : class, IServiceBehavior, new() => UpdateOrAdd<IServiceBehavior, TItem>(collection, action);

        private static void UpdateOrAdd<T, TItem>(KeyedByTypeCollection<T> collection, Action<TItem> action) where TItem : class, T, new()
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var item = collection.Find<TItem>();

            if (item != null)
            {
                action(item);
            }
            else
            {
                item = new TItem();
                action(item);
                collection.Add(item);
            }
        }

        private static bool Contains<T, TItem>(KeyedByTypeCollection<T> collection)
            where TItem : class, T
        {
            return collection.Any(item => item is TItem);
        }

        public static bool HasBehavior<TItem>(this ServiceEndpoint endpoint)
            where TItem : class, IEndpointBehavior =>
            Contains<IEndpointBehavior, TItem>(endpoint.Behaviors);
    }
}
