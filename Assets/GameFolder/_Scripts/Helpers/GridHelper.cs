using UnityEngine;
using UnityEngine.UI;
using System; // Math.Sqrt için

namespace SKC.Helper
{
    public static class GridHelper
    {
        public static Tuple<int, int> CalculateOptimalGrid(int totalCards, RectTransform containerRect, float targetCardAspectRatio)
        {
            float containerAspectRatio = containerRect.rect.width / containerRect.rect.height;
            
            // Yaklaşık olarak karekökünü alarak başlangıç satır ve sütun sayısını tahmin et
            int bestRows = 1;
            int bestColumns = totalCards;
            float bestDeviation = float.MaxValue; // En iyi sapmayı bulmak için

            // Olası tüm satır/sütun kombinasyonlarını dene
            for (int r = 1; r <= totalCards; r++)
            {
                if (totalCards % r == 0) // r satır olabilirse
                {
                    int c = totalCards / r; // c sütun sayısı
                    
                    // Bu r ve c kombinasyonu için her kartın genişlik/yükseklik oranını hesapla
                    // Kapsayıcının (container) içindeki kullanılabilir alanın oranını hesaplamamız gerekiyor
                    // Ancak burada henüz spacing ve padding yok. Genel bir yaklaşımla başlayabiliriz.
                    
                    // Şimdilik sadece ana kapsayıcı oranına göre yaklaşık bir hücre oranı hesaplayalım
                    // Bu çok doğru bir yaklaşım değil, çünkü spacing ve padding'i dahil etmiyor.
                    // Daha doğru bir hesaplama için aşağıdaki SetGridLayout metoduna bak.

                    // Yeni yaklaşım: Optimal 'columns' sayısını bulmaya çalışmak
                    // Bu algoritmik bir problemdir ve farklı çözümleri olabilir.
                    // Buradaki amaç, her bir hücrenin mümkün olduğunca 'targetCardAspectRatio'ya yakın olmasını sağlamaktır.
                    
                    // Daha pratik bir yaklaşım, 'columns' sayısını önceden belirlemek ve 'rows'u buna göre ayarlamak olacaktır.
                    // Ya da kullanıcının seçmesine izin vermek.

                    // Ancak "optimal" sütun sayısını otomatik bulmak için:
                    // Genellikle, yatay bir ekranda, sütun sayısı satır sayısından fazla olur (veya eşit).
                    // Dikey bir ekranda ise satır sayısı sütun sayısından fazla olur.

                    // Burada, containerRect'in oranını kullanarak optimal sütun sayısını bulmaya çalışabiliriz.
                    // Bu problem, bir alana belirli sayıda öğeyi, her öğenin belirli bir oranını koruyarak sığdırma problemidir.
                    // Genellikle en iyi çözüm, olası sütun sayılarını (1'den totalCards'a kadar) denemek ve
                    // her durum için kart boyutunu ve buna bağlı olarak oluşan boşluk miktarını değerlendirmektir.

                    // Basit bir heuristic: Container'ın en boy oranına en yakın gridi bulmak
                    float currentGridAspectRatio = (float)c / r; // GridLayout'ın genel oranı (sütun/satır)

                    // Bu oran, container'ın oranına ne kadar yakınsa o kadar iyi.
                    float deviation = Mathf.Abs(containerAspectRatio - currentGridAspectRatio);

                    if (deviation < bestDeviation)
                    {
                        bestDeviation = deviation;
                        bestRows = r;
                        bestColumns = c;
                    }
                }
            }
            return new Tuple<int, int>(bestRows, bestColumns);
        }

        public static void SetGridLayout(int newRows, int newColumns, float cardSpacing, GridLayoutGroup gridLayoutGroup,
            RectTransform cardGridParentRect, float targetCardAspectRatio) // targetCardAspectRatio eklendi
        {
            int rows = newRows;
            int columns = newColumns;

            float parentWidth = cardGridParentRect.rect.width;
            float parentHeight = cardGridParentRect.rect.height;

            float paddingLeft = gridLayoutGroup.padding.left;
            float paddingRight = gridLayoutGroup.padding.right;
            float paddingTop = gridLayoutGroup.padding.top;
            float paddingBottom = gridLayoutGroup.padding.bottom;

            float availableWidth = parentWidth - paddingLeft - paddingRight;
            float availableHeight = parentHeight - paddingTop - paddingBottom;

            // Her bir hücre için ayrılan maksimum genişlik ve yükseklik
            // Kartlar arasındaki boşlukları çıkarıyoruz
            float maxCellWidth = (availableWidth - (columns - 1) * cardSpacing) / columns;
            float maxCellHeight = (availableHeight - (rows - 1) * cardSpacing) / rows;

            float calculatedCellWidth;
            float calculatedCellHeight;

            // En boy oranını koruyarak kartları sığdırma
            // İki olasılık var: Genişliğe göre mi sınırlıyız, yüksekliğe göre mi?
            // Kartın genişliği = Yüksekliği * targetCardAspectRatio
            // Kartın yüksekliği = Genişliği / targetCardAspectRatio

            float heightIfFixedWidth = maxCellWidth / targetCardAspectRatio; // Genişlik tam doldurulursa yükseklik ne olur?
            float widthIfFixedHeight = maxCellHeight * targetCardAspectRatio; // Yükseklik tam doldurulursa genişlik ne olur?

            // Hangi eksende daha kısıtlı olduğumuzu bulalım.
            // Genişlik tam sığarsa ve yüksekliği de sığarsa, genişliği kullan.
            if (heightIfFixedWidth <= maxCellHeight)
            {
                calculatedCellWidth = maxCellWidth;
                calculatedCellHeight = heightIfFixedWidth;
            }
            else // Genişlik tam sığarken yükseklik aşarsa, yüksekliği tam doldur ve genişliği ona göre küçült
            {
                calculatedCellHeight = maxCellHeight;
                calculatedCellWidth = widthIfFixedHeight;
            }
            
            // Eğer kartlar tam ortada değilse, GridLayoutGroup'un Child Alignment'ını MidleCenter yapın.
            gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter; // Burayı koddan set edelim ki emin olalım.

            gridLayoutGroup.cellSize = new Vector2(calculatedCellWidth, calculatedCellHeight);
            gridLayoutGroup.spacing = new Vector2(cardSpacing, cardSpacing);
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;

            Debug.Log($"Calculated Cell Size: {gridLayoutGroup.cellSize.x}x{gridLayoutGroup.cellSize.y}");
            Debug.Log($"Target Card Aspect Ratio: {targetCardAspectRatio}");
            Debug.Log($"Calculated Rows: {rows}, Columns: {columns}");
        }
    }
}