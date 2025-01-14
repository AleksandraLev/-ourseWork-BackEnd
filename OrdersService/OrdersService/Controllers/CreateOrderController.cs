﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersService.DTOs;
using OrdersService.Repositories.Exceptions;
using OrdersService.Services;
using OrdersService.Services.Exceptions;

namespace OrdersService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CreateOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public CreateOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Переданы некорректные данные.");
            }
            try
            {
                if (createOrderDTO.Items == null || !createOrderDTO.Items.Any())
                {
                    return BadRequest("Список товаров пуст или не указан.");
                }
                await _orderService.CreateOrderAsync(createOrderDTO);
                return Ok("Заказ создан и обрабатывается!");
            }
            catch (OrderSavedFailedException)
            {
                return BadRequest("Ошибка при создании заказа.");
            }
        }
    }
}
