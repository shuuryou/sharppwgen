using System;
using System.Collections.Generic;
using System.Globalization;

namespace pwgen
{
    /*
     * SharpPwgen.cs
     *
     * Copyright (C) 2003-2006 KATO Kazuyoshi <kzys@8-p.info>
     * Copyright (C) 2022 shuuryou
     *
     * This program is a C# port of a JavaScript port of pwgen.
     * The original C source code written by Theodore Ts'o.
     * <http://sourceforge.net/projects/pwgen/>
     * The original JavaScript source code written by Kazuyoshi Kato.
     * <https://github.com/kzys/pwgen-js/>
     * 
     * This file may be distributed under the terms of the GNU General
     * Public License.
     */
    public static class SharpPwgen
    {
        private static Random RANDOM;

        static SharpPwgen()
        {
            FillPhonemes();
            RANDOM = new Random();
        }

        public static string Generate(int maxLength = 8,
            bool includeCapitalLetter = true, bool includeNumber = true)
        {
            string result = string.Empty;

            while (string.IsNullOrEmpty(result))
                result = Generate0(maxLength, includeCapitalLetter, includeNumber);

            return result;
        }

        private static string Generate0(int maxLength, bool includeCapitalLetter, bool includeNumber)
        {
            string result = string.Empty;
            Phoneme prev = Phoneme.NONE;
            bool haveNumber = false;
            bool haveCapitalLetter = false;
            bool isFirst = true;

            var shouldBe = (RANDOM.NextDouble() < 0.5) ? Phoneme.VOWEL : Phoneme.CONSONANT;

            while (result.Length < maxLength)
            {
                int i = RANDOM.Next(0, ELEMENTS.Count);
                string str = ELEMENTS[i].Key;
                Phoneme flags = ELEMENTS[i].Value;

                /* Filter on the basic type of the next element */
                if ((flags & shouldBe) == 0)
                    continue;
                /* Handle the NOT_FIRST flag */
                if (isFirst && (flags.HasFlag(Phoneme.NOT_FIRST)))
                    continue;
                /* Don't allow VOWEL followed a Vowel/Dipthong pair */
                if ((prev.HasFlag(Phoneme.VOWEL)) && (flags.HasFlag(Phoneme.VOWEL)) && (flags.HasFlag(Phoneme.DIPTHONG)))
                    continue;
                /* Don't allow us to overflow the buffer */
                if (result.Length + str.Length > maxLength)
                    continue;

                if (includeCapitalLetter && !haveCapitalLetter)
                {
                    if ((isFirst || (flags.HasFlag(Phoneme.CONSONANT))) &&
                        (RANDOM.NextDouble() > 0.3))
                    {
                        str = str.Substring(0, 1).ToUpperInvariant() + str.Substring(1);
                        haveCapitalLetter = true;
                    }
                }

                /*
                 * OK, we found an element which matches our criteria,
                 * let's do it!
                 */
                result += str;

                if (includeNumber && !haveNumber)
                {
                    if (!isFirst && (RANDOM.NextDouble() < 0.3))
                    {
                        result += RANDOM.Next(0, 10).ToString(CultureInfo.InvariantCulture);
                        haveNumber = true;

                        isFirst = true;
                        prev = 0;
                        shouldBe = (RANDOM.NextDouble() < 0.5) ? Phoneme.VOWEL : Phoneme.CONSONANT;
                        continue;
                    }
                }

                /*
                 * OK, figure out what the next element should be
                 */
                if (shouldBe == Phoneme.CONSONANT)
                {
                    shouldBe = Phoneme.VOWEL;
                }
                else
                { /* should_be == VOWEL */
                    if ((prev.HasFlag(Phoneme.VOWEL)) ||
                        (flags.HasFlag(Phoneme.DIPTHONG)) || (RANDOM.Next() > 0.3))
                    {
                        shouldBe = Phoneme.CONSONANT;
                    }
                    else
                    {
                        shouldBe = Phoneme.VOWEL;
                    }
                }
                prev = flags;
                isFirst = false;
            }

            if ((includeCapitalLetter && !haveCapitalLetter) || (includeNumber && !haveNumber))
                return null;

            return result;
        }

        [Flags]
        private enum Phoneme
        {
            NONE = 0,
            CONSONANT = 1,
            VOWEL = 1 << 1,
            DIPTHONG = 1 << 2,
            NOT_FIRST = 1 << 3
        }

        private static List<KeyValuePair<string, Phoneme>> ELEMENTS = new List<KeyValuePair<string, Phoneme>>();

        private static void FillPhonemes()
        {
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("a", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ae", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ah", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ai", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("b", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("c", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ch", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("d", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("e", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ee", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ei", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("f", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("g", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("gh", Phoneme.CONSONANT | Phoneme.DIPTHONG | Phoneme.NOT_FIRST));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("h", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("i", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ie", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("j", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("k", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("l", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("m", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("n", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ng", Phoneme.CONSONANT | Phoneme.DIPTHONG | Phoneme.NOT_FIRST));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("o", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("oh", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("oo", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("p", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ph", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("qu", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("r", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("s", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("sh", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("t", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("th", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("u", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("v", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("w", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("x", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("y", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("z", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ae", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ah", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ai", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("b", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("c", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ch", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("d", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("e", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ee", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ei", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("f", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("g", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("gh", Phoneme.CONSONANT | Phoneme.DIPTHONG | Phoneme.NOT_FIRST));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("h", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("i", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ie", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("j", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("k", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("l", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("m", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("n", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ng", Phoneme.CONSONANT | Phoneme.DIPTHONG | Phoneme.NOT_FIRST));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("o", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("oh", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("oo", Phoneme.VOWEL | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("p", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("ph", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("qu", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("r", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("s", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("sh", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("t", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("th", Phoneme.CONSONANT | Phoneme.DIPTHONG));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("u", Phoneme.VOWEL));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("v", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("w", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("x", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("y", Phoneme.CONSONANT));
            ELEMENTS.Add(new KeyValuePair<string, Phoneme>("z", Phoneme.CONSONANT));
        }
    }
}