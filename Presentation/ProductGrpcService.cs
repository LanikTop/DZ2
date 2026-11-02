using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using FluentValidation;
using Application.DTOs;
using Presentation.Protos;

namespace Application.Services;

public class ProductGrpcService : Presentation.Protos.ProductService.ProductServiceBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductRequestDTO> _createValidator;
    private readonly IValidator<GetProductsRequestDTO> _getValidator;

    public ProductGrpcService(
        IProductService productService,
        IValidator<CreateProductRequestDTO> createValidator,
        IValidator<GetProductsRequestDTO> getValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _getValidator = getValidator;
    }

    public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request,
        ServerCallContext context)
    {
        var requestDTO = new CreateProductRequestDTO
        {
            Name = request.Name,
            Price = (decimal)request.Price,
            Weight = (decimal)request.Weight,
            Type = ToDomainType(request.Type),
            WarehouseNumber = request.WarehouseNumber
        };
        ThrowIfInvalid(await _createValidator.ValidateAsync(requestDTO));

        var product = await _productService.CreateProductAsync(requestDTO);

        return new CreateProductResponse { Id = product.Id.ToString() };
    }

    public override async Task<GetProductsResponse> GetProducts(GetProductsRequest request,
        ServerCallContext context)
    {
        var requestDTO = new GetProductsRequestDTO
        {
            CreatedAt = request.CreatedAt?.ToDateTimeOffset(),
            Type = request.HasType ? ToDomainType(request.Type) : null,
            WarehouseNumber = request.HasWarehouseNumber ? request.WarehouseNumber : null,
            Page = request.Page,
            PageSize = request.PageSize
        };

        ThrowIfInvalid(await _getValidator.ValidateAsync(requestDTO));

        var result = await _productService.GetProductsAsync(requestDTO);

        var response = new GetProductsResponse { TotalCount = result.TotalCount };
        response.Products.AddRange(result.Products.Select(ToProto));

        return response;
    }

    public override async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Некорректный формат ID."));

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Товар с ID {id} не найден."));

        return new GetProductByIdResponse { Product = ToProto(product) };
    }

    private static ProductResponse ToProto(ProductDTO product) => new ProductResponse()
    {
        Id = product.Id.ToString(),
        Name = product.Name,
        Price = (double)product.Price,
        Weight = (double)product.Weight,
        Type = ToProtoType(product.Type),
        CreatedAt = Timestamp.FromDateTimeOffset(product.CreatedAt),
        WarehouseNumber = product.WarehouseNumber
    };

    private static Domain.Models.ProductType ToDomainType(Presentation.Protos.ProductType type) => type switch
    {
        Presentation.Protos.ProductType.HouseholdChemicals => Domain.Models.ProductType.HouseholdChemicals,
        Presentation.Protos.ProductType.Electronics => Domain.Models.ProductType.Electronics,
        Presentation.Protos.ProductType.Food => Domain.Models.ProductType.Food,
        _ => Domain.Models.ProductType.General
    };

    private static Presentation.Protos.ProductType ToProtoType(Domain.Models.ProductType type) => type switch
    {
        Domain.Models.ProductType.HouseholdChemicals => Presentation.Protos.ProductType.HouseholdChemicals,
        Domain.Models.ProductType.Electronics => Presentation.Protos.ProductType.Electronics,
        Domain.Models.ProductType.Food => Presentation.Protos.ProductType.Food,
        _ => Presentation.Protos.ProductType.General
    };

    private static void ThrowIfInvalid(FluentValidation.Results.ValidationResult validation)
    {
        if (validation.IsValid)
            return;

        throw new RpcException(new Status(
            StatusCode.InvalidArgument,
            string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}