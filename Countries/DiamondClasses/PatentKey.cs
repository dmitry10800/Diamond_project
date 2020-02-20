using System;
using System.Text.RegularExpressions;

namespace Integration
{
    public class PatentKey
    {
        private static readonly Regex _patentKeyRegex = new Regex("^(?<authority>[A-Z]{2})(?<number>[A-Z0-9]+?)(?<kind>[A-Z][0-9]?)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly string _key;

        public PatentKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Patent key cannot be null");
            }

            var match = _patentKeyRegex.Match(key);
            if (!match.Success)
            {
                throw new ArgumentException($"Patent key must be in the format {_patentKeyRegex}");
            }

            _key = key;
            Authority = match.Groups["authority"].Value;
            Number = match.Groups["number"].Value;
            Kind = match.Groups["kind"].Value;
        }

        public PatentKey(string authority, string number, string kind) : this($"{authority}{number}{kind}") { }

        public string Authority { get; }
        public string Number { get; }
        public string Kind { get; }

        public static implicit operator string(PatentKey patentKey) => patentKey._key;
        public static explicit operator PatentKey(string value) => new PatentKey(value);

        protected bool Equals(PatentKey other) => string.Equals(_key, other._key);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PatentKey)obj);
        }

        public override int GetHashCode() => _key != null ? _key.GetHashCode() : 0;

        public override string ToString() => _key;
    }
}