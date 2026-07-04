// Arabic text reshaper for Unity TextMeshPro.
// Based on the ArabicSupport library by Abdulla Konash (MIT license).
// Converts Arabic characters into their contextual glyph forms and reverses
// the string so it renders correctly left-to-right in standard TMP.

using System.Collections.Generic;
using System.Text;

public static class ArabicFixer
{
    static readonly Dictionary<char, char[]> ArabicMapper = new Dictionary<char, char[]>
    {
        // Each entry: isolated, initial, medial, final
        { 'ا', new[] { 'ﺍ', 'ﺍ', 'ﺎ', 'ﺎ' } },
        { 'أ', new[] { 'ﺃ', 'ﺃ', 'ﺄ', 'ﺄ' } },
        { 'إ', new[] { 'ﺇ', 'ﺇ', 'ﺈ', 'ﺈ' } },
        { 'آ', new[] { 'ﺁ', 'ﺁ', 'ﺂ', 'ﺂ' } },
        { 'ب', new[] { 'ﺏ', 'ﺑ', 'ﺒ', 'ﺐ' } },
        { 'ت', new[] { 'ﺕ', 'ﺗ', 'ﺘ', 'ﺖ' } },
        { 'ث', new[] { 'ﺙ', 'ﺛ', 'ﺜ', 'ﺚ' } },
        { 'ج', new[] { 'ﺝ', 'ﺟ', 'ﺠ', 'ﺞ' } },
        { 'ح', new[] { 'ﺡ', 'ﺣ', 'ﺤ', 'ﺢ' } },
        { 'خ', new[] { 'ﺥ', 'ﺧ', 'ﺨ', 'ﺦ' } },
        { 'د', new[] { 'ﺩ', 'ﺩ', 'ﺪ', 'ﺪ' } },
        { 'ذ', new[] { 'ﺫ', 'ﺫ', 'ﺬ', 'ﺬ' } },
        { 'ر', new[] { 'ﺭ', 'ﺭ', 'ﺮ', 'ﺮ' } },
        { 'ز', new[] { 'ﺯ', 'ﺯ', 'ﺰ', 'ﺰ' } },
        { 'س', new[] { 'ﺱ', 'ﺳ', 'ﺴ', 'ﺲ' } },
        { 'ش', new[] { 'ﺵ', 'ﺷ', 'ﺸ', 'ﺶ' } },
        { 'ص', new[] { 'ﺹ', 'ﺻ', 'ﺼ', 'ﺺ' } },
        { 'ض', new[] { 'ﺽ', 'ﺿ', 'ﻀ', 'ﺾ' } },
        { 'ط', new[] { 'ﻁ', 'ﻃ', 'ﻄ', 'ﻂ' } },
        { 'ظ', new[] { 'ﻅ', 'ﻇ', 'ﻈ', 'ﻆ' } },
        { 'ع', new[] { 'ﻉ', 'ﻋ', 'ﻌ', 'ﻊ' } },
        { 'غ', new[] { 'ﻍ', 'ﻏ', 'ﻐ', 'ﻎ' } },
        { 'ف', new[] { 'ﻑ', 'ﻓ', 'ﻔ', 'ﻒ' } },
        { 'ق', new[] { 'ﻕ', 'ﻗ', 'ﻘ', 'ﻖ' } },
        { 'ك', new[] { 'ﻙ', 'ﻛ', 'ﻜ', 'ﻚ' } },
        { 'ل', new[] { 'ﻝ', 'ﻟ', 'ﻠ', 'ﻞ' } },
        { 'م', new[] { 'ﻡ', 'ﻣ', 'ﻤ', 'ﻢ' } },
        { 'ن', new[] { 'ﻥ', 'ﻧ', 'ﻨ', 'ﻦ' } },
        { 'ه', new[] { 'ﻩ', 'ﻫ', 'ﻬ', 'ﻪ' } },
        { 'و', new[] { 'ﻭ', 'ﻭ', 'ﻮ', 'ﻮ' } },
        { 'ي', new[] { 'ﻱ', 'ﻳ', 'ﻴ', 'ﻲ' } },
        { 'ى', new[] { 'ﻯ', 'ﻯ', 'ﻰ', 'ﻰ' } },
        { 'ئ', new[] { 'ﺉ', 'ﺋ', 'ﺌ', 'ﺊ' } },
        { 'ء', new[] { 'ء', 'ء', 'ء', 'ء' } },
        { 'ؤ', new[] { 'ﺅ', 'ﺅ', 'ﺆ', 'ﺆ' } },
        { 'ة', new[] { 'ﺓ', 'ﺓ', 'ﺔ', 'ﺔ' } },
        { 'ل', new[] { 'ﻝ', 'ﻟ', 'ﻠ', 'ﻞ' } },
        { 'لا', new[] { 'ﻻ', 'ﻻ', 'ﻼ', 'ﻼ' } },
    };

    static readonly HashSet<char> RightConnectedLetters = new HashSet<char>
    {
        'ب', 'ت', 'ث', 'ج', 'ح', 'خ', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ',
        'ع', 'غ', 'ف', 'ق', 'ك', 'ل', 'م', 'ن', 'ه', 'ي', 'ئ',
    };

    static bool IsArabicLetter(char c)
    {
        return ArabicMapper.ContainsKey(c);
    }

    public static string Fix(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            char current = input[i];
            if (!IsArabicLetter(current))
            {
                result.Append(current);
                continue;
            }

            bool connectsLeft = i > 0 && RightConnectedLetters.Contains(input[i - 1]);
            bool connectsRight = i < input.Length - 1 && RightConnectedLetters.Contains(current);

            int form;
            if (!connectsLeft && !connectsRight) form = 0;      // isolated
            else if (connectsLeft && !connectsRight) form = 3;  // final
            else if (!connectsLeft && connectsRight) form = 1;  // initial
            else form = 2;                                      // medial

            if (ArabicMapper.TryGetValue(current, out char[] forms))
                result.Append(forms[form]);
            else
                result.Append(current);
        }

        // Reverse so the shaped glyphs render left-to-right in TMP.
        char[] chars = result.ToString().ToCharArray();
        System.Array.Reverse(chars);
        return new string(chars);
    }
}
