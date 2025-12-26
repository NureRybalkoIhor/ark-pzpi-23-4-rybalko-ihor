using FoodPreOrder.Api.Services;
using FoodPreOrder.Domain.Entities;
using Xunit;

namespace FoodPreOrder.Tests
{
    public class CalculationServiceTests
    {
        [Fact]
        public void CalculateEstimatedReadyTime_ShouldReturnCorrectTime()
        {
            var service = new CalculationService();
            var orderTime = new DateTime(2025, 12, 20, 12, 0, 0);

            var dishes = new List<Dish>
            {
                new Dish { NameUA = "Р В Р’В Р вЂ™Р’В Р В Р Р‹Р РЋРІвЂћСћР В Р’В Р В Р вЂ№Р В Р вЂ Р В РІР‚С™Р Р†Р вЂљРЎС™Р В Р’В Р вЂ™Р’В Р В Р’В Р Р†Р вЂљР’В¦Р В Р’В Р В Р вЂ№Р В Р вЂ Р В РІР‚С™Р Р†Р вЂљРЎС™Р В Р’В Р В Р вЂ№Р В Р’В Р РЋРІР‚СљР В Р’В Р В Р вЂ№Р В Р вЂ Р В РІР‚С™Р РЋРІвЂћСћР В Р’В Р В Р вЂ№Р В Р’В Р Р†Р вЂљРЎв„ўР В Р’В Р вЂ™Р’В Р В Р Р‹Р Р†Р вЂљРЎС›Р В Р’В Р вЂ™Р’В Р В Р’В Р Р†Р вЂљР’В¦Р В Р’В Р вЂ™Р’В Р В РІР‚в„ўР вЂ™Р’Вµ", PreparationTimeMinutes = 20 }, 
                new Dish { NameUA = "Р В Р’В Р вЂ™Р’В Р В Р Р‹Р Р†РІР‚С›РЎС›Р В Р’В Р вЂ™Р’В Р В РІР‚в„ўР вЂ™Р’В°Р В Р’В Р вЂ™Р’В Р В Р’В Р Р†Р вЂљР’В Р В Р’В Р вЂ™Р’В Р В РІР‚в„ўР вЂ™Р’В°", PreparationTimeMinutes = 5 }
            };

            var expectedTime = orderTime.AddMinutes(25);

            var result = service.CalculateEstimatedReadyTime(orderTime, dishes);

            Assert.Equal(expectedTime, result);
        }

        [Fact]
        public void CalculateDistance_ShouldReturnDistance_BetweenKharkivAndKyiv()
        {
            var service = new CalculationService();

            double kharkivLat = 50.0;
            double kharkivLon = 36.2;
            double kyivLat = 50.4;
            double kyivLon = 30.5;

            double distance = service.CalculateDistance(kharkivLat, kharkivLon, kyivLat, kyivLon);

            Assert.True(distance > 0);
            Assert.InRange(distance, 400, 500);
        }
    }
}
