п»їusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Domain.Entities
{
    public class RestaurantDailyStat
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }        // Р’СЃСЊРѕРіРѕ
        public int SuccessfulOrders { get; set; }   // Р’РёРєРѕРЅР°РЅРёС…
        public int CancelledOrders { get; set; }    // РЎРєР°СЃРѕРІР°РЅРёС…

        // --- Р‘Р»РѕРє 2: Р¤С–РЅР°РЅСЃРё ---
        public decimal TotalRevenue { get; set; }   // Р’РёСЂСѓС‡РєР° Р·Р° РґРµРЅСЊ

        // --- Р‘Р»РѕРє 3: Р•С„РµРєС‚РёРІРЅС–СЃС‚СЊ (РњР°С‚РµРјР°С‚РёРєР°) ---
        // РЎРµСЂРµРґРЅС–Р№ С‡Р°СЃ РѕС‡С–РєСѓРІР°РЅРЅСЏ (РЅР°РїСЂРёРєР»Р°Рґ, 18.5 С…РІ)
        public double AverageCookingTimeMinutes { get; set; }

        // Р’С–РґСЃРѕС‚РѕРє Р·Р°РІР°РЅС‚Р°Р¶РµРЅРѕСЃС‚С– РєСѓС…РЅС– (Load Factor)
        public double LoadFactorPercent { get; set; }

        // --- Р‘Р»РѕРє 4: РџС–РєРѕРІРµ РЅР°РІР°РЅС‚Р°Р¶РµРЅРЅСЏ ---
        // Р“РѕРґРёРЅР° РїС–Рє (РЅР°РїСЂРёРєР»Р°Рґ, 13 РѕР·РЅР°С‡Р°С” 13:00-14:00)
        public int PeakHour { get; set; }
        // РњР°РєСЃРёРјР°Р»СЊРЅР° РєС–Р»СЊРєС–СЃС‚СЊ Р·Р°РјРѕРІР»РµРЅСЊ Р·Р° С†СЋ РіРѕРґРёРЅСѓ
        public int PeakLoadCount { get; set; }
    }
}
