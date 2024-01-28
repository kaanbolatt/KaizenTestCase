using System;

#region Soru1
/*
SORU 1
Hızlı tüketim sektöründe faaliyet gösteren bir gıda firması, ürün ambalajları içerisine kod 
yerleştirerek, bu kodlar aracılığı ile çeşitli kampanyalar düzenlemek istemektedir. Proje aşağıda 
kısaca özetlenmiştir.
1. Firma aşağıdaki özelliklere sahip kodlar üretilmesini talep etmektedir. 
• Kodlar 8 hane uzunluğunda ve unique olmalıdır.
• Kodlar ACDEFGHKLMNPRTXYZ234579 karakter kümesini içermelidir.
2. Kullanıcılar kampanya dönemi içerisinde çeşitli kanallar üzerinden ellerindeki kodları 
kullanarak kampanyalara katılabilecektir.
BEKLENENLER
• Kod üretimi belirlediğiniz bir algoritmaya uygun olarak yapılmalıdır. 
• Kod geçerliliği veritabanı, array, hash table, file, redis, elastic gibi bir saklama 
ortamından kontrol edilmemeli sadece algoritmik olarak yapılmalıdır.
• Kodların tahmin edilerek sistemin manipüle edilme olasılığı yeterince düşük olmalıdır. 
• T-SQL ya da C# kullanabilirsiniz. Aşağıda T-SQL için procedure örnekleri verilmiştir.

*/

#endregion Soru1


// bellekte ardışık bölgelere yerleştirmemek ve her elemanın kendi bellek bölgesi olması için kullanıldı
// hash tree, elastic search vs. kullanılması istenmemişti.
struct CodeData
{
    public string[] GeneratedCodes;
}

class Program
{
    static void Main()
    {
        int codeLength = 8; // istenen hane sayısı 8 olduğu için 8 verildi.
        string characterSet = "ACDEFGHKLMNPRTXYZ234579"; // istenen karakter seti
        bool flag = false; // yanlış girilirse döngü devam etmesi için flag yazıldı.

        #region Kod üretimi
        CodeData codeData = GenerateUniqueCodes(codeLength, characterSet, 100); // 100 adet random ve unique kod oluşturuldu ve struct CodeData'ya atandı.

        Console.WriteLine("Üretilen Kodlar:");
        foreach (var code in codeData.GeneratedCodes)
        {
            Console.WriteLine(code);
        }
        #endregion Kod üretimi

        // Kullanıcı doğru giriş yapana kadar döngüye sokması için while kullanıldı.
        while (!flag)
        {
            Console.Write("Lütfen bir kod giriniz: ");
            string userInput = Console.ReadLine(); // kullanıcıdan kampanya kodu alındı.

            if (IsValidCode(userInput, codeData.GeneratedCodes)) // isValidCode ile kodun struct CodeData'da olup olmadığı kontrol edildi.
            {
                Console.WriteLine("Kod geçerli. Kullanıcı kampanyaya katılabilir.");
                flag = true; // kod doğru ise while döngüsünden çıkmak için true'ya çekildi.
            }
            else
            {
                Console.WriteLine("Geçersiz kod. Lütfen doğru bir kod giriniz.");
            }
        }
    }

    static CodeData GenerateUniqueCodes(int codeLength, string characterSet, int count)
    {
        CodeData codeData = new CodeData();
        codeData.GeneratedCodes = new string[count]; // istenilen sayı kadar string oluşturuldu.

        Random random = new Random();

        // istenilen sayı kadar unique kod oluşturulup indexlere atandı.
        for (int i = 0; i < count; i++)
        {
            char[] codeArray = new char[codeLength];
            for (int j = 0; j < codeLength; j++)
            {
                codeArray[j] = characterSet[random.Next(characterSet.Length)];
            }

            codeData.GeneratedCodes[i] = new string(codeArray);
        }

        return codeData;
    }

    static bool IsValidCode(string userInput, string[] validCodes)
    {
        // kullanıcının girdiği kod, structa tutulan değerlerden herhangi biri ile eşleşmesi durumunda true vermesi için
        // kod kontrolü fonksiyonu yazıldı.
        foreach (var validCode in validCodes)
        {
            if (userInput == validCode)
            {
                return true;
            }
        }
        return false;
    }
}
