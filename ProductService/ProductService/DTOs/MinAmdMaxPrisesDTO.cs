namespace ProductsService.DTOs
{
    public class MinAmdMaxPrisesDTO
    {
        private decimal minPrice;
        private decimal maxPrice;
        public decimal MinPrice
        {
            get => Math.Round(minPrice, 2);
            set => minPrice = value;
        }
        public decimal MaxPrice
        {
            get => Math.Round(maxPrice, 2);
            set => maxPrice = value;
        }
    }
}
