using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


#region Soru2
/*
 SORU 2
Bir fiş tarama sistemi geliştirilecektir. OCR aşaması için SaaS bir sistem kullanılmaktadır. 
Sistem Tanımı: Her fiş görseli için SaaS hizmetinden bir adet json response dönülmektedir. Json 
response içerisinde description ve ilgili description için koordinat bilgileri yer almaktadır. Amaç 
json’ın uygun şekilde parse edilerek fişe ait text’in görselde görünen haline en yakında halde 
kaydedilmesidir. Buna uygun olarak gerekli C# kodunu yazınız. Lütfen dikkat! Sizden OCR işlemi için bir 
çalışma yapmanız beklenmemektedir. Sadece verilen JSON’ın uygun şekilde parse edilmesi yeterlidir.

*/

#endregion Soru2
public class ReceiptItem
{
    public string Description { get; set; }
    public BoundingPoly BoundingPoly { get; set; }
}

public class BoundingPoly
{
    public List<Vertex> Vertices { get; set; }
}

public class Vertex
{
    public int X { get; set; }
    public int Y { get; set; }
}

class Program
{
    static async Task Main()
    {
        // JSON dosyasının içeriğini URL'den okuması için ReadJsonFromUrlAsync yazıldı.
        string json = await ReadJsonFromUrlAsync("https://drive.google.com/uc?id=1J_S-JZ7RCk9qHXAzVm_Y-OUoxe5D9MnR");

        // JSON'daki öğeleri ReceiptItem tipine çevir.
        List<ReceiptItem> receiptItems = JsonConvert.DeserializeObject<List<ReceiptItem>>(json);

        receiptItems.RemoveAt(0); // İlk elemanda toplu yazıyor, istenen çıktıyı elde etmek için çıkarttım.

        // Öğeleri Y koordinatına göre grupla ve sırala, çünkü satırları dikey eksende bulmak gerekiyor.
        var lines = GroupItemsByLines(receiptItems);

        // Her satırı X koordinatına göre sırala ve ekrana yazdır
        foreach (var line in lines)
        {
            var sortedLine = line.OrderBy(item => item.BoundingPoly.Vertices.Min(v => v.X)).ToList();
            Console.WriteLine(string.Join(" ", sortedLine.Select(item => item.Description)));
        }
    }

    // URL'den JSON okuma, klasik bir client kodu kullanıldı.
    static async Task<string> ReadJsonFromUrlAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"Failed to retrieve data from {url}. Status code: {response.StatusCode}");
            }
        }
    }

    static List<List<ReceiptItem>> GroupItemsByLines(List<ReceiptItem> items)
    {
        var lines = new List<List<ReceiptItem>>();
        var sortedItems = items.OrderBy(item => item.BoundingPoly.Vertices.Min(v => v.Y)).ToList();

        List<ReceiptItem> currentLine = new List<ReceiptItem>();
        int? currentLineY = null;

        // Otomatik olarak eşik değeri belirlemek gerekmekte, genel ortalamayı item sayısına bölerek.
        // Belirlenen treshold değerinin altını veyahut üstününün kullanılması durumunda istenen çıktı elde edilemez.
        int threshold = CalculateThreshold(sortedItems);

        foreach (var item in sortedItems)
        {
            int itemY = item.BoundingPoly.Vertices.Min(v => v.Y);

            // Eğer bu ilk öğe ise veya öğe eşik değerinin altındaysa yeni bir satır başlatılır.
            if (currentLineY == null || Math.Abs(itemY - currentLineY.Value) > threshold)
            {
                if (currentLine.Any())
                {
                    lines.Add(currentLine);
                    currentLine = new List<ReceiptItem>();
                }
                currentLineY = itemY;
            }

            currentLine.Add(item);
        }

        // Eğer son satırda öğe varsa eklenir.
        if (currentLine.Any())
        {
            lines.Add(currentLine);
        }

        return lines;
    }

    static int CalculateThreshold(List<ReceiptItem> items)
    {
        int totalThreshold = 0;
        int totalCount = items.Count - 1;

        for (int i = 0; i < totalCount; i++)
        {
            // bir sonraki itemin y değerinin en düşüğünden (dikey değer) mevcut itemin y değerinin en düşüğünü çıkarıp mutlak değerini alıyoruz.
            int y = Math.Abs(items[i + 1].BoundingPoly.Vertices.Min(v => v.Y) - items[i].BoundingPoly.Vertices.Min(v => v.Y));
            // toplam eşik değerine ekliyoruz.
            totalThreshold += y;
        }

        // Ortalama eşik değerini hesaplamak için elde ettiğimiz total eşik değerini item sayısına bölüyoruz.
        int averageThreshold = totalThreshold / totalCount;

        return averageThreshold;
    }
}
