using System.Linq.Expressions;
using HotelBooking.application.Helpers;
using HotelBooking.infrastructure.Models;

public interface IBaseManage<TEntity, TRepo, TDto, TCreateOrUpdateDTO> : ICommonManage<TDto, TCreateOrUpdateDTO>
{
}

public abstract class BaseManage<TEntity, TRepo, TDto, TCreateOrUpdateDTO> : IBaseManage<TEntity, TRepo, TDto, TCreateOrUpdateDTO>
    where TEntity : class
    where TRepo : IRepository<TEntity>
{
    // Implementation of base management functionalities
    protected readonly TRepo _repo;
    protected readonly IUnitOfWork _dbu;

    public BaseManage(TRepo repo, IUnitOfWork dbu)
    {
        _repo = repo;
        _dbu = dbu;
    }

    // --- ĐỊNH NGHĨA 3 HÀM BẮT BUỘC CON PHẢI LÀM ---

    // 1. Map từ Entity -> DTO (Dùng cho GetAll, GetById, return Create)
    protected abstract TDto MapToDto(TEntity entity);

    // 2. Map từ CreateDTO -> Entity (Dùng cho Create)
    protected abstract TEntity MapToEntity(TCreateOrUpdateDTO createDto);

    // 3. Map từ UpdateDTO vào Entity CÓ SẴN (Dùng cho Update, Deleted)
    protected abstract void MapToEntity(TCreateOrUpdateDTO updateDto, TEntity entity);

    public virtual async Task<ApiResponse<TDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
            {
                return ResponseFactory.Failure<TDto>(StatusCodeResponse.NotFound, MessageResponse.NOT_FOUND);
            }
            // Mapping logic from TEntity to TDto should be implemented here
            TDto dto = MapToDto(entity);

            return ResponseFactory.Success(dto, MessageResponse.GET_SUCCESSFULLY);
        }
        catch (Exception)
        {
            return ResponseFactory.ServerError<TDto>();
        }
    }

    public virtual async Task<ApiResponse<TDto>> CreateAsync(TCreateOrUpdateDTO createDto)
    {
        try
        {
            var entity = MapToEntity(createDto);
            await _repo.AddAsync(entity);
            await _dbu.SaveChangesAsync();

            // Mapping logic from TEntity to TDto should be implemented here
            TDto dto = MapToDto(entity);

            return ResponseFactory.Success(dto, MessageResponse.CREATE_SUCCESSFULLY);
        }
        catch (Exception)
        {
            return ResponseFactory.ServerError<TDto>();
        }
    }

    public virtual async Task<ApiResponse<TDto>> UpdateAsync(int id, TCreateOrUpdateDTO updateDto)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);

            MapToEntity(updateDto, entity);
            await _repo.UpdateAsync(entity);
            await _dbu.SaveChangesAsync();

            // Mapping logic from TEntity to TDto should be implemented here
            TDto dto = MapToDto(entity);

            return ResponseFactory.Success(dto, MessageResponse.UPDATE_SUCCESSFULLY);
        }
        catch (Exception)
        {
            return ResponseFactory.ServerError<TDto>();
        }
    }

    public virtual async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
            {
                return ResponseFactory.Failure<bool>(StatusCodeResponse.NotFound, MessageResponse.NOT_FOUND);
            }

            // --- CÁCH MỚI: DÙNG INTERFACE (Rich Model + Type Safe) ---

            // Kiểm tra xem Entity này có hỗ trợ chức năng Xóa mềm hay không?
            if (entity is ISoftDelete softDeleteEntity)
            {
                // GỌI HÀNH VI (Behavior):
                // Thay vì tự tay set: entity.IsDeleted = true (Anemic)
                // Ta ra lệnh: "Hãy tự xóa mình đi" (Rich)
                softDeleteEntity.Delete();

                // Chỉ cần update và save
                await _repo.UpdateAsync(entity);
                await _dbu.SaveChangesAsync();

                return ResponseFactory.Success(true, MessageResponse.DELETE_SUCCESSFULLY);
            }
            else
            {
                // --- KHÔNG XÓA CỨNG ---
                // Nếu bảng không có cột IsDeleted, ta trả về lỗi báo cho Developer biết
                // để tránh việc xóa nhầm dữ liệu quan trọng.
                return ResponseFactory.Failure<bool>(StatusCodeResponse.BadRequest, MessageResponse.DELETE_FAILED);
            }
        }
        catch (Exception)
        {
            return ResponseFactory.ServerError<bool>();
        }
    }
}