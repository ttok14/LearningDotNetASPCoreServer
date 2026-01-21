namespace LearningServer01
{
    public class Temp_StructureCost
    {
        public enum CurrencyType
        {
            Gold,
            Wood,
            Food
        }

        public class CostInfo
        {
            public CurrencyType Type;
            public int Price;
        }

        public readonly static Dictionary<int, CostInfo> CostContainer = new Dictionary<int, CostInfo>()
        {
            [360] = new CostInfo() { Type = CurrencyType.Gold, Price = 20 },
            [356] = new CostInfo() { Type = CurrencyType.Food, Price = 100 },
            [357] = new CostInfo() { Type = CurrencyType.Food, Price = 100 },
            [358] = new CostInfo() { Type = CurrencyType.Food, Price = 100 },
            [371] = new CostInfo() { Type = CurrencyType.Gold, Price = 40 },
            [374] = new CostInfo() { Type = CurrencyType.Gold, Price = 100 },
            [375] = new CostInfo() { Type = CurrencyType.Gold, Price = 200 },
            [376] = new CostInfo() { Type = CurrencyType.Gold, Price = 300 },
            [377] = new CostInfo() { Type = CurrencyType.Gold, Price = 300 },
            [378] = new CostInfo() { Type = CurrencyType.Gold, Price = 800 },
            [379] = new CostInfo() { Type = CurrencyType.Gold, Price = 100 },
            [380] = new CostInfo() { Type = CurrencyType.Gold, Price = 200 },
            [381] = new CostInfo() { Type = CurrencyType.Gold, Price = 300 },
            [382] = new CostInfo() { Type = CurrencyType.Gold, Price = 100 },
            [383] = new CostInfo() { Type = CurrencyType.Gold, Price = 200 },
            [384] = new CostInfo() { Type = CurrencyType.Gold, Price = 300 },
            [111] = new CostInfo() { Type = CurrencyType.Gold, Price = 1000 },
            [109] = new CostInfo() { Type = CurrencyType.Gold, Price = 500 },
            [113] = new CostInfo() { Type = CurrencyType.Gold, Price = 500 },
        };
    }
}
