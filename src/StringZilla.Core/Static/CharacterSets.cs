namespace StringZilla.Core.Static
{
    public static class CharacterSets
    {
        public static ReadOnlySpan<byte> AsciiLetters => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"u8;
        public static ReadOnlySpan<byte> AsciiLowercaseLetters => "abcdefghijklmnopqrstuvwxyz"u8;
        public static ReadOnlySpan<byte> AsciiUppercaseLetters => "ABCDEFGHIJKLMNOPQRSTUVWXYZ"u8;
        public static ReadOnlySpan<byte> PrintableAscii => "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~ \t\n\r\f\v"u8;
        public static ReadOnlySpan<byte> AsciiControlCharacters =>
        [
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 127,
        ];
        public static ReadOnlySpan<byte> AsciiDigits => "0123456789"u8;
        public static ReadOnlySpan<byte> AsciiHexDigits => "0123456789abcdefABCDEF"u8;
        public static ReadOnlySpan<byte> OctalDigits => "01234567"u8;
        public static ReadOnlySpan<byte> Punctuation => "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"u8;
        public static ReadOnlySpan<byte> Whitespaces => " \t\n\r\f\v"u8;
        public static ReadOnlySpan<byte> NewLines => "\n\r\f\v\x1C\x1D\x1E\x85"u8;
        public static ReadOnlySpan<byte> Base64Characters => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"u8;
    }
}