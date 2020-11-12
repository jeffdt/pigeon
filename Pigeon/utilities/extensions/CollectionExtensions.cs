using System.Collections.Generic;
using System.Linq;

namespace pigeon.utilities.extensions {
    public static class CollectionExtensions {
        public static List<string> FindStringMatches(this IEnumerable<string> candidates, string searchValue, bool isCaseSensitive = false) {
            string search;

            if (isCaseSensitive) {
                search = searchValue;
                return candidates.Where(candidateStr => candidateStr.Contains(search)).ToList();
            }

            search = searchValue.ToLower();
            return candidates.Where(candidateStr => candidateStr.ToLower().Contains(search)).ToList();
        }
    }
}
