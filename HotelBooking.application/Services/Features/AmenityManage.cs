using System.Text.Json;
using HotelBooking.application.Helpers;
using HotelBooking.infrastructure.Models;

public interface IAmenityManage : IStandardManage<AmenityDTO, AmenityCreateOrUpdateDTO>;

public class AmenityManage : BaseManage<Amenity, IAmenityRepository, AmenityDTO, AmenityCreateOrUpdateDTO>, IAmenityManage
{
    public AmenityManage(IAmenityRepository repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }


    protected override AmenityDTO MapToDto(Amenity entity)
    {
        var additional = JsonSerializer.Deserialize<Dictionary<string, string?>>(entity.Additional ?? "{}") ?? new Dictionary<string, string?>();

        return new AmenityDTO
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = additional.GetValueOrDefault("Description", null),
            IconClass = additional.GetValueOrDefault("IconClass", null) ?? "",
            IconColor = additional.GetValueOrDefault("IconColor", null) ?? "blue",
        };
    }

    protected override void MapToEntity(AmenityCreateOrUpdateDTO updateDto, Amenity entity)
    {
        // DÙNG HÀM: Gọi phương thức hành vi
        // Service không còn biết về JSON hay logic default color "blue" nữa
        entity.Update(updateDto.Name, updateDto.Description, updateDto.IconClass, updateDto.IconColor);
    }

    protected override Amenity MapToEntity(AmenityCreateOrUpdateDTO createDto)
    {
        // DÙNG HÀM: Gọi Factory Method
        // Không còn new Amenity() { ... }
        return Amenity.Create(createDto.Name, createDto.Description, createDto.IconClass, createDto.IconColor);
    }

    public async Task<ApiResponse<List<AmenityDTO>>> GetAllAsync()
    {
        var amenities = await _repo.WhereAsync(a => a.IsDeleted == false);

        if (amenities == null || amenities.Count() == 0)
        {
            return ResponseFactory.Failure<List<AmenityDTO>>(StatusCodeResponse.NotFound, MessageResponse.EMPTY_LIST);
        }

        try
        {
            var result = amenities.Select(a => MapToDto(a)).ToList();

            return ResponseFactory.Success(result, MessageResponse.GET_SUCCESSFULLY);
        }
        catch (Exception)
        {
            return ResponseFactory.ServerError<List<AmenityDTO>>();
        }
    }

    public override async Task<ApiResponse<AmenityDTO>> CreateAsync(AmenityCreateOrUpdateDTO createDto)
    {
        if(string.IsNullOrEmpty(createDto.Name) || string.IsNullOrWhiteSpace(createDto.Name)) return ResponseFactory.Failure<AmenityDTO>(StatusCodeResponse.BadRequest, MessageResponse.EMPTY_NAME);

        // 1. Kiểm tra trùng tên
        var exists = await _repo.AnyAsync(a => a.Name.ToLower() == createDto.Name.ToLower() && a.IsDeleted == false);

        if (exists)
        {
            return ResponseFactory.Failure<AmenityDTO>(StatusCodeResponse.Conflict, MessageResponse.NAME_ALREADY_EXISTS);
        }

        // 2. Nếu không trùng, gọi hàm cha để tiếp tục quy trình chuẩn (Map -> Add -> Save)
        return await base.CreateAsync(createDto);
    }

    public override async Task<ApiResponse<AmenityDTO>> UpdateAsync(int id, AmenityCreateOrUpdateDTO updateDto)
    {
        var exists = await _repo.AnyAsync(x => x.Id != id && x.Name == updateDto.Name && x.IsDeleted == false && x.Id != id);

        if (exists)
        {
            return ResponseFactory.Failure<AmenityDTO>(StatusCodeResponse.Conflict, MessageResponse.NAME_ALREADY_EXISTS);
        }

        // 2. Gọi hàm cha để update
        return await base.UpdateAsync(id, updateDto);
    }

    public override async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        return await base.DeleteAsync(id);
    }
}