string BinaryToHex(string binary)
{
    string hex = "";
    for (int i = 0; i < binary.Length; i += 4)
    {
        string fourBits = binary.Substring(i, 4);
        hex += Convert.ToInt32(fourBits, 2).ToString("X");
    }
    return hex;
}

/// 100001011011111010101010101111001001111100100110 = 85BEAABC9F26
