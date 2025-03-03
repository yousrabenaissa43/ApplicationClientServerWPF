
namespace chiffrement_C_sar
{
    public static class Chiffrement_C_sar
    {
        public static char RotChar(char c, int key)
        {


            if (c >= '0' && c <= '9')
            {
                key = key % 10;
                int b = c + key;
                if (b > '9')
                    b -= 10;
                else if (b < '0')
                    b += 10;
                return (char)b;
            }


            // return (char)(c + key) ; 

            if (c >= 'A' && c <= 'Z')
            {
                key = key % 26;
                int b = c + key;
                if (b > 'Z')
                    b -= 26;
                else if (b < 'A')
                    b += 26;
                return (char)b;
            }
            if (c >= 'a' && c <= 'z')
            {
                key = key % 26;
                int b = c + key;
                if (b > 'z')
                    b -= 26;
                else if (b < 'a')
                    b += 26;
                return (char)b;
            }

            return c;
        }

        public static string RotString(string text, int key)
        {
            string cesarText = "";
            foreach (char c in text)
            {
                cesarText += RotChar(c, key);
            }
            return cesarText;

        }
    }

}