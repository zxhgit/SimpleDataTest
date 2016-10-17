using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zx.Web.ApiContainer.core
{
    /// <summary>
    /// This is a container for prefix values. It normalizes all the values into dotted-form and then stores
    /// them in a sorted array. All queries for prefixes are also normalized to dotted-form, and searches
    /// for ContainsPrefix are done with a binary search.
    /// </summary>
    internal class PrefixContainer
    {
        private readonly ICollection<string> _originalValues;
        private readonly string[] _sortedValues;

        internal PrefixContainer(ICollection<string> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            _originalValues = values;
            _sortedValues = values.Where(val => val != null).ToArray();
            Array.Sort(_sortedValues, StringComparer.OrdinalIgnoreCase);
        }

        internal bool ContainsPrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            // only match empty string when we have some value
            if (prefix.Length == 0) return _sortedValues.Length > 0;
            return Array.BinarySearch(_sortedValues, prefix, new PrefixComparer(prefix)) > -1;
        }

        // Given "foo.bar", "foo.hello", "something.other", foo[abc].baz and asking for prefix "foo" will return:
        // - "bar"/"foo.bar"
        // - "hello"/"foo.hello"
        // - "abc"/"foo[abc]"
        internal IDictionary<string, string> GetKeysFromPrefix(string prefix)
        {
            IDictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in _originalValues)
            {
                if (entry == null) continue;
                if (entry.Length == prefix.Length) continue;// No key in this entry
                if (prefix.Length == 0)
                    GetKeyFromEmptyPrefix(entry, result);
                else if (entry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    GetKeyFromNonEmptyPrefix(prefix, entry, result);
            }
            return result;
        }

        private static void GetKeyFromEmptyPrefix(string entry, IDictionary<string, string> results)
        {
            var dotPosition = entry.IndexOf('.');
            var bracketPosition = entry.IndexOf('[');
            var delimiterPosition = -1;
            if (dotPosition == -1 && bracketPosition != -1)
                delimiterPosition = bracketPosition;
            else
                delimiterPosition = bracketPosition == -1 ? dotPosition : Math.Min(dotPosition, bracketPosition);
            var key = delimiterPosition == -1 ? entry : entry.Substring(0, delimiterPosition);
            results[key] = key;
        }

        private static void GetKeyFromNonEmptyPrefix(string prefix, string entry, IDictionary<string, string> results)
        {
            string key;
            string fullName;
            var keyPosition = prefix.Length + 1;
            switch (entry[prefix.Length])
            {
                case '.':
                    var dotPosition = entry.IndexOf('.', keyPosition);
                    if (dotPosition == -1)
                        dotPosition = entry.Length;
                    key = entry.Substring(keyPosition, dotPosition - keyPosition);
                    fullName = entry.Substring(0, dotPosition);
                    break;
                case '[':
                    var bracketPosition = entry.IndexOf(']', keyPosition);
                    if (bracketPosition == -1)
                        return;// Malformed for dictionary
                    key = entry.Substring(keyPosition, bracketPosition - keyPosition);
                    fullName = entry.Substring(0, bracketPosition + 1);
                    break;
                default:
                    return;
            }
            if (!results.ContainsKey(key))
                results.Add(key, fullName);
        }

        internal static bool IsPrefixMatch(string prefix, string testString)
        {
            if (testString == null) return false;
            // shortcut - non-null testString matches empty prefix
            if (prefix.Length == 0) return true;
            if (prefix.Length > testString.Length) return false; // not long enough
            // prefix doesn't match
            if (!testString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;
            if (testString.Length == prefix.Length) return true; // exact match
            // invariant: testString.Length > prefix.Length
            switch (testString[prefix.Length])
            {
                case '.':
                case '[':
                    return true; // known delimiters
                default:
                    return false; // not known delimiter
            }
        }

        private class PrefixComparer : IComparer<String>
        {
            private string _prefix;

            public PrefixComparer(string prefix)
            {
                _prefix = prefix;
            }

            public int Compare(string x, string y)
            {
                var testString = Object.ReferenceEquals(x, _prefix) ? y : x;
                if (IsPrefixMatch(_prefix, testString)) return 0;
                return StringComparer.OrdinalIgnoreCase.Compare(x, y);
            }
        }
    }
}