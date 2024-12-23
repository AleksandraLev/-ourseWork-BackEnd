using ProductsService.DTOs;
using ProductsService.Model;
using ProductsService.Services.Exceptions;
using ProductsService.Repositories;
using ProductsService.Repositories.Exceptions;
using ProductsService.Messaging;

namespace ProductsService.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductRepository _repository;
        private readonly ICategoryService _categoryService;
        private readonly IKafkaProduserService _kafkaProduserService;

        public ProductService(ILogger<ProductService> logger, IProductRepository repository, ICategoryService categoryService, IKafkaProduserService kafkaProduserService)
        {
            _logger = logger;
            _repository = repository;
            _categoryService = categoryService;
            _kafkaProduserService = kafkaProduserService;
        }
        public async Task CreateProductAsync(AddProductDTO addProductDTO)
        {
            //var _product = await _repository.SelectProductByNameAsync(addProductDTO.Name);
            //if (_product != null)
            //{
            //    throw new ProductExistsException(nameof(addProductDTO));
            //}
            Category category = await _categoryService.GetByNameAsync(addProductDTO.CategoryName);
            int categoryId;
            if (category == null)
            {
                _logger.LogWarning("Такой категории товаров не существует.");
                throw new CategoryNotFoundException(addProductDTO.CategoryName);
            }
            else
            {
                _logger.LogInformation($"Создание нового товара в категирии \"{category.Name}\".");
                categoryId = category.Id;
                var product = new Product
                {
                    Name = addProductDTO.Name,
                    Price = addProductDTO.Price,
                    Description = addProductDTO.Description,
                    CategoryId = categoryId,
                    StockQuantity = addProductDTO.StockQuantity,
                };
                var transation = await _repository.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation($"Сохранение товара \"{product.Name}\".");
                    await _repository.AddProductAsync(product);
                    await transation.CommitAsync();
                }
                catch (ProductSavedFaledException)
                {
                    _logger.LogError("Ошибка при сохранении товара");
                    await transation.RollbackAsync();
                    throw new ProductSavedFaledException();
                }
            }
        }
        public async Task<Product> GetByNameAsync(string ProductName)
        {
            _logger.LogInformation("Получение товара по названию.");
            return await _repository.SelectProductByNameAsync(ProductName);
        }
        public async Task<List<ProductInfoDTO>> GetProductsOfCategory(CategoryDTO categoryDTO)
        {
            if (await _categoryService.GetByNameAsync(categoryDTO.Name) == null)
            {
                _logger.LogError($"Категории {categoryDTO.Name} не существует.");
                throw new CategoryNotFoundException(categoryDTO.Name);
            }
            else
            {
                _logger.LogInformation("Получение всех товаров из категирии.");
                return await _repository.GetProductNamesByCategoryNameAsync(categoryDTO.Name);
            }
        }
        public async Task ChangeProductNameAsync(ChangeProductNameDTO changeProductNameDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeProductNameDTO.Id);
            if (product == null)
            {
                _logger.LogWarning($"Товар с артикулом {changeProductNameDTO.Id} не найден.");
                throw new ProductNotFoundException(changeProductNameDTO.Id.ToString());
            }
            else
            {
                if (product.Name == changeProductNameDTO.Name)
                {
                    _logger.LogWarning("Попытка изменить название товара то же самое.");
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        _logger.LogInformation("Изменение названия товара...");
                        product.Name = changeProductNameDTO.Name;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException ex)
                    {
                        _logger.LogError("Ошибка при при изменении названия товара:" + ex.Message);
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                    _logger.LogInformation("Название товара успешно изменено.");
                }
            }
        }
        public async Task ChangePriceAsync(ChangePriceDTO changePriceDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changePriceDTO.Id);
            if (product == null)
            {
                _logger.LogWarning($"Товар с артикулом {changePriceDTO.Id} не найден.");
                throw new ProductNotFoundException(changePriceDTO.Id.ToString());
            }
            else
            {
                if (product.Price == changePriceDTO.Price)
                {
                    _logger.LogWarning("Попытка изменить цену ту же самую.");
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        _logger.LogInformation("Изменение цены товара...");
                        product.Price = changePriceDTO.Price;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException ex)
                    {
                        _logger.LogError("Ошибка при при изменении цены товара:" + ex.Message);
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                    _logger.LogInformation("Цена товара успешно изменена.");
                }
            }
        }
        public async Task ChangeDescriptionAsync(ChangeDescriptionDTO changeDescriptionDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeDescriptionDTO.Id);
            if (product == null)
            {
                _logger.LogWarning($"Товар с артикулом {changeDescriptionDTO.Id} не найден.");
                throw new ProductNotFoundException(changeDescriptionDTO.Id.ToString());
            }
            else
            {
                if (product.Description == changeDescriptionDTO.Description)
                {
                    _logger.LogWarning("Попытка изменить описание то же самое.");
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        _logger.LogInformation("Изменение описания товара...");
                        product.Description = changeDescriptionDTO.Description;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException ex)
                    {
                        _logger.LogError("Ошибка при при изменении описания товара:" + ex.Message);
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                    _logger.LogInformation("Описание товара успешно изменено.");
                }
            }
        }
        public async Task ChangeCategoryOfProductAsync(ChangeCategoryOfProductDTO changeCategoryOfProductDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeCategoryOfProductDTO.Id);
            if (product == null)
            {
                _logger.LogWarning($"Товар с артикулом {changeCategoryOfProductDTO.Id} не найден.");
                throw new ProductNotFoundException(changeCategoryOfProductDTO.Id.ToString());
            }
            else
            {
                Category category = await _categoryService.GetByNameAsync(changeCategoryOfProductDTO.CategoryName);
                if (category == null)
                {
                    _logger.LogWarning("Такой категории не существует.");
                    throw new CategoryNotFoundException(changeCategoryOfProductDTO.CategoryName);
                }
                else
                {
                    if (product.CategoryId == category.Id)
                    {
                        _logger.LogWarning("Попытка категорию товара ту же самую.");
                        throw new ChangeToSameDataException();
                    }
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        _logger.LogInformation("Изменение категории товара...");
                        product.CategoryId = category.Id;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException ex)
                    {
                        _logger.LogError("Ошибка при при изменении категории товара:" + ex.Message);
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                    _logger.LogInformation("Категория товара успешно изменена.");
                }
            }
        }
        public async Task ChangeStockQuantityAsync(ChangeStockQuantityDTO changeStockQuantityDTO)
        {
            var product = await _repository.SelectProductByIdAsync(changeStockQuantityDTO.Id);
            if (product == null)
            {
                _logger.LogWarning($"Товар с артикулом {changeStockQuantityDTO.Id} не найден.");
                throw new ProductNotFoundException(changeStockQuantityDTO.Id.ToString());
            }
            else
            {
                if (product.StockQuantity == changeStockQuantityDTO.StockQuantity)
                {
                    _logger.LogWarning("Попытка количество товара то же самое.");
                    throw new ChangeToSameDataException();
                }
                else
                {
                    var transation = await _repository.BeginTransactionAsync();
                    try
                    {
                        _logger.LogInformation("Изменение количества товара на складе...");
                        product.StockQuantity = changeStockQuantityDTO.StockQuantity;
                        await _repository.SaveChangesAsync();
                        await transation.CommitAsync();
                    }
                    catch (ProductSavedFaledException ex)
                    {
                        _logger.LogError("Ошибка при при изменении количества товара на складе:" + ex.Message);
                        await transation.RollbackAsync();
                        throw new ProductSavedFaledException();
                    }
                    _logger.LogInformation("Количество товара на складе успешно изменено.");
                }
            }
        }
        public async Task<List<ProductInfoDTO>> GetAllProductNamesAsync()
        {
            try
            {
                _logger.LogInformation("Получение списка товаров.");
                List<ProductInfoDTO> productNames = await _repository.GetAllProductNamesAsync();
                if (productNames == null)
                {
                    _logger.LogWarning("Тoвары не найдены.");
                    throw new ProductNotFoundException("Тoвары не найдены.");
                }
                return productNames;
            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogError("Ошибка при получении списка товаров:" + ex.Message);
                throw new ProductNotFoundException("Ошибка при получении списка товаров.");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при получении списка товаров:" + ex.Message);
                throw new GetProductException("Ошибка при получении списка товаров.");
            }
        }
        public async Task<List<ProductInfoDTO>> GetProductsSelectedByPriseAsync(MinAmdMaxPrisesDTO prisesDTO)
        {
            try
            {
                _logger.LogInformation("Получение списка товаров.");
                List<ProductInfoDTO> productNames = await _repository.SelectProductsByPriceAsync(prisesDTO);
                if (productNames == null)
                {
                    _logger.LogWarning("Тoвары не найдены.");
                    throw new ProductNotFoundException("Тoвары не найдены.");
                }
                _logger.LogInformation("Список товаров получен.");
                return productNames;
            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogError("Ошибка при получении списка товаров:" + ex.Message);
                throw new ProductNotFoundException("Ошибка при получении списка товаров.");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при получении списка товаров:" + ex.Message);
                throw new GetProductException("Ошибка при получении списка товаров.");
            }
        }
        public async Task<Product> GetByIdAsync(int Id)
        {
            try
            {
                _logger.LogInformation($"Получение товара с артикулом {Id}.");
                Product product = await _repository.SelectProductByIdAsync(Id);
                if (product == null)
                {
                    _logger.LogWarning("Тoвар не найден.");
                    throw new ProductNotFoundException(Id.ToString());
                }
                _logger.LogInformation($"Товар с артикулом {Id} получен.");
                return product;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Ошибка при получении товара:" + ex.Message);
                throw new GetProductException("Ошибка при получении товара.");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Ошибка при получении товара:" + ex.Message);
                throw new GetProductException("Ошибка при получении товара.");
            }
        }
        public async Task CheckOrderInfoAsync(List<KafkaOrderItemDTO> kafkaOrderItemDTO)
        {
            _logger.LogInformation($"Проверка товаров в заказе на наличие.");
            /*KafkaProductDTO kafkaProductDTO = new KafkaProductDTO();
            if(kafkaOrderItemDTO != null)
            {
                var product = await _repository.SelectProductByIdAsync(kafkaOrderItemDTO.ProductId);
                if (product == null)
                {
                    kafkaProductDTO.ProductExist = false;
                    kafkaProductDTO.ProductId = kafkaOrderItemDTO.ProductId;
                    kafkaProductDTO.QuantityExist = false;
                    kafkaProductDTO.OrderItemId = kafkaOrderItemDTO.Id;
                    kafkaProductDTO.OrderId = kafkaOrderItemDTO.OrderId;
                }
                else
                {
                    kafkaProductDTO.ProductExist = true;
                    kafkaProductDTO.ProductId = kafkaOrderItemDTO.ProductId;
                }
                try
                {
                    product = await _repository.SelectProductByIdAsync(kafkaOrderItemDTO.ProductId);
                }
                catch (ArgumentNullException)
                {
                    kafkaProductDTO.ProductId = false;
                    //throw new GetProductException("Ошибка при получении товара.");
                }
                catch (OperationCanceledException)
                {
                    kafkaProductDTO.ProductId = false;
                    //throw new GetProductException("Ошибка при получении товара.");
                }
                if (product != null)
                {
                    if (kafkaOrderItemDTO.Quantity <= product.StockQuantity)
                    {
                        kafkaProductDTO.QuantityExist = true;
                        kafkaProductDTO.Quantity = kafkaOrderItemDTO.Quantity;
                        product.StockQuantity -= kafkaOrderItemDTO.Quantity;

                        kafkaProductDTO.OrderItemId = kafkaOrderItemDTO.Id;
                        kafkaProductDTO.OrderId = kafkaOrderItemDTO.OrderId;
                        kafkaProductDTO.Price = product.Price;

                        var transation = await _repository.BeginTransactionAsync();
                        try
                        {
                            await _repository.SaveChangesAsync();
                            await transation.CommitAsync();
                        }
                        catch (ProductSavedFaledException)
                        {
                            await transation.RollbackAsync();
                            throw new ProductSavedFaledException();
                        }
                    }
                    else
                    {
                        kafkaProductDTO.QuantityExist = false;
                    }
                }
                Console.WriteLine($"{kafkaProductDTO.OrderId}, {kafkaProductDTO.OrderItemId}, {kafkaProductDTO.ProductExist}");
                await _kafkaProduserService.SendMessageAsync("product-change", kafkaProductDTO);
            }*/
            /*if (kafkaOrderItemDTO == null || !kafkaOrderItemDTO.Any())
            {
                Console.WriteLine("Список товаров пуст.");
                return;
            }

            var kafkaProductDTOs = new List<KafkaProductDTO>();

            foreach (var item in kafkaOrderItemDTO)
            {
                KafkaProductDTO kafkaProductDTO = new KafkaProductDTO();
                var product = await _repository.SelectProductByIdAsync(item.ProductId);

                if (product == null)
                {
                    kafkaProductDTO.ProductExist = false;
                    kafkaProductDTO.ProductId = item.ProductId;
                    kafkaProductDTO.QuantityExist = false;
                    kafkaProductDTO.OrderItemId = item.Id;
                    kafkaProductDTO.OrderId = item.OrderId;
                }
                else
                {
                    kafkaProductDTO.ProductExist = true;
                    kafkaProductDTO.ProductId = item.ProductId;

                    if (item.Quantity <= product.StockQuantity)
                    {
                        kafkaProductDTO.QuantityExist = true;
                        kafkaProductDTO.Quantity = item.Quantity;
                        product.StockQuantity -= item.Quantity;

                        kafkaProductDTO.OrderItemId = item.Id;
                        kafkaProductDTO.OrderId = item.OrderId;
                        kafkaProductDTO.Price = product.Price;

                        var transaction = await _repository.BeginTransactionAsync();
                        try
                        {
                            await _repository.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (ProductSavedFaledException)
                        {
                            await transaction.RollbackAsync();
                            throw new ProductSavedFaledException();
                        }
                    }
                    else
                    {
                        kafkaProductDTO.QuantityExist = false;
                    }
                }

                kafkaProductDTOs.Add(kafkaProductDTO);
                Console.WriteLine($"Обработано: OrderId: {kafkaProductDTO.OrderId}, ProductId: {kafkaProductDTO.ProductId}, ProductExist: {kafkaProductDTO.ProductExist}, QuantityExist: {kafkaProductDTO.QuantityExist}");
            }*/
            if (kafkaOrderItemDTO == null || !kafkaOrderItemDTO.Any())
            {
                _logger.LogWarning("Список товаров пуст.");
                return;
            }

            var kafkaProductDTOs = new List<KafkaProductDTO>();
            bool allProductsValid = true; // Флаг для проверки, все ли товары корректны

            foreach (var item in kafkaOrderItemDTO)
            {
                KafkaProductDTO kafkaProductDTO = new KafkaProductDTO();
                var product = await _repository.SelectProductByIdAsync(item.ProductId);

                if (product == null)
                {
                    // Если товар не найден, то не меняем количество и добавляем в DTO информацию о товаре

                    _logger.LogWarning("Запрашиваемый товар из заказа не найден.");

                    kafkaProductDTO.ProductExist = false;
                    kafkaProductDTO.ProductId = item.ProductId;
                    kafkaProductDTO.QuantityExist = false;
                    kafkaProductDTO.OrderItemId = item.Id;
                    kafkaProductDTO.OrderId = item.OrderId;

                    allProductsValid = false; // Один из товаров не найден, помечаем заказ как некорректный
                }
                else
                {
                    kafkaProductDTO.OrderId = item.OrderId;
                    kafkaProductDTO.ProductExist = true;
                    kafkaProductDTO.ProductId = item.ProductId;

                    if (item.Quantity <= product.StockQuantity)
                    {
                        // Если количество товара на складе достаточное, обновляем информацию в DTO
                        _logger.LogInformation($"Товар с артикулом {kafkaProductDTO.ProductId} существует, и его количества на складе достаточкно, чтобы выполнить заказ.");
                        kafkaProductDTO.QuantityExist = true;
                        kafkaProductDTO.Quantity = item.Quantity;
                        kafkaProductDTO.Price = product.Price;
                    }
                    else
                    {
                        // Если товара недостаточно, помечаем его как отсутствующий

                        _logger.LogWarning($"Запрашиваемое колличество товара с атикулом {kafkaProductDTO.ProductId} из заказа отсутствует на складе.");

                        kafkaProductDTO.QuantityExist = false;
                    }

                    // Уменьшаем количество товара на складе только после всех проверок
                    if (kafkaProductDTO.QuantityExist)
                    {
                        _logger.LogInformation("Уменьшаем количество товара на складе после всех проверок.");
                        product.StockQuantity -= item.Quantity;
                    }
                }

                kafkaProductDTOs.Add(kafkaProductDTO);

                // Логируем информацию для отладки
                _logger.LogInformation($"Обработано: OrderId: {kafkaProductDTO.OrderId}, ProductId: {kafkaProductDTO.ProductId}, ProductExist: {kafkaProductDTO.ProductExist}, QuantityExist: {kafkaProductDTO.QuantityExist}");
            }

            // Если все товары прошли проверку (все валидны), то сохраняем изменения в базе данных
            if (allProductsValid)
            {
                var transaction = await _repository.BeginTransactionAsync();
                try
                {
                    // Сохраняем изменения в базе данных
                    _logger.LogInformation("Сохраняем изменения в базе данных.");
                    await _repository.SaveChangesAsync();
                    await transaction.CommitAsync(); // Подтверждаем транзакцию
                }
                catch (ProductSavedFaledException)
                {
                    _logger.LogError("Ошибка при сохранении изменений в базе дпнных.");
                    await transaction.RollbackAsync();
                    throw new ProductSavedFaledException();
                }
                _logger.LogInformation("Данные изменены.");
            }
            _logger.LogInformation("Отправка сообщения с обработанными данными с topic \"product-change\".");
            await _kafkaProduserService.SendMessageAsync("product-change", kafkaProductDTOs);
        }
    }
}
