using System;
using System.Text.RegularExpressions;

namespace PopcatClient.Updater
{
    public class VersionName
    {
        public VersionName(string stringVersionName) {
            // rawVersion accepts formats like the following:
            //     (v)<Major>.<Minor>(.Patch)(-Flag)   <= Deprecated
            // validate the format:
            var r = new Regex(@"^v?\d{1,5}(?:\.\d{1,5}){1,2}(?:\-(?:stable|beta(?:\.\d)?))?$",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!r.IsMatch(stringVersionName))
                throw new ArgumentException("The version name provided is invalid.", nameof(stringVersionName));
            
            if (stringVersionName.StartsWith("v")) stringVersionName = stringVersionName.Substring(1); // Removes leading 'v'

            var flagStr = stringVersionName.Contains("-") ? stringVersionName.Split('-')[1].ToLower() : string.Empty;
            var flagName = flagStr.Split('.')[0];
            FlagName = flagName switch
            {
                "stable" => VersionFlag.Stable,
                "beta" => VersionFlag.Beta,
                _ => VersionFlag.Stable
            };

            // Set beta build if the version is a beta
            if (FlagName == VersionFlag.Beta)
                BetaBuild = flagStr.Split('.').Length > 1 ? int.Parse(flagStr.Split('.')[1]) : 1;
            else
                BetaBuild = null;

            stringVersionName = stringVersionName.Replace(flagStr, string.Empty); // Removes flag from version name

            var nums = stringVersionName.Split('.');
            Major = int.Parse(nums[0].Replace("-", string.Empty));
            Minor = int.Parse(nums[1].Replace("-", string.Empty));
            Patch = nums.Length == 3 ? int.Parse(nums[2].Replace("-", string.Empty)) : 0;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public VersionFlag FlagName { get; set; }
        public int? BetaBuild { get; set; }
        public bool PreRelease => FlagName == VersionFlag.Beta;

        public string GetFourDigitVersionName(int lastDigit = 0) => $"{Major}.{Minor}.{Patch}.{lastDigit}";

        public static bool operator >(VersionName a, VersionName b)
        {
            if (a == b) return false;

            if (a.Major > b.Major) return true;
            if (a.Major < b.Major) return false;

            if (a.Minor > b.Minor) return true;
            if (a.Minor < b.Minor) return false;

            if (a.Patch > b.Patch) return true;
            if (a.Patch < b.Patch) return false;

            if (!a.PreRelease && b.PreRelease) return true;
            if (a.PreRelease && !b.PreRelease) return false;

            if (a.PreRelease && b.PreRelease)
            {
                if (a.BetaBuild > b.BetaBuild) return true;
                if (a.BetaBuild < b.BetaBuild) return false;
            }

            return false;
        }

        public static bool operator <(VersionName a, VersionName b)
        {
            if (a == b) return false;
            return !(a > b);
        }

        public static bool operator >=(VersionName a, VersionName b) => a > b || a == b;

        public static bool operator <=(VersionName a, VersionName b) => a < b || a == b;

        public static bool operator ==(VersionName a, VersionName b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Patch == b.Patch && a.FlagName == b.FlagName && a.BetaBuild == b.BetaBuild;
        }

        public static bool operator !=(VersionName a, VersionName b)
        {
            return !(a == b);
        }

        public static implicit operator VersionName(string versionNameString) => new(versionNameString);

        public static bool VersionNameIsValid(string versionName) {
            try {
                _ = new VersionName(versionName);
                return true;
            } catch {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            try
            {
                var castedObj = (VersionName) obj;
                return this == castedObj;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hash = 23957;
            hash = hash * 723 + ToString(true, true, true, true).GetHashCode();
            return hash;
        }

        public string ToString(bool leadingV = true,
            bool alwaysShowPatchDigit = false,
            bool alwaysShowTagName = false,
            bool alwaysShowBetaBuild = false)
        {
            var str = string.Empty;
            str += leadingV ? "v" : "";
            str += Major + "." + Minor;
            str += alwaysShowPatchDigit || Patch != 0 ? "." + Patch : "";
            if (!PreRelease)
                str += alwaysShowTagName ? "-stable" : "";
            else
                str += "-beta" + (BetaBuild == 1
                    ? alwaysShowBetaBuild ? "." + BetaBuild : ""
                    : "." + BetaBuild);
            return str;
        }

        public enum VersionFlag {
            Stable, Beta
        }
    }
}