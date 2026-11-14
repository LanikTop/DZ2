using Application.DTOs;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductRequestDTO> _createValidator;
        private readonly IValidator<GetProductsRequestDTO> _getValidator;
        private readonly IValidator<UpdateProductRequestDTO> _updateValidator;

        public ProductController(IProductService productService,
            IValidator<CreateProductRequestDTO> createValidator,
            IValidator<GetProductsRequestDTO> getValidator,
            IValidator<UpdateProductRequestDTO> updateValidator)
        {
            _productService = productService;
            _createValidator = createValidator;
            _getValidator = getValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(Guid productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound(new { Message = $"Товар с ID {productId} не найден." });

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<CreateProductResponseDTO>> CreateProduct([FromBody] CreateProductRequestDTO request)
        {
            var validationResult = await _createValidator.ValidateAsync(
                request,
                HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                AddValidationErrors(validationResult);
                return ValidationProblem(ModelState);
            }

            var response = await _productService.CreateProductAsync(request);

            return CreatedAtAction(nameof(GetProductById), new { productId = response.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<GetProductsResponseDTO>> GetProducts([FromQuery] GetProductsRequestDTO request)
        {
            var validationResult = await _getValidator.ValidateAsync(
                request,
                HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                AddValidationErrors(validationResult);
                return ValidationProblem(ModelState);
            }

            var response = await _productService.GetProductsAsync(request);

            return Ok(response);
        }

        [HttpPatch]
        [Route("update")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct([FromBody] UpdateProductRequestDTO request)
        {
            var validationResult = await _updateValidator.ValidateAsync(
                request,
                HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                AddValidationErrors(validationResult);
                return ValidationProblem(ModelState);
            }

            var response = await _productService.UpdateProductAsync(request);

            if (response == null)
                return NotFound(new { Message = $"Товар с ID {request.Id} не найден." });

            return Ok(response);
        }

        private void AddValidationErrors(FluentValidation.Results.ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}